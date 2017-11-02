using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Threax.AspNetCore.UserBuilder
{
    public class UserBuilderOptions
    {
        /// <summary>
        /// A callback to add additional user policies to the policy chain.
        /// </summary>
        public Func<AdditionalPoliciesCallbackArgs, IUserBuilder> ConfigureAddititionalPolicies { get; set; }

        /// <summary>
        /// Use the in memory cache for building user claims. This will only really work if the app is running in a single
        /// instance as the claims cache does not have any intra process communications. If you are going to deploy the
        /// app in a distributed way, where there could be multiple instances working together, you should disable this cache
        /// or supply a IClaimCache service that can handle the claims correctly for your application.
        /// </summary>
        public bool UseClaimsCache { get; set; } = true;

        /// <summary>
        /// A list of claim types to cache in the cache. Only the claim types specified will be included.
        /// If you have a custom user builder that adds claims other than role types you should add that claim type to this list.
        /// Defaults to ClaimTypes.Role (http://schemas.microsoft.com/ws/2008/06/identity/claims/role).
        /// </summary>
        public List<string> CacheClaimTypes { get; set; } = new List<string>() { ClaimTypes.Role };
    }
}
