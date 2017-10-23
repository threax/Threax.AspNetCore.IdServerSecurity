using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Threax.AspNetCore.UserBuilder.Entities
{
    /// <summary>
    /// A role in the system.
    /// </summary>
    public class Role
    {
        /// <summary>
        /// The id of the role.
        /// </summary>
        public Guid RoleId { get; set; }

        /// <summary>
        /// The name of the role to assign.
        /// </summary>
        [MaxLength(450)]
        public String Name { get; set; }
    }
}
