using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Threax.AspNetCore.JwtCookieAuth
{
    public class JwtCookieAuthenticationOptions : AuthenticationSchemeOptions
    {
        /// <summary>
        /// The path the cookie will live on.
        /// </summary>
        public String CookiePath { get; set; }

        /// <summary>
        /// The authority to trust for id management.
        /// </summary>
        public String Authority { get; set; }

        /// <summary>
        /// The path to the metadata.
        /// </summary>
        public String MetadataPath { get; set; } = "/.well-known/openid-configuration";

        /// <summary>
        /// The client id for the bearer token.
        /// </summary>
        public String ClientId { get; set; }

        /// <summary>
        /// The client secret for the bearer token.
        /// </summary>
        public String ClientSecret { get; set; }

        /// <summary>
        /// The authentication events.
        /// </summary>
        public new JwtCookieEvents Events
        {
            get => (JwtCookieEvents)base.Events;
            set => base.Events = value;
        }

        /// <summary>
        /// The algorithm used to secure the security token.
        /// </summary>
        public String SecurityTokenAlgo { get; set; } = SecurityAlgorithms.RsaSha256;

        /// <summary>
        /// The scheme to use when challenging the user.
        /// </summary>
        public String ChallengeScheme { get; set; }

        /// <summary>
        /// The AccessDeniedPath property informs the handler that it should change an outgoing 403 Forbidden status
        /// code into a 302 redirection onto the given path.
        /// </summary>
        public string AccessDeniedPath { get; set; }

        /// <summary>
        /// Set this to true (default) to store cookies in session memory with no expiration instead of using the token's
        /// expiration date. Tokens will still expire on their own, so this won't make them last forever.
        /// </summary>
        public bool StoreCookiesInSession { get; set; } = true;

        /// <summary>
        /// Set this to true (default) to validate the audience for jwt cookies. This is reccomended.
        /// </summary>
        public bool ValidateAudience { get; set; } = true;

        /// <summary>
        /// Set this to an enumerable over all the valid audiences.
        /// </summary>
        public IEnumerable<string> ValidAudiences { get; set; }

        /// <summary>
        /// Set this to true (default) to validate the token lifetime.
        /// </summary>
        public bool ValidateLifetime { get; set; } = true;

        /// <summary>
        /// Gets or sets the clock skew to apply when validating a time. Default is 5 seconds.
        /// </summary>
        public TimeSpan ClockSkew { get; set; } = TimeSpan.FromSeconds(5);

        /// <summary>
        /// Set this to true to make bearer cookies http only. Default: true.
        /// </summary>
        public bool BearerHttpOnly { get; set; } = true;

        /// <summary>
        /// Set this to true to make refresh cookies http only. Default: true.
        /// </summary>
        public bool RefreshHttpOnly { get; set; } = true;
    }
}
