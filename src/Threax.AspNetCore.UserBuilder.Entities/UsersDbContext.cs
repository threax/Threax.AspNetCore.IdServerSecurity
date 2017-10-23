using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Threax.AspNetCore.UserBuilder.Entities
{
    /// <summary>
    /// A simple subclass that uses User and Role as the generic type arguments.
    /// </summary>
    public abstract class UsersDbContext : DbContext
    {
        AuthorizationEntityOptions entityOptions;

        /// <summary>
        /// Constructor for weakly typed options.
        /// </summary>
        /// <param name="options">The strong type options.</param>
        public UsersDbContext(DbContextOptions options)
            : this(options, new AuthorizationEntityOptions())
        {

        }

        /// <summary>
        /// Constructor for weakly typed options.
        /// </summary>
        public UsersDbContext(DbContextOptions options, AuthorizationEntityOptions entityOptions)
            : base(options)
        {
            this.entityOptions = entityOptions;
        }

        /// <summary>
        /// The authorized users for this db context.
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// The authorized users for this db context.
        /// </summary>
        public DbSet<Role> Roles { get; set; }

        /// <summary>
        /// The roles individual users belong to.
        /// </summary>
        public DbSet<UserToRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserToRole>().HasKey(x => new { x.UserId, x.RoleId });
            modelBuilder.Entity<User>().ToTable(entityOptions.UserTableName);
            modelBuilder.Entity<Role>().ToTable(entityOptions.RoleTableName);
            modelBuilder.Entity<UserToRole>().ToTable(entityOptions.UserToRoleTableName);

            base.OnModelCreating(modelBuilder);
        }
    }
}
