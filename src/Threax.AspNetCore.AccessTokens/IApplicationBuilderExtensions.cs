using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Threax.AspNetCore.AccessTokens;

namespace Microsoft.AspNetCore.Builder
{
    public static class IApplicationBuilderExtensions
    {
        public static IMvcBuilder AddAccessTokenController(this IMvcBuilder builder, Action<AccessTokenOptions> optionsBuilder)
        {
            builder.Services.AddSingleton<AccessTokenOptions>(s =>
            {
                var options = new AccessTokenOptions();

                if (optionsBuilder != null)
                {
                    optionsBuilder.Invoke(options);
                }

                return options;
            });

            builder.AddApplicationPart(typeof(IApplicationBuilderExtensions).Assembly);

            return builder;
        }
    }
}
