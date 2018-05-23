using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using Threax.AspNetCore.AuthCore;

namespace Threax.AspNetCore.JwtCookieAuth
{
    class PostConfigureJwtCookieAuthenticationOptions : IPostConfigureOptions<JwtCookieAuthenticationOptions>
    {
        public void PostConfigure(string name, JwtCookieAuthenticationOptions options)
        {
            if (String.IsNullOrEmpty(options.ClientId))
            {
                throw new ArgumentException("You must provide a client id in the options.");
            }

            if (String.IsNullOrEmpty(options.MetadataPath))
            {
                throw new ArgumentException("You must provide a metadata path in the options.");
            }

            if (String.IsNullOrEmpty(options.Authority))
            {
                throw new ArgumentException("You must provide an authority in the options.");
            }

            if (String.IsNullOrEmpty(options.ChallengeScheme))
            {
                throw new ArgumentException("You must provide an ChallengeScheme in the options.");
            }

            options.CookiePath = CookieUtils.FixPath(options.CookiePath);
        }
    }
}
