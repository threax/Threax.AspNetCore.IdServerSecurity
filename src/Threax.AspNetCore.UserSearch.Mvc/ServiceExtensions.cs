using Threax.AspNetCore.UserSearchMvc.Services;

namespace Microsoft.Extensions.DependencyInjection.Extensions
{
    public static class ServiceExtensions
    {
        public static IMvcBuilder AddUserSearchMvc(this IMvcBuilder builder)
        {
            builder.Services.TryAddScoped<IUserSearchService, UserSearchService>();

            return builder;
        }

    }
}
