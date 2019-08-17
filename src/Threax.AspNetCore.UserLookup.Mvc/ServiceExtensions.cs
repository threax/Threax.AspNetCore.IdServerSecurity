using AutoMapper;
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
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                //Auto find profile classes
                var profiles = typeof(ServiceExtensions).GetTypeInfo().Assembly.GetTypes()
                    .Where(t => t.IsSubclassOf(typeof(Profile)))
                    .Select(i => Activator.CreateInstance(i) as Profile)
                    .ToList();

                cfg.AddProfiles(profiles);
            });

            builder.Services.AddScoped<AppMapper>(s => new AppMapper(mapperConfig.CreateMapper(s.GetRequiredService)));

            var options = new UserLookupOptions();
            configure.Invoke(options);

            builder.Services.TryAddScoped(typeof(IUserSearchService), options.UserSearchServiceType);

            return builder;
        }

    }
}
