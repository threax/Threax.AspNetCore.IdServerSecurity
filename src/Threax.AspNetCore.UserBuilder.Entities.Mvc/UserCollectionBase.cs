using System;
using System.Collections.Generic;
using System.Text;
using Threax.AspNetCore.Halcyon.Ext;

namespace Threax.AspNetCore.UserBuilder.Entities.Mvc
{
    public class UserCollectionBase<TRoleAssignments> : PagedCollectionView<TRoleAssignments>
        where TRoleAssignments : IRoleAssignments, new()
    {
        private RolesQuery rolesQuery;

        public UserCollectionBase(RolesQuery query, int total, IEnumerable<TRoleAssignments> items) : base(query, total, items)
        {
            this.rolesQuery = query;
        }

        protected override void AddCustomQuery(string rel, QueryStringBuilder query)
        {
            base.AddCustomQuery(rel, query);
            if(rolesQuery.UserId != null && rolesQuery.UserId.Count > 0)
            {
                foreach(var id in rolesQuery.UserId)
                {
                    query.AppendItem("userId", id.ToString());
                }
            }
        }
    }
}
