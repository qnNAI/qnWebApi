using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Models.Identity;

namespace Application.Common.Interfaces.Services {

    public interface IIdentityService {

        Task<IEnumerable<UserDto>> GetUsersAsync();
        Task<AuthenticateResponse> SignInAsync(SignInRequest request);
        Task<AuthenticateResponse> SignUpAsync(SignUpRequest request);
    }
}
