using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Threax.AspNetCore.UserBuilder.Entities
{
    /// <summary>
    /// Additional options for the authorization database.
    /// </summary>
    public class AuthorizationDatabaseOptions
    {
        /// <summary>
        /// Called after UseSqlServer is called on the DbContextOptionsBuilder, you can further customize the database here, but
        /// you do not need to call UseSqlServer as it will already be called. If you want to modify the options for UseSqlServer
        /// see SqlServerOptionsAction.
        /// </summary>
        public Action<DbContextOptionsBuilder> OptionsAction { get; set; }

        /// <summary>
        /// Called after the MigrationsAssembly is set on the SqlServerDbContextOptionsBuilder. You can further customize the sql
        /// server options by using this function, but you do not need to set the migrations assembly as that will already be set.
        /// </summary>
        public Action<SqlServerDbContextOptionsBuilder> SqlServerOptionsAction { get; set; }
    }
}
