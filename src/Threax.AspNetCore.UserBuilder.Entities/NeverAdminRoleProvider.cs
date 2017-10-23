using System;
using System.Collections.Generic;
using System.Text;

namespace Threax.AspNetCore.UserBuilder.Entities
{
    class NeverAdminRoleProvider : IAdminRoleProvider
    {
        public AdminRoles GetAdminRoles()
        {
            return new AdminRoles();
        }
    }
}
