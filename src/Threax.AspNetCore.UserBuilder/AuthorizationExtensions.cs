using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Threax.AspNetCore.UserBuilder
{
    public static class AuthorizationExtensions
    {
        /// <summary>
        /// Make the IUserBuilder policy one that needs whitelisted users and assignes them roles.
        /// Be sure you have added an IUserBuilder to the services if using this mode.
        /// </summary>
        /// <param name="services">The service collection to add to.</param>
        /// <param name="additionalPoliciesCallback">A function to build additional users policies with access to the services.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddUserBuilderForUserWhitelistWithRoles(this IServiceCollection services, Action<UserBuilderOptions> optionsCallback = null)
        {
            var options = new UserBuilderOptions();
            optionsCallback?.Invoke(options);

            services.AddSingleton<IClaimCache>(s => new ClaimCache(options.CacheClaimTypes));

            services.AddScoped(s =>
            {
                var usersRepo = s.GetRequiredService<IUsersRepository>();
                var loggerFactory = s.GetRequiredService<ILoggerFactory>();
                var log = loggerFactory.CreateLogger("Authentication");
                IUserBuilder builder = GetAdditionalPolicies(s, options.ConfigureAddititionalPolicies);
                builder = new UserRoleBuilder(usersRepo, log, builder);
                builder = new UserWhitelistAuthorizer(usersRepo, log, builder);
                if (options.UseClaimsCache)
                {
                    builder = new CachedClaimsUserBuilder(s.GetRequiredService<IClaimCache>(), builder);
                }
                return builder;
            });

            return services;
        }

        /// <summary>
        /// Make the IUserBuilder policy one that needs whitelisted users with no roles.
        /// Be sure you have added an IUserBuilder to the services if using this mode.
        /// </summary>
        /// <param name="services">The service collection to add to.</param>
        /// <param name="additionalPoliciesCallback">A function to build additional users policies with access to the services.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddUserBuilderForUserWhitelist(this IServiceCollection services, Action<UserBuilderOptions> optionsCallback = null)
        {
            var options = new UserBuilderOptions();
            optionsCallback?.Invoke(options);

            services.AddSingleton<IClaimCache>(s => new ClaimCache(options.CacheClaimTypes));

            services.AddScoped(s =>
            {
                var usersRepo = s.GetRequiredService<IUsersRepository>();
                var loggerFactory = s.GetRequiredService<ILoggerFactory>();
                var log = loggerFactory.CreateLogger("Authentication");
                var builder = GetAdditionalPolicies(s, options.ConfigureAddititionalPolicies);
                builder =  new UserWhitelistAuthorizer(usersRepo, log, builder);
                if (options.UseClaimsCache)
                {
                    builder = new CachedClaimsUserBuilder(s.GetRequiredService<IClaimCache>(), builder);
                }
                return builder;
            });

            return services;
        }

        /// <summary>
        /// Make the IUserBuilder policy one that allows all users and assignes some of them additional roles.
        /// Be sure you have added an IUsersRepository to the services if using this mode.
        /// </summary>
        /// <param name="services">The service collection to add to.</param>
        /// <param name="additionalPoliciesCallback">A function to build additional users policies with access to the services.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddUserBuilderForAllUsersWithRoles(this IServiceCollection services, Action<UserBuilderOptions> optionsCallback = null)
        {
            var options = new UserBuilderOptions();
            optionsCallback?.Invoke(options);

            services.AddSingleton<IClaimCache>(s => new ClaimCache(options.CacheClaimTypes));

            services.AddScoped<IUserBuilder>(s =>
            {
                var usersRepo = s.GetRequiredService<IUsersRepository>();
                var loggerFactory = s.GetRequiredService<ILoggerFactory>();
                var log = loggerFactory.CreateLogger("Authentication");
                var builder = GetAdditionalPolicies(s, options.ConfigureAddititionalPolicies);
                builder = new UserRoleBuilder(usersRepo, log, builder);
                if (options.UseClaimsCache)
                {
                    builder = new CachedClaimsUserBuilder(s.GetRequiredService<IClaimCache>(), builder);
                }
                return builder;
            });

            return services;
        }

        /// <summary>
        /// Make the IUserBuilder policy one that allows all users through. You can add additional policies to make this do something
        /// if you need to.
        /// </summary>
        /// <param name="services">The service collection to add to.</param>
        /// <param name="additionalPoliciesCallback">A function to build additional users policies with access to the services.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddUserBuilderForAnybody(this IServiceCollection services, Action<UserBuilderOptions> optionsCallback = null)
        {
            var options = new UserBuilderOptions();
            optionsCallback?.Invoke(options);

            services.AddSingleton<IClaimCache>(s => new ClaimCache(options.CacheClaimTypes));

            //Otherwise shim the authorizer
            services.AddScoped<IUserBuilder>(s =>
            {
                var builder = GetAdditionalPolicies(s, options.ConfigureAddititionalPolicies);
                builder = new AllAccessAuthorizer(builder);
                if (options.UseClaimsCache)
                {
                    builder = new CachedClaimsUserBuilder(s.GetRequiredService<IClaimCache>(), builder);
                }
                return builder;
            });

            return services;
        }

        public static IUserBuilder GetAdditionalPolicies(IServiceProvider services, Func<AdditionalPoliciesCallbackArgs, IUserBuilder> additionalPoliciesCallback)
        {
            IUserBuilder additionalPolicies = null;
            if (additionalPoliciesCallback != null)
            {
                additionalPolicies = additionalPoliciesCallback(new AdditionalPoliciesCallbackArgs(services));
            }

            return additionalPolicies;
        }
    }
}
