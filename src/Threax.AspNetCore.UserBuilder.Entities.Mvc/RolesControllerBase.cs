using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Threax.AspNetCore.AuthCore;
using Threax.AspNetCore.Halcyon.Ext;

namespace Threax.AspNetCore.UserBuilder.Entities.Mvc
{
    public class RolesControllerRels
    {
        public const String ListUsers = "ListUsers";
        public const String DeleteUser = "DeleteUser";
        public const String SetUser = "SetUser";
        public const String GetUser = "GetUser";
    }

    [Authorize(Roles = AuthorizationAdminRoles.EditRoles, ActiveAuthenticationSchemes = AuthCoreSchemes.Bearer)]
    public abstract class RolesControllerBase<TRoleAssignments, TCollection> : Controller
        where TRoleAssignments : IRoleAssignments, new()
        where TCollection : UserCollectionBase<TRoleAssignments>
    {
        private IRoleManager roleManager;
        private IHttpContextAccessor contextAccessor;

        public RolesControllerBase(IRoleManager roleManager, IHttpContextAccessor contextAccessor)
        {
            this.roleManager = roleManager;
            this.contextAccessor = contextAccessor;
        }

        [HttpGet]
        [HalRel(RolesControllerRels.ListUsers)]
        public async virtual Task<TCollection> ListUsers([FromQuery] RolesQuery query)
        {
            if (query.UserId.Count == 1)
            {
                var user = await this.roleManager.GetRoles<TRoleAssignments>(query.UserId[0], query.Name);
                query.SkipTo(1);
                return GetUserCollection(query, 1, new TRoleAssignments[] { user });
            }
            else if (query.UserId.Count > 1)
            {
                var users = await this.roleManager.GetRoles<TRoleAssignments>(query.UserId);
                query.Limit = query.UserId.Count;
                query.SkipTo(query.UserId.Count);
                return GetUserCollection(query, users.Count, users);
            }
            else
            {
                var users = await this.roleManager.GetUsers<TRoleAssignments>(query.Offset, query.Limit);
                query.SkipTo(users.Total);
                return GetUserCollection(query, users.Total, users.Results);
            }
        }

        protected abstract TCollection GetUserCollection(RolesQuery query, int total, IEnumerable<TRoleAssignments> users);

        [HttpGet("{UserId}")]
        [HalRel(RolesControllerRels.GetUser)]
        public virtual Task<TRoleAssignments> GetUser(Guid userId)
        {
            return this.roleManager.GetRoles<TRoleAssignments>(userId, $"Unknown - {userId}");
        }

        [HttpPut("{UserId}")]
        [HalRel(RolesControllerRels.SetUser)]
        [AutoValidate]
        public async virtual Task<TRoleAssignments> SetUser(Guid userId, [FromBody]TRoleAssignments value)
        {
            value.UserId = userId;
            await roleManager.SetRoles(value);
            return await GetUser(value.UserId);
        }

        /// <summary>
        /// Delete a user.
        /// </summary>
        /// <param name="userId">The user id to delete</param>
        [HttpDelete("{UserId}")]
        [HalRel(RolesControllerRels.DeleteUser)]
        [Authorize(Roles = AuthorizationAdminRoles.SuperAdmin, ActiveAuthenticationSchemes = AuthCoreSchemes.Bearer)]
        public virtual Task DeleteUser(Guid userId)
        {
            return roleManager.DeleteUser(userId);
        }
    }
}
