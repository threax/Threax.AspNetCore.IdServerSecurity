using System;
using System.Collections.Generic;
using System.Text;

namespace Threax.AspNetCore.UserBuilder.Entities
{
    /// <summary>
    /// This interface looks up the relevant admin roles on the current user calling role related functions.
    /// </summary>
    public interface IAdminRoleProvider
    {
        AdminRoles GetAdminRoles();
    }
}
