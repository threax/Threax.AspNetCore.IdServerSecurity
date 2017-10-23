using System.Security.Claims;
using System.Threading.Tasks;

namespace Threax.AspNetCore.UserBuilder
{
    /// <summary>
    /// This authorizer always lets the user through, used mostly to shim if we don't have
    /// any other IUserBuilder.
    /// </summary>
    public class AllAccessAuthorizer : UserAuthorizerBuilderLink
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="next">The next policy</param>
        public AllAccessAuthorizer(IUserBuilder next = null)
            : base(next)
        {
            
        }

        /// <summary>
        /// Always passes.
        /// </summary>
        /// <param name="principal">The user to check.</param>
        /// <returns>Always returns true.</returns>
        public override Task<bool> ValidateAndBuildUser(ClaimsPrincipal principal)
        {
            return this.ChainNext(true, principal);
        }
    }
}
