using System;
using System.Collections.Generic;
using System.Text;

namespace Threax.AspNetCore.UserBuilder.Entities
{
    public class AuthorizationAdminRoles
    {
        /// <summary>
        /// Permission to edit the roles of other users unless they are super admins.
        /// </summary>
        public const String EditRoles = "EditRoles";

        /// <summary>
        /// Permission to edit the roles of all users when combined with the EditRoles permission. Also enables
        /// the user to delete other users.
        /// </summary>
        public const String SuperAdmin = "SuperAdmin";

        public static IEnumerable<String> All()
        {
            yield return EditRoles;
            yield return SuperAdmin;
        }
    }
}
