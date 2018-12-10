using System;
using Threax.AspNetCore.UserSearchMvc.Services;

namespace Microsoft.Extensions.DependencyInjection.Extensions
{
    public static class ServiceExtensions
    {
        [Obsolete("This Library is obsolete. Use Threax.AspNetCore.UserLookup.Mvc instead.")]
        public static IMvcBuilder AddUserSearchMvc(this IMvcBuilder builder)
        {
            builder.Services.TryAddScoped<IUserSearchService, UserSearchService>();

            return builder;
        }

    }
}
