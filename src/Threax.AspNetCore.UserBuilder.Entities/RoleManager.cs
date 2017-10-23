using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Threax.AspNetCore.UserBuilder.Entities
{
    /// <summary>
    /// The class handles roles coming from the entity framework.
    /// </summary>
    public class RoleManager : IRoleManager
    {
        private IUserEntityRepository userRepo;
        private IAdminRoleProvider adminRoles;

        public RoleManager(IUserEntityRepository userRepo, IAdminRoleProvider adminRoles)
        {
            this.userRepo = userRepo;
            this.adminRoles = adminRoles;
        }

        /// <summary>
        /// Get the roles for the specified user id. If that user is in the database you will get its roles, otherwise you will
        /// get a new TRoleAssignmentType for the user with no roles in it.
        /// </summary>
        /// <typeparam name="TRoleAssignmentType">The role type to map the roles back to.</typeparam>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<TRoleAssignmentType> GetRoles<TRoleAssignmentType>(Guid userId, String userDescriptiveName)
            where TRoleAssignmentType : IRoleAssignments, new()
        {
            var user = await userRepo.GetUser(userId);
            if(user == null)
            {
                user = new User()
                {
                    UserId = userId,
                    Name = userDescriptiveName
                };
            }
            return await GetUserRoles<TRoleAssignmentType>(user);
        }

        /// <summary>
        /// Get the roles for the specified user id. If that user is in the database you will get its roles, otherwise you will
        /// get a new TRoleAssignmentType for the user with no roles in it.
        /// </summary>
        /// <typeparam name="TRoleAssignmentType">The role type to map the roles back to.</typeparam>
        /// <param name="userIds"></param>
        /// <returns></returns>
        public async Task<List<TRoleAssignmentType>> GetRoles<TRoleAssignmentType>(IEnumerable<Guid> userIds)
            where TRoleAssignmentType : IRoleAssignments, new()
        {
            var users = await userRepo.GetUsers(userIds);
            var results = new List<TRoleAssignmentType>();
            //Fill out any users that were not found
            foreach(var id in userIds)
            {
                var user = users.FirstOrDefault(i => i.UserId == id);
                if (user == null)
                {
                    user = new User()
                    {
                        UserId = id,
                    };
                    users.Add(user);
                }
                results.Add(await GetUserRoles<TRoleAssignmentType>(user));
            }
            return results;
        }

        /// <summary>
        /// Set the role assignments for a user. Will not happen if the calling user does not have permission.
        /// </summary>
        /// <param name="roles">The roles to set.</param>
        /// <returns></returns>
        public async Task SetRoles(IRoleAssignments roles)
        {
            var admin = adminRoles.GetAdminRoles();

            if (!admin.EditRoles)
            {
                throw new UnauthorizedAccessException("User not allowed to edit roles.");
            }

            var targetUser = new User()
            {
                UserId = roles.UserId,
                Name = roles.Name,
            };

            //Determine if we are changing editroles or superadmin, must be a superadmin user to do this
            var targetRoles = await userRepo.GetUserRoles(targetUser.UserId);
            var targetIsRoleEditor = targetRoles.Contains(AuthorizationAdminRoles.EditRoles);
            var targetIsSuperAdmin = targetRoles.Contains(AuthorizationAdminRoles.SuperAdmin);

            if ((roles.EditRoles != targetIsRoleEditor) && !admin.SuperAdmin)
            {
                throw new UnauthorizedAccessException("User not allowed to change EditRoles permissions on another user, must be a Super Admin.");
            }

            if ((roles.SuperAdmin != targetIsSuperAdmin) && !admin.SuperAdmin)
            {
                throw new UnauthorizedAccessException("User not allowed to change SuperAdmin permissions on another user, must be a Super Admin.");
            }

            await userRepo.UpdateUser(targetUser, roles.GetRoleValues());
        }

        /// <summary>
        /// Get the users in the application.
        /// </summary>
        /// <typeparam name="TRoleAssignmentType">The type to map the user roles back onto.</typeparam>
        /// <param name="offset">The paged offset into the user list.</param>
        /// <param name="limit">The limit of the number of users to take.</param>
        /// <returns>The role assignments for the specified users.</returns>
        public async Task<SystemUserRoleInfo<TRoleAssignmentType>> GetUsers<TRoleAssignmentType>(int offset, int limit)
            where TRoleAssignmentType : IRoleAssignments, new()
        {
            var users = userRepo.GetUsers();

            var total = await users.CountAsync();
            var skip = offset * limit;
            if(skip > total)
            {
                skip = total;
            }
            if(skip + limit > total)
            {
                total -= skip;
            }

            var results = await users.Skip(skip).Take(limit).ToListAsync();
            return new SystemUserRoleInfo<TRoleAssignmentType>()
            {
                Total = total,
                Results = results.Select(i =>
                {
                    var e = GetUserRoles<TRoleAssignmentType>(i);
                    e.Wait();
                    return e.Result;
                }),
            };
        }

        /// <summary>
        /// Delete the user from the application. Will only happen if the calling user has permission.
        /// </summary>
        /// <param name="userId">The user id to delete.</param>
        /// <returns></returns>
        public Task DeleteUser(Guid userId)
        {
            var admin = adminRoles.GetAdminRoles();
            if (admin.SuperAdmin)
            {
                return userRepo.DeleteUser(userId);
            }
            else
            {
                throw new InvalidOperationException("User not allowed to delete another user, must be a Super Admin.");
            }
        }

        private async Task<TRoleAssignmentType> GetUserRoles<TRoleAssignmentType>(User user) where TRoleAssignmentType : IRoleAssignments, new()
        {
            var roles = userRepo.GetUserRoles(user.UserId);
            var assignments = new TRoleAssignmentType();
            assignments.UserId = user.UserId;
            if (user != null)
            {
                assignments.Name = user.Name;
            }
            assignments.SetRoleValues(await roles);
            return assignments;
        }
    }
}
