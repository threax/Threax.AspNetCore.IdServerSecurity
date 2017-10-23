using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Threax.AspNetCore.AuthCore
{
    public class AuthorizeUserContext
    {
        private bool rejected = false;

        public AuthorizeUserContext(ClaimsPrincipal claimsPrincipal, HttpContext httpContext)
        {
            this.ClaimsPrincipal = claimsPrincipal;
            this.HttpContext = httpContext;
        }

        public void Reject()
        {
            rejected = true;
        }

        public bool IsRejected
        {
            get
            {
                return rejected;
            }
        }

        public ClaimsPrincipal ClaimsPrincipal { get; private set; }

        public HttpContext HttpContext { get; private set; }
    }
}
