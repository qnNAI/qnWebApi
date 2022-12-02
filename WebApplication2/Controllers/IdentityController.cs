using Application.Common.Interfaces.Services;
using Application.Models.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers {

    [Route("api/identity")]
    [ApiController]
    public class IdentityController : ControllerBase {
        private readonly IIdentityService _identityService;

        public IdentityController(IIdentityService identityService) {
            this._identityService = identityService;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers() {
            var users = await _identityService.GetUsersAsync();
            return Ok(users);
        }

        [HttpPost("login")]
        public async Task<IActionResult> SignIn([FromBody] SignInRequest request) {
            var result = await _identityService.SignInAsync(request);
            if(!result.Succeeded) {
                return Unauthorized(result);
            }

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<AuthenticateResponse> SignUp([FromBody] SignUpRequest request) {
            var result = await _identityService.SignUpAsync(request);
            return result;
        }
    }
}
