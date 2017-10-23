using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Threax.AspNetCore.IdServerAuth
{
    public static class HttpContextExtensions
    {
        public static Task SignOutOfIdServer(this HttpContext httpContext)
        {
            return httpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        }
    }
}
