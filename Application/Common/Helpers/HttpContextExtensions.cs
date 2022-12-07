using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Helpers {

    public static class HttpContextExtensions {

        public static string GetUserId(this HttpContext context) {
            if(context is null || context.User is null) {
                return string.Empty;
            }
            return context.User.Claims.FirstOrDefault(x => x.Type == "Id")?.Value ?? string.Empty;
        }
    }
}
