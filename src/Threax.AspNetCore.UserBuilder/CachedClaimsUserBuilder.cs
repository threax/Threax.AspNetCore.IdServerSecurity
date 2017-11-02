using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Threax.AspNetCore.UserBuilder
{
    public class CachedClaimsUserBuilder : UserAuthorizerBuilderLink
    {
        private IClaimCache claimCache;

        public CachedClaimsUserBuilder(IClaimCache claimCache, IUserBuilder next)
            :base(next)
        {
            this.claimCache = claimCache;
        }

        public override async Task<bool> ValidateAndBuildUser(ClaimsPrincipal principal)
        {
            if (await this.claimCache.GetClaims(principal))
            {
                return true;
            }

            var result = await this.ChainNext(true, principal);
            if (result)
            {
                await this.claimCache.UpdateClaims(principal);
            }
            return result;
        }
    }
}
