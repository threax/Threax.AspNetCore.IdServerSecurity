using System;
using System.Collections.Generic;
using System.Text;

namespace Threax.AspNetCore.AccessTokens
{
    public class AccessTokenOptions
    {
        /// <summary>
        /// The name of the bearer token header.
        /// </summary>
        public String BearerHeaderName { get; set; } = "bearer";

        /// <summary>
        /// The name of the authentication scheme to use to get the access tokens.
        /// </summary>
        public string AuthenticationScheme { get; set; }
    }
}
