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

namespace Infrastructure.Services {

    public class IdentityService : IIdentityService {
        private readonly JwtSettings _jwtSettings;
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityService(JwtSettings settings, UserManager<ApplicationUser> userManager) {
            _jwtSettings = settings;
            _userManager = userManager;
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
            return new AuthenticateResponse {
                Succeeded = true,
                Token = token
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

            return new AuthenticateResponse {
                Succeeded = true,
                Token = token
            };
        }

        private string GenerateJwtToken(ApplicationUser user) {
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
                Expires = DateTime.Now.AddSeconds(_jwtSettings.TokenLifetimeSeconds), // UtcNow throws
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
