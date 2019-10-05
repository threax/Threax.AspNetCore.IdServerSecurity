using Microsoft.AspNetCore.Antiforgery;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Threax.AspNetCore.Xsrf;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddConventionalXsrf(this IServiceCollection services, Action<XsrfOptions> configureOptions)
        {
            var options = new XsrfOptions();
            configureOptions?.Invoke(options);

            services.AddScoped<IXsrfTokenCookieManager, XsrfTokenCookieManager>();
            services.AddSingleton(options);
            services.AddAntiforgery(o =>
            {
                o.Cookie = options.AntiforgeryCookie;
            });

            return services;
        }
    }
}
