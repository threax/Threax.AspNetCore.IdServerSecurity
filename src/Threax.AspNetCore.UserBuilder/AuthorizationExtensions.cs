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
        /// <param name="additionalPolicies">Any additional policies you want in the user builder chain.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddUserBuilderForUserWhitelistWithRoles(this IServiceCollection services, IUserBuilder additionalPolicies = null)
        {
            services.AddScoped(s =>
            {
                return CreateUserBuilderForUserWhitelistWithRoles(additionalPolicies, s);
            });

            return services;
        }

        /// <summary>
        /// Make the IUserBuilder policy one that needs whitelisted users and assignes them roles.
        /// Be sure you have added an IUserBuilder to the services if using this mode.
        /// </summary>
        /// <param name="services">The service collection to add to.</param>
        /// <param name="additionalPoliciesCallback">A function to build additional users policies with access to the services.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddUserBuilderForUserWhitelistWithRoles(this IServiceCollection services, Func<AdditionalPoliciesCallbackArgs, IUserBuilder> additionalPoliciesCallback)
        {
            services.AddScoped(s =>
            {
                return CreateUserBuilderForUserWhitelistWithRoles(GetAdditionalPolicies(s, additionalPoliciesCallback), s);
            });

            return services;
        }

        private static IUserBuilder CreateUserBuilderForUserWhitelistWithRoles(IUserBuilder additionalPolicies, IServiceProvider s)
        {
            var usersRepo = s.GetRequiredService<IUsersRepository>();
            var loggerFactory = s.GetRequiredService<ILoggerFactory>();
            var log = loggerFactory.CreateLogger("Authentication");
            return new UserWhitelistAuthorizer(usersRepo, log, new UserRoleBuilder(usersRepo, log, additionalPolicies));
        }

        /// <summary>
        /// Make the IUserBuilder policy one that needs whitelisted users with no roles.
        /// Be sure you have added an IUserBuilder to the services if using this mode.
        /// </summary>
        /// <param name="services">The service collection to add to.</param>
        /// <param name="additionalPolicies">Any additional policies you want in the user builder chain.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddUserBuilderForUserWhitelist(this IServiceCollection services, IUserBuilder additionalPolicies = null)
        {
            services.AddScoped(s =>
            {
                return CreateUserBuilderForUserWhitelist(additionalPolicies, s);
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
        public static IServiceCollection AddUserBuilderForUserWhitelist(this IServiceCollection services, Func<AdditionalPoliciesCallbackArgs, IUserBuilder> additionalPoliciesCallback)
        {
            services.AddScoped(s =>
            {
                return CreateUserBuilderForUserWhitelist(GetAdditionalPolicies(s, additionalPoliciesCallback), s);
            });

            return services;
        }

        private static IUserBuilder CreateUserBuilderForUserWhitelist(IUserBuilder additionalPolicies, IServiceProvider s)
        {
            var usersRepo = s.GetRequiredService<IUsersRepository>();
            var loggerFactory = s.GetRequiredService<ILoggerFactory>();
            var log = loggerFactory.CreateLogger("Authentication");
            return new UserWhitelistAuthorizer(usersRepo, log, additionalPolicies);
        }

        /// <summary>
        /// Make the IUserBuilder policy one that allows all users and assignes some of them additional roles.
        /// Be sure you have added an IUsersRepository to the services if using this mode.
        /// </summary>
        /// <param name="services">The service collection to add to.</param>
        /// <param name="additionalPolicies">Any additional policies you want in the user builder chain.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddUserBuilderForAllUsersWithRoles(this IServiceCollection services, IUserBuilder additionalPolicies = null)
        {
            services.AddScoped(s =>
            {
                return CreateUserBuilderForAllUsersWithRoles(additionalPolicies, s);
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
        public static IServiceCollection AddUserBuilderForAllUsersWithRoles(this IServiceCollection services, Func<AdditionalPoliciesCallbackArgs, IUserBuilder> additionalPoliciesCallback)
        {
            services.AddScoped<IUserBuilder>(s =>
            {
                return CreateUserBuilderForAllUsersWithRoles(GetAdditionalPolicies(s, additionalPoliciesCallback), s);
            });

            return services;
        }

        private static IUserBuilder CreateUserBuilderForAllUsersWithRoles(IUserBuilder additionalPolicies, IServiceProvider s)
        {
            var usersRepo = s.GetRequiredService<IUsersRepository>();
            var loggerFactory = s.GetRequiredService<ILoggerFactory>();
            var log = loggerFactory.CreateLogger("Authentication");
            return new UserRoleBuilder(usersRepo, log, additionalPolicies);
        }

        /// <summary>
        /// Make the IUserBuilder policy one that allows all users through. You can add additional policies to make this do something
        /// if you need to.
        /// </summary>
        /// <param name="services">The service collection to add to.</param>
        /// <param name="additionalPolicies">Any additional policies you want in the user builder chain.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddUserBuilderForAnybody(this IServiceCollection services, IUserBuilder additionalPolicies = null)
        {
            if(additionalPolicies != null)
            {
                //If we have policies add 
                services.AddScoped<IUserBuilder>(s =>
                {
                    return additionalPolicies;
                });
            }
            else
            {
                //Otherwise shim the authorizer
                services.AddScoped<IUserBuilder>(s =>
                {
                    return new AllAccessAuthorizer();
                });
            }

            return services;
        }

        /// <summary>
        /// Make the IUserBuilder policy one that allows all users through. You can add additional policies to make this do something
        /// if you need to.
        /// </summary>
        /// <param name="services">The service collection to add to.</param>
        /// <param name="additionalPoliciesCallback">A function to build additional users policies with access to the services.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddUserBuilderForAnybody(this IServiceCollection services, Func<AdditionalPoliciesCallbackArgs, IUserBuilder> additionalPoliciesCallback)
        {
            if (additionalPoliciesCallback != null)
            {
                //If we have policies add 
                services.AddScoped<IUserBuilder>(s =>
                {
                    var policy = GetAdditionalPolicies(s, additionalPoliciesCallback);
                    if(policy == null)
                    {
                        return new AllAccessAuthorizer();
                    }
                    return policy;
                });
            }
            else
            {
                //Otherwise shim the authorizer
                services.AddScoped<IUserBuilder>(s =>
                {
                    return new AllAccessAuthorizer();
                });
            }

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
