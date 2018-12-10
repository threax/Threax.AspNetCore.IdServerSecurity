using AutoMapper;
using System;
using System.Reflection;
using Threax.AspNetCore.UserLookup;
using Threax.AspNetCore.UserLookup.Mvc.Mappers;
using Threax.AspNetCore.UserLookup.Mvc.InputModels;
using Threax.AspNetCore.UserLookup.Mvc.ViewModels;

namespace Microsoft.Extensions.DependencyInjection.Extensions
{
    public static class ServiceExtensions
    {
        public static IMvcBuilder AddThreaxUserLookup(this IMvcBuilder builder, Action<UserLookupOptions> configure)
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                //Auto find profile classes
                cfg.AddProfiles(typeof(AppMapper).GetTypeInfo().Assembly);
            });

            builder.Services.AddScoped<AppMapper>(s => new AppMapper(mapperConfig.CreateMapper(s.GetRequiredService)));

            var options = new UserLookupOptions();
            configure.Invoke(options);

            builder.Services.TryAddScoped(typeof(IUserSearchService), options.UserSearchServiceType);

            return builder;
        }

    }
}
