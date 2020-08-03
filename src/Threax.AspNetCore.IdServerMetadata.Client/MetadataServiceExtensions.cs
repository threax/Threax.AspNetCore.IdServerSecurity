using System;
using System.Collections.Generic;
using System.Text;
using Threax.AspNetCore.IdServerMetadata;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Threax.AspNetCore.IdServerMetadata.Client;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MetadataServiceExtensions
    {
        public static IServiceCollection AddIdServerMetadataClient(this IServiceCollection builder)
        {
            builder.AddHttpClient<IMetadataClient, MetadataClient>();

            return builder;
        }
    }
}
