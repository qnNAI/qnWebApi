using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Security {

    public class JwtSettings {
        public string Secret { get; set; } = null!;
        public int TokenLifetimeSeconds { get; set; }
    }
}
