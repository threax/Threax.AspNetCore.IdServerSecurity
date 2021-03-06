﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Threax.AspNetCore.IdServerMetadata
{
    [Route("[controller]/[action]")]
    public class MetadataController : Controller
    {
        private MetadataOptions options;

        public MetadataController(MetadataOptions options)
        {
            this.options = options;

            if (!options.Enabled)
            {
                throw new InvalidOperationException("Metadata controller is not enabled.");
            }
        }

        [HttpGet]
        public ApiResourceMetadata Resource()
        {
            return options.Resource;
        }

        [HttpGet]
        public ApiResourceMetadata Scope()
        {
            return Resource(); //Backward compatiblity, just return resource
        }

        [HttpGet]
        public ClientMetadata Client()
        {
            var clientMetadata = options.Client;

            //Seem to only be able to discover this during a request.
            if (clientMetadata != null)
            {
                var currentUri = new Uri(Request.GetDisplayUrl());
                var pathBaseValue = "";
                var pathBase = Request.PathBase;
                if (pathBase.HasValue)
                {
                    pathBaseValue = pathBase.Value;
                }
                var host = $"https://{currentUri.Authority}{pathBaseValue}";
                clientMetadata.LogoutUri = clientMetadata.LogoutUri.Replace(MetadataConstants.HostVariable, host);
                clientMetadata.RedirectUris = clientMetadata.RedirectUris.Select(i => i.Replace(MetadataConstants.HostVariable, host)).ToList();
            }

            return clientMetadata;
        }

        [HttpGet]
        public ClientMetadata ClientCredentials()
        {
            return options.ClientCredentials;
        }
    }
}
