using Halcyon.HAL.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Threax.AspNetCore.Halcyon.Ext;

namespace Threax.AspNetCore.UserBuilder.Entities.Mvc
{
    [HalModel]
    public class RolesQuery : PagedCollectionQuery, IRoleQuery
    {
        /// <summary>
        /// The guid for the user, this is used to look up the user.
        /// </summary>
        public List<Guid> UserId { get; set; } = new List<Guid>();

        /// <summary>
        /// A name for the user. Used only as a reference, will be added to the result if the user is not found.
        /// </summary>
        public String Name { get; set; }

        public virtual IQueryable<User> Create(IQueryable<User> query)
        {
            return query;
        }
    }
}
