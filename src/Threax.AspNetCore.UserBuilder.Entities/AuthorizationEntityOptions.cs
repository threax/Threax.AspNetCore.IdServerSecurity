using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Threax.AspNetCore.UserBuilder.Entities
{
    public class AuthorizationEntityOptions : IDbContextOptionsExtension
    {
        public String UserTableName { get; set; } = "spc.auth.Users";

        public String RoleTableName { get; set; } = "spc.auth.Roles";

        public String UserToRoleTableName { get; set; } = "spc.auth.UsersToRoles";

        public void ApplyServices(IServiceCollection services)
        {
            
        }
    }
}
