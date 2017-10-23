using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Threax.AspNetCore.UserBuilder
{
    /// <summary>
    /// A base class to easily create sublcasses that can be chained together to determine if a user is
    /// valid. The user is considered valid if any of the links in the chain consider the user valid.
    /// </summary>
    public abstract class UserAuthorizerBuilderLink : IUserBuilder
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="next">The next item in the chain.</param>
        public UserAuthorizerBuilderLink(IUserBuilder next)
        {
            this.Next = next;
        }

        /// <summary>
        /// Determine if a claims principal is valid. Returns true if it is.
        /// </summary>
        /// <param name="principal">The principal to inspect.</param>
        /// <returns>True if valid, false otherwise.</returns>
        public abstract Task<bool> ValidateAndBuildUser(ClaimsPrincipal principal);

        /// <summary>
        /// Call this from your IsUserAuthorized function to call the next link in the chain.
        /// Uses or to determine if it should keep going down the chain.
        /// </summary>
        /// <param name="valid">The true or false result from this part of the chain.</param>
        /// <param name="principal">The claims principal to inspect.</param>
        /// <returns>True if valid, false otherwise.</returns>
        protected async virtual Task<bool> ChainNext(bool valid, ClaimsPrincipal principal)
        {
            if(valid && Next != null)
            {
                valid = await Next.ValidateAndBuildUser(principal);
            }
            return valid;
        }

        /// <summary>
        /// The user authorizers can be chained together. Null means your at the end of the chain.
        /// </summary>
        public IUserBuilder Next { get; set; }
    }
}
