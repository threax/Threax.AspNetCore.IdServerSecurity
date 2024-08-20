using System;
using System.Security.Claims;

namespace Threax.AspNetCore.AuthCore
{
    public class AuthorizeUserContext
    {
        private bool rejected = false;

        public AuthorizeUserContext(ClaimsPrincipal claimsPrincipal, IServiceProvider requestServices)
        {
            this.ClaimsPrincipal = claimsPrincipal;
            this.RequestServices = requestServices;
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

        public IServiceProvider RequestServices { get; private set; }
    }
}
