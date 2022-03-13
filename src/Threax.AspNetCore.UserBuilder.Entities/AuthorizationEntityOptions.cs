using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Threax.AspNetCore.UserBuilder.Entities
{
    public class AuthorizationEntityOptions : IDbContextOptionsExtension
    { 
        public String UserTableName { get; set; } = "spc.auth.Users";

        public String RoleTableName { get; set; } = "spc.auth.Roles";

        public String UserToRoleTableName { get; set; } = "spc.auth.UsersToRoles";

        private ContextOptionsExtensionsInfo extensionInfo;

        public AuthorizationEntityOptions()
        {
            extensionInfo = new ContextOptionsExtensionsInfo(this);
        }

        public DbContextOptionsExtensionInfo Info => extensionInfo;

        public void ApplyServices(IServiceCollection services)
        {
            
        }
        public class ContextOptionsExtensionsInfo : DbContextOptionsExtensionInfo
        {
            public ContextOptionsExtensionsInfo(IDbContextOptionsExtension extension) : base(extension)
            {
            }

            public override bool IsDatabaseProvider => true;

            public override string LogFragment => "";

            public override int GetServiceProviderHashCode()
            {
                return 0;
            }

            public override void PopulateDebugInfo([NotNullAttribute] IDictionary<string, string> debugInfo)
            {

            }

            public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
            {
                //Not really sure if this should be true or false
                //Trying false for now since we are returning 0 for the hash code
                return false;
            }
        }

        public void Validate(IDbContextOptions options)
        {
            
        }
    }
}
