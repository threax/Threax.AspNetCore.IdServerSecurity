using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Threax.AspNetCore.UserBuilder.Entities
{
    public class UserToRole
    {
        /// <summary>
        /// The id of the user.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// The user.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// The id of the role.
        /// </summary>
        public Guid RoleId { get; set; }

        /// <summary>
        /// The role.
        /// </summary>
        public Role Role { get; set; }
    }
}
