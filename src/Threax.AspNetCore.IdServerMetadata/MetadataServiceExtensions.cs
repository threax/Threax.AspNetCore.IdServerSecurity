using System;
using System.Collections.Generic;
using System.Text;
using Threax.AspNetCore.IdServerMetadata;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MetadataServiceExtensions
    {
        public static IMvcBuilder AddIdServerMetadata(this IMvcBuilder builder, Action<MetadataOptions> optionsBuilder)
        {
            //Build the options each time they are requested, this will not happen that much
            builder.Services.AddScoped<MetadataOptions>(s =>
            {
                var options = new MetadataOptions();

                if (optionsBuilder != null)
                {
                    optionsBuilder.Invoke(options);
                }

                return options;
            });

            builder.AddApplicationPart(typeof(MetadataServiceExtensions).Assembly);

            return builder;
        }
    }
}
