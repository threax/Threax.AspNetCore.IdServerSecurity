using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.Authorization
{
    /// <summary>
    /// This class holds the names of the Authorization Schemes that are supported.
    /// </summary>
    public static class AuthCoreSchemes
    {
        /// <summary>
        /// This is the authorization scheme to allow bearer tokens. Use this on your api endpoints
        /// to ensure that they will ignore cookie based requests since the bearer token can act as
        /// an xsrf token as well.
        /// </summary>
        public const String Bearer = "Bearer";

        /// <summary>
        /// This is the authorization scheme to allow cookies. Use this on your ui endpoints to ensure
        /// that they will get user from the request cookies. This should not be used on any endpoints
        /// with side effects, since cookies are always sent by the browser and have no xsrf protection.
        /// </summary>
        public const String Cookies = "JwtCookies";
    }
}
