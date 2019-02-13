using System;
using System.Collections.Generic;
using System.Text;
using Threax.AspNetCore.Halcyon.Ext;

namespace Threax.AspNetCore.UserBuilder.Entities.Mvc
{
    public class UserCollectionBase<TRoleAssignments, TRoleQuery> : PagedCollectionViewWithQuery<TRoleAssignments, TRoleQuery>
        where TRoleAssignments : IRoleAssignments, new()
        where TRoleQuery : RolesQuery
    {
        private TRoleQuery rolesQuery;

        public UserCollectionBase(TRoleQuery query, int total, IEnumerable<TRoleAssignments> items) : base(query, total, items)
        {
            this.rolesQuery = query;
        }
    }

    public class UserCollectionBase<TRoleAssignments> : UserCollectionBase<TRoleAssignments, RolesQuery>
        where TRoleAssignments : IRoleAssignments, new()
    {
        public UserCollectionBase(RolesQuery query, int total, IEnumerable<TRoleAssignments> items) : base(query, total, items)
        {
        }
    }
}
