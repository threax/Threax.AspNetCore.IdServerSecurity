using Halcyon.HAL.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        [UiOrder(int.MinValue, 0)]
        public String Name { get; set; }

        [UiSearch]
        [UiOrder(int.MaxValue - 1, 0)]
        public bool EditRoles { get; set; }

        [UiSearch]
        [UiOrder(int.MaxValue, 0)]
        public bool SuperAdmin { get; set; }

        public virtual IQueryable<User> Create(IQueryable<User> query)
        {
            if(Name != null)
            {
                query = query.Where(i => i.Name.Contains(Name));
            }

            var roleFilter = this.GetType().GetTypeInfo().GetProperties()
                .Where(i => i.PropertyType == typeof(bool))
                .Select(i => Tuple.Create(i.Name, (bool)i.GetValue(this))).Where(i => i.Item2).Select(i => i.Item1).ToList();

            if (roleFilter.Any())
            {
                query = query.Where(i => i.Roles.Select(j => j.Role.Name).Intersect(roleFilter).Any());
            }

            query = query.OrderBy(i => i.Name);
            return query;
        }
    }
}
