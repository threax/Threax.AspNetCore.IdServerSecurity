using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Threax.AspNetCore.UserBuilder.Entities
{
    public class UserEntityRepository : IUserEntityRepository
    {
        public UsersDbContext authorizedUsersDb;

        public UserEntityRepository(UsersDbContext authorizedUsersDb)
        {
            this.authorizedUsersDb = authorizedUsersDb;
        }

        /// <summary>
        /// Check to see if the given user id exists in the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> HasUserId(Guid id)
        {
            return await authorizedUsersDb.Users.AnyAsync(u => u.UserId == id);
        }

        /// <summary>
        /// Get the roles for the given user id in the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetUserRoles(Guid id)
        {
            return await authorizedUsersDb.UserRoles.Where(r => r.UserId == id).Select(r => r.Role.Name).ToListAsync();
        }

        /// <summary>
        /// Get the user specified by id. Will return null if the user does not exist.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<User> GetUser(Guid id)
        {
            return await authorizedUsersDb.Users.Where(r => r.UserId == id).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get the users associated with the list of ids.
        /// </summary>
        /// <param name="ids">The ids to lookup.</param>
        /// <returns>A list of the users with the specified id.</returns>
        public async Task<List<User>> GetUsers(IEnumerable<Guid> ids)
        {
            return await authorizedUsersDb.Users.Where(r => ids.Contains(r.UserId)).ToListAsync();
        }

        /// <summary>
        /// Get all users in this repository.
        /// </summary>
        /// <returns></returns>
        public IQueryable<User> GetUsers()
        {
            return authorizedUsersDb.Users;
        }

        /// <summary>
        /// Delete the user and all of their role assignments.
        /// </summary>
        /// <param name="userId">The user id to delete.</param>
        /// <returns></returns>
        public async Task DeleteUser(Guid userId)
        {
            var user = authorizedUsersDb.Users.Where(i => i.UserId == userId);
            var roles = authorizedUsersDb.UserRoles.Where(i => i.UserId == userId);

            authorizedUsersDb.UserRoles.RemoveRange(roles);
            authorizedUsersDb.Users.RemoveRange(user);

            await authorizedUsersDb.SaveChangesAsync();
        }

        /// <summary>
        /// Add a user with the given userid to the database with the given roles, the roles are only added
        /// to any roles already on the user account never removed. This makes it safe to call even if the user
        /// already exists.
        /// </summary>
        /// <param name="context">The context to update.</param>
        /// <param name="userId">The user id to add to the database.</param>
        /// <param name="roles">The roles.</param>
        public async Task AddUser(User user, IEnumerable<String> roles)
        {
            if (!await authorizedUsersDb.Users.AnyAsync(u => u.UserId == user.UserId))
            {
                authorizedUsersDb.Users.Add(user);
            }

            var userCurrentRoles = await authorizedUsersDb.UserRoles.Where(i => i.UserId == user.UserId).Select(i => i.RoleId).ToListAsync();

            var selectedRoleIds = authorizedUsersDb.Roles.Where(r => roles.Contains(r.Name) && !userCurrentRoles.Contains(r.RoleId));
            authorizedUsersDb.UserRoles.AddRange(selectedRoleIds.Select(i =>
                new UserToRole()
                {
                    UserId = user.UserId,
                    RoleId = i.RoleId
                }));

            await authorizedUsersDb.SaveChangesAsync();
        }

        /// <summary>
        /// Update a user with the given user id in the database, this will update the name if the new name is not null.
        /// It will also reassign all the roles, removing any that the user does not have permisssion for and adding any
        /// that they do.
        /// </summary>
        /// <param name="context">The context to update.</param>
        /// <param name="userId">The user id to add to the database.</param>
        /// <param name="roles">The roles.</param>
        public async Task UpdateUser(User user, IEnumerable<Tuple<String, bool>> roles)
        {
            //Make sure entity exists
            var userEntity = await authorizedUsersDb.Users.FirstOrDefaultAsync(u => u.UserId == user.UserId);

            if (userEntity == null)
            {
                authorizedUsersDb.Users.Add(user);
            }
            else if (user.Name != null)
            {
                //If it exists and we are setting a new name, update the entity name
                userEntity.Name = user.Name;
            }

            //Update roles
            var userCurrentRoles = await authorizedUsersDb.UserRoles.Where(i => i.UserId == user.UserId).Select(i => i.Role).ToListAsync();
            var rolesToAdd = new List<String>();
            var rolesToRemove = new List<Role>();
            foreach (var role in roles)
            {
                var roleEntity = userCurrentRoles.FirstOrDefault(r => r.Name == role.Item1);
                bool hasRole = roleEntity != null;
                if (role.Item2)
                {
                    if (!hasRole)
                    {
                        rolesToAdd.Add(role.Item1);
                    }
                }
                else
                {
                    if (hasRole)
                    {
                        rolesToRemove.Add(roleEntity);
                    }
                }
            }

            authorizedUsersDb.UserRoles.RemoveRange(rolesToRemove.Select(i =>
                new UserToRole()
                {
                    UserId = user.UserId,
                    RoleId = i.RoleId
                }));

            var selectedRoleIds = authorizedUsersDb.Roles.Where(r => rolesToAdd.Any(n => n == r.Name));
            authorizedUsersDb.UserRoles.AddRange(selectedRoleIds.Select(i =>
                new UserToRole()
                {
                    UserId = user.UserId,
                    RoleId = i.RoleId
                }));

            await authorizedUsersDb.SaveChangesAsync();
        }
    }
}
