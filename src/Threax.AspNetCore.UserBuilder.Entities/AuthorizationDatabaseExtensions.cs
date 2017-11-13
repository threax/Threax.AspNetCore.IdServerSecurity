using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Threax.AspNetCore.UserBuilder;

namespace Threax.AspNetCore.UserBuilder.Entities
{
    public static class AuthorizationDatabaseExtensions
    {
        /// <summary>
        /// Add the authorization database.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="migrationsAssembly">The assembly to apply migrations to.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddAuthorizationDatabase<TSubclassType>(this IServiceCollection services, String connectionString, Assembly migrationsAssembly, AuthorizationDatabaseOptions authDbOptions = null)
            where TSubclassType : UsersDbContext
        {
            if(authDbOptions == null)
            {
                authDbOptions = new AuthorizationDatabaseOptions();
            }

            services.AddDbContextPool<TSubclassType>(o =>
            {
                o.UseSqlServer(connectionString, options =>
                {
                    options.MigrationsAssembly(migrationsAssembly.GetName().Name);
                    authDbOptions.SqlServerOptionsAction?.Invoke(options);
                });
                authDbOptions.OptionsAction?.Invoke(o);
            });

            services.TryAddScoped<UsersDbContext, TSubclassType>(); //Make the authorization service aware of our database subclass.
            services.TryAddScoped<IUsersRepository>(s => s.GetRequiredService<IUserEntityRepository>());
            services.TryAddScoped<IUserEntityRepository, UserEntityRepository>();
            services.TryAddScoped<IRoleManager, RoleManager>();
            services.TryAddScoped<IAdminRoleProvider, IdentityAdminRoleProvider>();

            return services;
        }

        /// <summary>
        /// Apply any migrations and update the roles in the database to match the passed in collection.
        /// If you need to migrate and upgrade directly use this extension method, otherwise call InitializeAuthorizationDatabase
        /// during Configure, which is the reccomended approach.
        /// </summary>
        /// <param name="context">The context to update.</param>
        /// <param name="roles">The roles.</param>
        public static async Task SeedAuthorizationDatabase(this UsersDbContext context, IEnumerable<String> roles)
        {
            var dbRoles = context.Roles;
            var rolesToAdd = roles.Where(r => !dbRoles.Any(dbr => r == dbr.Name));

            context.Roles.AddRange(rolesToAdd.Select(r => new Entities.Role()
            {
                Name = r
            }));

            if(!context.Roles.Any(i => i.Name == AuthorizationAdminRoles.EditRoles))
            {
                context.Roles.Add(new Role()
                {
                    Name = AuthorizationAdminRoles.EditRoles,
                });
            }

            if (!context.Roles.Any(i => i.Name == AuthorizationAdminRoles.SuperAdmin))
            {
                context.Roles.Add(new Role()
                {
                    Name = AuthorizationAdminRoles.SuperAdmin,
                });
            }

            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Add all users with the given userids to the database with the given roles, the roles are only added
        /// to any roles already on the user account never removed. This makes it safe to call even if the user
        /// already exists.
        /// </summary>
        /// <param name="scope">The service scope.</param>
        /// <param name="roles">The roles.</param>
        public static async Task AddUsers(this IUserEntityRepository repo, IEnumerable<User> users, IEnumerable<String> roles)
        {
            foreach (var user in users)
            {
                await repo.AddUser(user, roles);
            }
        }

        /// <summary>
        /// Add a user with the given userid to the database with the given roles, the roles are only added
        /// to any roles already on the user account never removed. This makes it safe to call even if the user
        /// already exists.
        /// </summary>
        /// <param name="scope">The service scope.</param>
        /// <param name="roles">The roles.</param>
        public static Task AddUser(this IUserEntityRepository repo, User user, IEnumerable<String> roles)
        {
            return repo.AddUser(user, roles);
        }

        /// <summary>
        /// Add an admin, this will include all the roles you pass in along with the roles in AuthorizationAdminRoles.
        /// This will make the users super admins.
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="user"></param>
        /// <param name="roles"></param>
        public static Task AddAdmin(this IUserEntityRepository repo, User user, IEnumerable<String> roles)
        {
            return AddUser(repo, user, roles.Concat(AuthorizationAdminRoles.All()));
        }

        /// <summary>
        /// Add multiple admins. See AddAdmin for more info.
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="users"></param>
        /// <param name="roles"></param>
        public static Task AddAdmins(this IUserEntityRepository repo, IEnumerable<User> users, IEnumerable<String> roles)
        {
            return AddUsers(repo, users, roles.Concat(AuthorizationAdminRoles.All()));
        }
    }
}
