using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Threax.AspNetCore.UserBuilder.Entities
{
    public interface IRoleManager
    {
        /// <summary>
        /// Get the roles for a user.
        /// </summary>
        /// <typeparam name="TRoleAssignmentType">The type to map the roles onto.</typeparam>
        /// <param name="userId">The user id of the user.</param>
        /// <param name="userDescriptiveName">A name for the user if one has not been assigned yet. For record keeping only, not used to search.</param>
        /// <returns></returns>
        Task<TRoleAssignmentType> GetRoles<TRoleAssignmentType>(Guid userId, String userDescriptiveName) where TRoleAssignmentType : IRoleAssignments, new();

        /// <summary>
        /// Get the roles for multiple users.
        /// </summary>
        /// <typeparam name="TRoleAssignmentType">The type to map the roles onto.</typeparam>
        /// <param name="userIds">The user id of the users to look up.</param>
        /// <returns></returns>
        Task<List<TRoleAssignmentType>> GetRoles<TRoleAssignmentType>(IEnumerable<Guid> userIds) where TRoleAssignmentType : IRoleAssignments, new();

        /// <summary>
        /// Set the roles for a user.
        /// </summary>
        /// <param name="roles">The roles to set</param>
        /// <returns></returns>
        Task SetRoles(IRoleAssignments roles);

        /// <summary>
        /// Get the users in the application along with their roles.
        /// </summary>
        /// <typeparam name="TRoleAssignmentType">The role assignment type to assign to.</typeparam>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        Task<SystemUserRoleInfo<TRoleAssignmentType>> GetUsers<TRoleAssignmentType>(IRoleQuery query) where TRoleAssignmentType : IRoleAssignments, new();

        /// <summary>
        /// Delete the user with the specified user id from the application.
        /// </summary>
        /// <param name="userId">The id of the user to erase.</param>
        /// <returns></returns>
        Task DeleteUser(Guid userId);
    }
}