using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Threax.AspNetCore.UserBuilder.Entities
{
    class IdentityAdminRoleProvider : IAdminRoleProvider
    {
        private IHttpContextAccessor contextAccessor;

        public IdentityAdminRoleProvider(IHttpContextAccessor contextAccessor)
        {
            this.contextAccessor = contextAccessor;
        }

        public AdminRoles GetAdminRoles()
        {
            var user = contextAccessor.HttpContext.User;
            return new AdminRoles()
            {
                EditRoles = user.IsInRole(AuthorizationAdminRoles.EditRoles),
                SuperAdmin = user.IsInRole(AuthorizationAdminRoles.SuperAdmin)
            };
        }
    }
}
