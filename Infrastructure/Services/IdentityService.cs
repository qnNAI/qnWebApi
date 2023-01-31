using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces.Services;
using Application.Common.Security;
using Domain.Entities;
using Application.Models.Identity;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Infrastructure.Persistence.Contexts;

namespace Infrastructure.Services {

    public class IdentityService : IIdentityService {
        private readonly JwtSettings _jwtSettings;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly TokenValidationParameters _tokenValidationParameters;

        public IdentityService(JwtSettings settings, UserManager<ApplicationUser> userManager, ApplicationDbContext context, TokenValidationParameters tokenValidationParameters) {
            _jwtSettings = settings;
            _userManager = userManager;
            _context = context;
            _tokenValidationParameters = tokenValidationParameters;
        }

        public async Task<IEnumerable<UserDto>> GetUsersAsync() {
            var users = await _userManager.Users.ProjectToType<UserDto>().ToListAsync();

            return users;
        }

        public async Task<AuthenticateResponse> SignInAsync(SignInRequest request) {
            if (request is null) {
                throw new ArgumentNullException(nameof(request));
            }

            var user = await _userManager.FindByEmailAsync(request.Login) ??
                await _userManager.FindByNameAsync(request.Login);

            if (user is null) {
                return new AuthenticateResponse {
                    Succeeded = false,
                    Errors = new[] {
                        new IdentityError {
                            Code = "UserNotFound",
                            Description = "User not found"
                        }
                    }
                };
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!isPasswordValid) {
                return new AuthenticateResponse {
                    Succeeded = false,
                    Errors = new[] {
                        new IdentityError {
                            Code = "PasswordInvalid",
                            Description = "Password invalid"
                        }
                    }
                };
            }

            var token = GenerateJwtToken(user);
            var refreshToken = await GetRefreshTokenForUserAsync(user, token.Id);

            return new AuthenticateResponse {
                Succeeded = true,
                Token = token.Token,
                RefreshToken = refreshToken.Token,
                RefreshTokenExp = refreshToken.Expires
            };
        }

        public async Task<AuthenticateResponse> SignUpAsync(SignUpRequest request) {
            if (request is null) {
                throw new ArgumentNullException(nameof(request));
            }

            var appUser = request.Adapt<ApplicationUser>();
            var result = await _userManager.CreateAsync(appUser, request.Password);

            if (!result.Succeeded) {
                return result.Adapt<AuthenticateResponse>();
            }

            var user = await _userManager.FindByNameAsync(request.Username);
            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken(user, token.Id);

            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);

            return new AuthenticateResponse {
                Succeeded = true,
                Token = token.Token,
                RefreshToken = refreshToken.Token,
                RefreshTokenExp = refreshToken.Expires
            };
        }

        public async Task<AuthenticateResponse> RefreshTokenAsync(TokenRequest request) {
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenValidationParameters = _tokenValidationParameters.Clone();
            tokenValidationParameters.ValidateLifetime = false; // token has to be expired
            var principal = tokenHandler.ValidateToken(request.Token, tokenValidationParameters, out var validatedToken);

            var utcExpDate = long.Parse(principal.Claims.First(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
            var expDate = UnixTimeStampToDateTime(utcExpDate);

            if(expDate > DateTime.UtcNow) {
                return new AuthenticateResponse {
                    Succeeded = false,
                    Errors = new[] {
                        new IdentityError {
                            Code = "TokenNotExpired",
                            Description = "Token is not expired yet"
                        }
                    }
                };
            }

            var storedRefreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == request.RefreshToken);

            if(storedRefreshToken == null) {
                return new AuthenticateResponse {
                    Succeeded = false,
                    Errors = new[] {
                        new IdentityError {
                            Code = "RefreshTokenDoesNotExist",
                            Description = "Refresh token does not exist"
                        }
                    }
                };
            }

            if(DateTime.UtcNow > storedRefreshToken.Expires) {
                return new AuthenticateResponse {
                    Succeeded = false,
                    Errors = new[] {
                        new IdentityError {
                            Code = "RefreshTokenExpired",
                            Description = "Refresh token has expired"
                        }
                    }
                };
            }

            if(storedRefreshToken.IsRevoked) {
                return new AuthenticateResponse {
                    Succeeded = false,
                    Errors = new[] {
                        new IdentityError {
                            Code = "RefreshTokenRevoked",
                            Description = "Refresh token has been revoked"
                        }
                    }
                };
            }

            if(storedRefreshToken.IsUsed) {
                return new AuthenticateResponse {
                    Succeeded = false,
                    Errors = new[] {
                        new IdentityError {
                            Code = "RefreshTokenUsed",
                            Description = "Refresh token has already been used"
                        }
                    }
                };
            }

            var jti = principal.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;

            if (jti == null || storedRefreshToken.JwtId != jti) {
                return new AuthenticateResponse {
                    Succeeded = false,
                    Errors = new[] {
                        new IdentityError {
                            Code = "InvalidJwtToken",
                            Description = "Jwt token is invalid"
                        }
                    }
                };
            }

            storedRefreshToken.IsUsed = true;
            _context.RefreshTokens.Update(storedRefreshToken);
            await _context.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(storedRefreshToken.UserId);
            var token = GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken(user, token.Id);

            user.RefreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);

            return new AuthenticateResponse {
                Succeeded = true,
                Token = token.Token,
                RefreshToken = newRefreshToken.Token,
                RefreshTokenExp = newRefreshToken.Expires
            };
        }

        private JwtTokenResponse GenerateJwtToken(ApplicationUser user) {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            var claims = new List<Claim> {
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(JwtRegisteredClaimNames.Jti, user.Id),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new(nameof(user.Id), user.Id)
            };

            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddSeconds(_jwtSettings.TokenLifetimeSeconds), // UtcNow throws
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature),
                NotBefore = DateTime.UtcNow
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new JwtTokenResponse {
                Id = token.Id,
                Token = tokenHandler.WriteToken(token)
            };
        }

        private RefreshToken GenerateRefreshToken(ApplicationUser user, string jwtId) {
            var randomNum = new byte[32];

            using var random = RandomNumberGenerator.Create();
            random.GetNonZeroBytes(randomNum);

            return new RefreshToken {
                Token = Convert.ToBase64String(randomNum),
                UserId = user.Id,
                Created = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMonths(1),
                JwtId = jwtId
            };
        }

        private async Task<RefreshToken> GetRefreshTokenForUserAsync(ApplicationUser user, string jwtId) {
            var existing = user.RefreshTokens.FirstOrDefault(x => x.IsActive);

            if(existing != null) {
                existing.JwtId = jwtId;
                return existing;
            }

            var token = GenerateRefreshToken(user, jwtId);

            user.RefreshTokens.Add(token);
            await _userManager.UpdateAsync(user);

            return token;
        }

        private DateTime UnixTimeStampToDateTime(long unixTimeStamp) {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToUniversalTime();
            return dateTime;
        }
    }
}
