using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Identity {

    public class SignInRequest {
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
