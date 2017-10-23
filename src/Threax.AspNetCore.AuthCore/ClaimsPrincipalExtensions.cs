using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Threax.AspNetCore.AuthCore
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Get the access token.
        /// </summary>
        /// <param name="identity">The identity to read from.</param>
        /// <returns>The access token string.</returns>
        public static String GetAccessToken(this ClaimsPrincipal identity)
        {
            return identity.Claims.FirstOrDefault(i => i.Type == ClaimTypes.AccessToken)?.Value;
        }

        /// <summary>
        /// Get the user's guid.
        /// </summary>
        /// <param name="identity">The identity to read from.</param>
        /// <returns>The guid for the user.</returns>
        public static Guid GetUserGuid(this ClaimsPrincipal identity)
        {
            return new Guid(identity.Claims.FirstOrDefault(i => i.Type == ClaimTypes.ObjectGuid).Value);
        }

        /// <summary>
        /// Get the user id as a nice string suitable for printing. Don't use this for anything but logging.
        /// </summary>
        /// <param name="identity">The identity to read from.</param>
        /// <returns>The guid for the user.</returns>
        public static String GetUserLogString(this ClaimsPrincipal identity)
        {
            if (identity == null)
            {
                return "null user";
            }
            var guid = identity.GetUserGuid();
            //var fullName = identity.GetFullName();
            //return $"{guid} - {fullName}";
            return guid.ToString();
        }
    }
}
