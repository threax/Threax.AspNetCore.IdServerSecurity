using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Threax.AspNetCore.UserBuilder.Entities.Mvc
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Add the authorization entity mvc services.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddAuthorizationEntityMvc(this IServiceCollection services)
        {
            services.TryAddScoped<IAdminRoleProvider, IdentityAdminRoleProvider>();

            return services;
        }
    }
}
