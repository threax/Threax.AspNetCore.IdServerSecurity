using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading.Tasks;
using Threax.AspNetCore.AuthCore;

namespace Threax.AspNetCore.UserBuilder
{
    /// <summary>
    /// Authorizer that allowes specific user ids access. Can be chained.
    /// </summary>
    public class UserWhitelistAuthorizer : UserAuthorizerBuilderLink
    {
        private IUsersRepository allowedUsersRepo;
        private ILogger log;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="allowedUsersRepo">The repository of valid user ids.</param>
        /// <param name="log">The log to write to.</param>
        /// <param name="next">The next user builder in the chain.</param>
        public UserWhitelistAuthorizer(IUsersRepository allowedUsersRepo, ILogger log, IUserBuilder next = null)
            : base(next)
        {
            this.allowedUsersRepo = allowedUsersRepo;
            this.log = log;
        }

        /// <summary>
        /// Determine if the user allowed individual access.
        /// </summary>
        /// <param name="principal">The principal to check.</param>
        /// <returns></returns>
        public override async Task<bool> ValidateAndBuildUser(ClaimsPrincipal principal)
        {
            bool valid = await allowedUsersRepo.HasUserId(principal.GetUserGuid());
            if (valid)
            {
                log.LogInformation($"User {principal.GetUserLogString()} is whitelisted for access.");
            }
            else
            {
                log.LogError($"Cannot log in user {principal.GetUserLogString()} user is not whitelisted for access.");
            }
            return await ChainNext(valid, principal);
        }
    }
}
