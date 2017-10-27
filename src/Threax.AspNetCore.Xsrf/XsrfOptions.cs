using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Threax.AspNetCore.Xsrf
{
    public class XsrfOptions
    {
        public XsrfOptions()
        {
            AntiforgeryCookie = new CookieBuilder();
            AntiforgeryCookie.HttpOnly = true;
            AntiforgeryCookie.SecurePolicy = CookieSecurePolicy.Always;

            TokenCookie = new CookieBuilder();
            AntiforgeryCookie.SecurePolicy = CookieSecurePolicy.Always;
        }

        /// <summary>
        /// The cookie properties for the Antiforgery Cookie, which is never delt with directly on the client side.
        /// </summary>
        public CookieBuilder AntiforgeryCookie { get; set; }

        /// <summary>
        /// The cookie properties for the Token Cookie that the client side will read and send back.
        /// </summary>
        public CookieBuilder TokenCookie { get; set; }

        /// <summary>
        /// The paths the xsrf tokens should be sent to.
        /// </summary>
        public List<String> Paths { get; } = new List<string>();
    }
}
