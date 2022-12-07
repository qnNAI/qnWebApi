using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Application.Models.Identity;
using Mapster;

namespace Infrastructure {

    public static class MappingProfile {

        public static void ApplyMappings() {
            TypeAdapterConfig<ApplicationUser, UserDto>
                .NewConfig()
                .Map(dest => dest.Username, src => src.UserName);

            TypeAdapterConfig<SignUpRequest, ApplicationUser>
                .NewConfig()
                .Map(dest => dest.UserName, src => src.Username);
        }
    }
}
