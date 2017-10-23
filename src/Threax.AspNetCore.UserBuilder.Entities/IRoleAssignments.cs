using System;
using System.Collections.Generic;
using System.Text;

namespace Threax.AspNetCore.UserBuilder.Entities
{
    public interface IRoleAssignments
    {
        /// <summary>
        /// The id of the user these roles are for.
        /// </summary>
        Guid UserId { get; set; }

        /// <summary>
        /// The name of the user for display purposes.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Get a list of all the roles and their values.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Tuple<String, bool>> GetRoleValues();

        /// <summary>
        /// Set the role values. Any roles in currentRoles are roles the user has.
        /// </summary>
        /// <param name="currentRoles">The current roles for the user.</param>
        void SetRoleValues(IEnumerable<String> currentRoles);

        /// <summary>
        /// The role assignment for the EditRoles permission.
        /// </summary>
        bool EditRoles { get; set; }

        /// <summary>
        /// The role assignment for the SuperAdmin permission
        /// </summary>
        bool SuperAdmin { get; set; }
    }
}
