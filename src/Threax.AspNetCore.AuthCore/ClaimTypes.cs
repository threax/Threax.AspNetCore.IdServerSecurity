using System;

namespace Threax.AspNetCore.AuthCore
{
    public class ClaimTypes
    {
        /// <summary>
        /// The claim name for access tokens.
        /// </summary>
        public const String AccessToken = "AccessToken";

        /// <summary>
        /// The name of the claim for the user guid.
        /// </summary>
        public const String ObjectGuid = "objectGUID";
    }
}
