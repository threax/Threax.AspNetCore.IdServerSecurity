using Halcyon.HAL.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Threax.AspNetCore.Halcyon.Ext;
using Threax.AspNetCore.Models;

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
        /// Search by user name.
        /// </summary>
        [UiSearch]
        public String Name { get; set; }

        public virtual IQueryable<User> Create(IQueryable<User> query)
        {
            if(Name != null)
            {
                query = query.Where(i => i.Name.Contains(Name));
            }

            query = query.OrderBy(i => i.Name);
            return query;
        }
    }
}
