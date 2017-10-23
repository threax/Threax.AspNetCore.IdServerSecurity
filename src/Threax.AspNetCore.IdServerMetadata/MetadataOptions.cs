using System;
using System.Collections.Generic;
using System.Text;

namespace Threax.AspNetCore.IdServerMetadata
{
    public class MetadataOptions
    {
        /// <summary>
        /// The resource metadata settings.
        /// </summary>
        public ApiResourceMetadata Resource { get; set; }

        public void CreateConventionalResource(String scopeName, String displayName)
        {
            Resource = new ApiResourceMetadata()
            {
                DisplayName = displayName,
                ScopeName = scopeName
            };
        }

        /// <summary>
        /// The client metadata settings.
        /// </summary>
        public ClientMetadata Client { get; set; }

        /// <summary>
        /// Set the Client propety to a conventional setup for client metadata. This makes it easier to create this metadata
        /// without needing to supply the full configuration.
        /// </summary>
        /// <param name="clientId">The id of the client.</param>
        /// <param name="clientName">A pretty name for the client.</param>
        /// <param name="scope">The scope the client will use.</param>
        public void CreateConventionalClient (String clientId, String clientName, String scope, String logoutUri)
        {
            if (String.IsNullOrEmpty(logoutUri))
            {
                throw new InvalidOperationException("You must provide a logout uri.");
            }

            logoutUri = logoutUri.Replace('\\', '/');
            if(logoutUri[0] != '/')
            {
                logoutUri = "/" + logoutUri;
            }

            Client = new ClientMetadata()
            {
                ClientId = clientId,
                Name = clientName,
                AllowedGrantTypes = new List<string>() { "hybrid" },
                RedirectUris = new List<string>() { $"{MetadataConstants.HostVariable}/signin-oidc" },
                LogoutUri = $"{MetadataConstants.HostVariable}{logoutUri}",
                AllowedScopes = new List<string>()
                {
                    "openid",
                    "profile",
                    "offline_access",
                    scope
                },
                LogoutSessionRequired = true,
                EnableLocalLogin = false,
                AccessTokenLifetime = 3600
            };
        }
    }
}
