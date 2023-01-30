using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Identity {

    public class JwtTokenResponse {

        public string Id { get; set; } = null!;

        public string Token { get; set; } = null!;
    }
}
