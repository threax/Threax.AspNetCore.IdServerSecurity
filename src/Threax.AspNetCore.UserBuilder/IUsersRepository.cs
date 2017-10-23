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
    }
}
