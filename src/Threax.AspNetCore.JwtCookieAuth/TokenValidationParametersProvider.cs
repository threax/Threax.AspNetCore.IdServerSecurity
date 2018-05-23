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
using System.Threading.Tasks;

namespace Threax.AspNetCore.JwtCookieAuth
{
    class TokenValidationParametersProvider : ITokenValidationParametersProvider
    {
        public TokenValidationParametersProvider()
        {
            
        }

        public async Task<TokenValidationParameters> GetTokenValidationParameters(JwtCookieAuthenticationOptions options)
        {
            //Grab the metadata
            using (var handler = new HttpClientHandler())
            using (var httpClient = new HttpClient(handler))
            {
                httpClient.Timeout = TimeSpan.FromMinutes(1);
                httpClient.MaxResponseContentBufferSize = 1024 * 1024 * 10; // 10 MB

                Uri metadataUri = new Uri(new Uri(options.Authority), options.MetadataPath);

                var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                    metadataUri.AbsoluteUri,
                    new OpenIdConnectConfigurationRetriever(),
                    new HttpDocumentRetriever(httpClient) { RequireHttps = true }
                );

                var config = await configManager.GetConfigurationAsync(new CancellationToken());

                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = config.SigningKeys.FirstOrDefault(),

                    ValidateIssuer = true,
                    ValidIssuer = config.Issuer,

                    ValidateAudience = false,

                    ValidateLifetime = false,
                    ClockSkew = TimeSpan.FromSeconds(5),
                };

                return tokenValidationParameters;
            }
        }
    }
}
