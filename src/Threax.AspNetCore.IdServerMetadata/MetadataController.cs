using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Threax.AspNetCore.IdServerMetadata
{
    [Route("[controller]/[action]")]
    [Authorize(AuthenticationSchemes = MetadataConstants.AuthenticationScheme)]
    public class MetadataController : Controller
    {
        private MetadataOptions options;

        public MetadataController(MetadataOptions options)
        {
            this.options = options;
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
                    //Remove the /Metadata off the end, the rest is our virtual directory path
                    var pathStr = pathBase.Value;
                    if (pathStr.Length > 9)
                    {
                        pathBaseValue = pathStr.Remove(pathStr.Length - 9);
                    }
                }
                var host = $"https://{currentUri.Authority}{pathBaseValue}";
                clientMetadata.LogoutUri = clientMetadata.LogoutUri.Replace(MetadataConstants.HostVariable, host);
                clientMetadata.RedirectUris = clientMetadata.RedirectUris.Select(i => i.Replace(MetadataConstants.HostVariable, host)).ToList();
            }

            return clientMetadata;
        }
    }
}
