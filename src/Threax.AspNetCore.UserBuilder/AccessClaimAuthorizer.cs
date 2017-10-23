using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Threax.AspNetCore.UserBuilder
{
    /// <summary>
    /// This authorizer makes sure a user has a particular claim on their principal before
    /// letting them in.
    /// </summary>
    public class AccessClaimAuthorizer : UserAuthorizerBuilderLink
    {
        private String claimValue;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="claimValue">The claim value to lookup to consider the user valid.</param>
        /// <param name="next">The next user builder in the chain.</param>
        public AccessClaimAuthorizer(String claimValue, IUserBuilder next = null)
            :base(next)
        {
            this.claimValue = claimValue;
        }

        /// <summary>
        /// Determine if a user is authorized based on the type of user they claim to be.
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public override Task<bool> ValidateAndBuildUser(ClaimsPrincipal principal)
        {
            bool valid = principal.Claims.Any(c => c.Value == claimValue);
            return this.ChainNext(valid, principal);
        }
    }
}
