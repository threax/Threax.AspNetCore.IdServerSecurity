using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <param name="logoutUri">The logout uri.</param>
        /// <param name="additionalScopes">Additional required scopes.</param>
        public void CreateConventionalClient (String clientId, String clientName, String scope, String logoutUri, IEnumerable<String> additionalScopes)
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

            var allowedScopes = new List<string>()
            {
                "openid",
                "profile",
                "offline_access",
                scope
            };
            if(additionalScopes != null)
            {
                allowedScopes.AddRange(additionalScopes);
            }

            Client = new ClientMetadata()
            {
                ClientId = clientId,
                Name = clientName,
                AllowedGrantTypes = new List<string>() { "hybrid" },
                RedirectUris = new List<string>() { $"{MetadataConstants.HostVariable}/signin-oidc" },
                LogoutUri = $"{MetadataConstants.HostVariable}{logoutUri}",
                AllowedScopes = allowedScopes,
                LogoutSessionRequired = true,
                EnableLocalLogin = false,
                AccessTokenLifetime = 3600
            };
        }

        public ClientMetadata ClientCredentials { get; set; }

        /// <summary>
        /// Set the Client propety to a conventional setup for client metadata. This makes it easier to create this metadata
        /// without needing to supply the full configuration.
        /// </summary>
        /// <param name="clientId">The id of the client.</param>
        /// <param name="clientName">A pretty name for the client.</param>
        /// <param name="scopes">Additional required scopes.</param>
        public void CreateConventionalClientCredentials(String clientId, String clientName, IEnumerable<String> scopes)
        {
            ClientCredentials = new ClientMetadata()
            {
                ClientId = clientId + ".ClientCreds",
                Name = clientName + " Client Credentials",
                AllowedGrantTypes = new List<string>() { "client_credentials" },
                AllowedScopes = scopes.ToList(),
                LogoutSessionRequired = false,
                EnableLocalLogin = false,
                AccessTokenLifetime = 3600
            };
        }
    }
}
