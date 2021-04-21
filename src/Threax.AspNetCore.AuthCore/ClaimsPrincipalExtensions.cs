using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Threax.AspNetCore.AuthCore
{
    public static class ClaimsPrincipalExtensions
    {
        private const string NameIdentifierType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";

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
            var sub = identity.FindFirst(NameIdentifierType);
            if(sub == null)
            {
                throw new InvalidOperationException($"Cannot find '{NameIdentifierType}' claim on identity.");
            }
            return new Guid(sub.Value);
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
