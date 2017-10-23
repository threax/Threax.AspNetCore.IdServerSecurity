using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Threax.AspNetCore.AuthCore;

namespace Threax.AspNetCore.UserBuilder
{
    /// <summary>
    /// This builder will add the roles specified by the users repo, it will always
    /// return true for validation.
    /// </summary>
    public class UserRoleBuilder : UserAuthorizerBuilderLink
    {
        private IUsersRepository userRepo;
        private ILogger log;

        public UserRoleBuilder(IUsersRepository userRepo, ILogger log, IUserBuilder next = null)
            : base(next)
        {
            this.userRepo = userRepo;
            this.log = log;
        }

        public override async Task<bool> ValidateAndBuildUser(ClaimsPrincipal principal)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Getting role claims for {0}", principal.GetUserLogString());
            //Only try to add claims if the identity is a ClaimsIdentity instance.
            var claimsId = principal.Identity as ClaimsIdentity;
            if (claimsId != null)
            {
                var roles = await userRepo.GetUserRoles(principal.GetUserGuid());
                foreach (var role in roles)
                {
                    sb.AppendFormat("\t{0}\n", role);
                    claimsId.AddClaim(new Claim(claimsId.RoleClaimType, role));
                }
            }
            log.LogInformation(sb.ToString());

            return await this.ChainNext(true, principal);
        }
    }
}
