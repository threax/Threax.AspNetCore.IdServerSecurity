using System;
using System.Collections.Generic;
using System.Text;
using Threax.AspNetCore.IdServerMetadata;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MetadataServiceExtensions
    {
        public static AuthenticationBuilder AddIdServerMetadataAuth(this AuthenticationBuilder builder, Action<JwtBearerOptions> optionsBuilder)
        {
            builder.AddJwtBearer(MetadataConstants.AuthenticationScheme, o =>
             {
                 o.Audience = "identityserver";
                 o.RequireHttpsMetadata = true;
                 optionsBuilder.Invoke(o);

                 o.Events = new JwtBearerEvents()
                 {
                     OnMessageReceived = s =>
                     {
                         //Get token out of "bearer" header
                         s.Token = s.HttpContext.Request.Headers["bearer"];
                         return System.Threading.Tasks.Task.FromResult(0);
                     }
                 };
             });

            return builder;
        }

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
