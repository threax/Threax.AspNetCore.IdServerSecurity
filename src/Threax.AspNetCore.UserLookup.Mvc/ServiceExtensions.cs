using AutoMapper;
using System.Reflection;
using Threax.AspNetCore.UserLookup;
using Threax.AspNetCore.UserLookup.Mvc.Mappers;
using Threax.AspNetCore.UserSearchMvc.InputModels;
using Threax.AspNetCore.UserSearchMvc.ViewModels;

namespace Microsoft.Extensions.DependencyInjection.Extensions
{
    public static class ServiceExtensions
    {
        public static IMvcBuilder AddUserSearchMvc(this IMvcBuilder builder)
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                //Auto find profile classes
                cfg.AddProfiles(typeof(AppMapper).GetTypeInfo().Assembly);
            });

            builder.Services.AddScoped<AppMapper>(s => new AppMapper(mapperConfig.CreateMapper(s.GetRequiredService)));

            return builder;
        }

    }
}
