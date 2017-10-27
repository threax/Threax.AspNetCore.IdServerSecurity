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

            //Grab the metadata
            var httpClient = new HttpClient(new HttpClientHandler());
            httpClient.Timeout = TimeSpan.FromMinutes(1);
            httpClient.MaxResponseContentBufferSize = 1024 * 1024 * 10; // 10 MB

            Uri metadataUri = new Uri(new Uri(options.Authority), options.MetadataPath);

            //Tomorrow
            //TO fix the compiler errors, see if you can set these properties directly on the options
            //There is then only a couple differences between this and the original version config
            //wise, will have to see what else can be done to get scopes and stuff correct
            //probably close to being able to run this

            if (options.ConfigurationManager == null)
            {
                options.ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                    metadataUri.AbsoluteUri,
                    new OpenIdConnectConfigurationRetriever(),
                    new HttpDocumentRetriever(httpClient) { RequireHttps = true }
                );
            }

            if (options.TokenValidationParameters == null)
            {
                var configManager = options.ConfigurationManager;
                var task = configManager.GetConfigurationAsync(new CancellationToken());
                task.Wait();
                var config = task.Result;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = config.SigningKeys.FirstOrDefault(),

                    ValidateIssuer = true,
                    ValidIssuer = config.Issuer,

                    ValidateAudience = false,

                    ValidateLifetime = false,
                    ClockSkew = TimeSpan.FromSeconds(5),
                };
            }
        }
    }
}
