using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Threax.AspNetCore.UserBuilder
{
    /// <summary>
    /// A repository of user ids that are allowed access to an app.
    /// </summary>
    public interface IUsersRepository
    {
        /// <summary>
        /// Check to see if the given id is in the repository.
        /// </summary>
        /// <param name="id">The id to check.</param>
        /// <returns>True if the id is in the repo.</returns>
        Task<bool> HasUserId(Guid id);

        /// <summary>
        /// Get the roles associated with the given user id.
        /// </summary>
        /// <param name="id">The user id to lookup.</param>
        /// <returns>An enumerator over all the roles for this user.</returns>
        Task<IEnumerable<String>> GetUserRoles(Guid id);

        /// <summary>
        /// Determine if a user is in a role specified by this repository.
        /// </summary>
        /// <param name="id">The id of the user.</param>
        /// <param name="roles">The string names of the roles to check.</param>
        /// <returns>True if the user is in the role, false otherwise.</returns>
        Task<bool> IsUserInRoles(Guid id, IEnumerable<String> roles);

        /// <summary>
        /// Get the ids of the users in the given role.
        /// </summary>
        /// <param name="role">The role to lookup.</param>
        /// <returns>An enumerable over the user ids for the user in the specified role.</returns>
        Task<IEnumerable<Guid>> GetUserIdsInRole(String role);
    }
}
