using System;
using System.Collections.Generic;
using System.Text;

namespace Threax.AspNetCore.UserBuilder.Entities
{
    /// <summary>
    /// This class represents the roles that the current user acting as an admin has. This can be none of these
    /// roles, which means they arent really an admin.
    /// </summary>
    public class AdminRoles
    {
        public bool EditRoles { get; set; }

        public bool SuperAdmin { get; set; }
    }
}
