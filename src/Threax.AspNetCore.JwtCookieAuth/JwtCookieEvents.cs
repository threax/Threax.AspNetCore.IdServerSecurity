using System;
using System.Threading.Tasks;
using Threax.AspNetCore.AuthCore;

namespace Threax.AspNetCore.JwtCookieAuth
{
    public class JwtCookieEvents
    {
        public virtual async Task ValidatePrincipal(AuthorizeUserContext valiationContext)
        {
            if(OnValidatePrincipal != null)
            {
                await OnValidatePrincipal.Invoke(valiationContext);
            }
        }

        public Func<AuthorizeUserContext, Task> OnValidatePrincipal { get; set; }
    }
}
