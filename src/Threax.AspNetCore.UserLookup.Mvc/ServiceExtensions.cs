using System;
using System.Linq;
using System.Reflection;
using Threax.AspNetCore.UserLookup;
using Threax.AspNetCore.UserLookup.Mvc.Mappers;

namespace Microsoft.Extensions.DependencyInjection.Extensions
{
    public static class ServiceExtensions
    {
        public static IMvcBuilder AddThreaxUserLookup(this IMvcBuilder builder, Action<UserLookupOptions> configure)
        {
            builder.Services.AddScoped<AppMapper>(s => new AppMapper());

            var options = new UserLookupOptions();
            configure.Invoke(options);

            builder.Services.TryAddScoped(typeof(IUserSearchService), options.UserSearchServiceType);

            return builder;
        }

    }
}
