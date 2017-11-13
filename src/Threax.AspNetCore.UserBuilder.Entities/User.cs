using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Threax.AspNetCore.UserBuilder.Entities
{
    /// <summary>
    /// A user for the system.
    /// </summary>
    public class User
    {
        /// <summary>
        /// The id of the user.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// A name for the user. Just for reference and ui display.
        /// </summary>
        [MaxLength(450)]
        public String Name { get; set; }

        /// <summary>
        /// The role mappings for the user.
        /// </summary>
        public List<UserToRole> Roles { get; set; }
    }
}
