using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Application.Models.Identity {

    public class UserDto {

        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public List<IdentityRole> Roles { get; set; }
    }
}
