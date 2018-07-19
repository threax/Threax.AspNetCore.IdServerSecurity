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

    [Authorize(Roles = AuthorizationAdminRoles.EditRoles, AuthenticationSchemes = AuthCoreSchemes.Bearer)]
    public abstract class RolesControllerBase<TRoleAssignments, TCollection, TRoleQuery> : Controller
        where TRoleAssignments : IRoleAssignments, new()
        where TCollection : UserCollectionBase<TRoleAssignments, TRoleQuery>
        where TRoleQuery : RolesQuery
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
        public async virtual Task<TCollection> ListUsers([FromQuery] TRoleQuery query)
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
                var users = await this.roleManager.GetUsers<TRoleAssignments>(query);
                return GetUserCollection(query, users.Total, users.Results);
            }
        }

        protected abstract TCollection GetUserCollection(TRoleQuery query, int total, IEnumerable<TRoleAssignments> users);

        [HttpGet("{UserId}")]
        [HalRel(RolesControllerRels.GetUser)]
        public async Task<TRoleAssignments> GetUser(Guid userId)
        {
            var assignments = await this.roleManager.GetRoles<TRoleAssignments>(userId, $"Unknown - {userId}");
            await OnGetUser(userId, assignments);
            return assignments;
        }

        protected virtual Task OnGetUser(Guid userId, TRoleAssignments roleAssignments)
        {
            return Task.CompletedTask;
        }

        [HttpPut("{UserId}")]
        [HalRel(RolesControllerRels.SetUser)]
        [AutoValidate]
        public async Task<TRoleAssignments> SetUser(Guid userId, [FromBody]TRoleAssignments value, [FromServices] IClaimCache claimCache)
        {
            value.UserId = userId;
            await roleManager.SetRoles(value);
            await OnUserSet(userId, value);
            await claimCache.Clear(value.UserId);
            return await GetUser(value.UserId);
        }

        protected virtual Task OnUserSet(Guid userId, TRoleAssignments value)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Delete a user.
        /// </summary>
        /// <param name="userId">The user id to delete</param>
        /// <param name="claimCache">The claim cache.</param>
        [HttpDelete("{UserId}")]
        [HalRel(RolesControllerRels.DeleteUser)]
        [Authorize(Roles = AuthorizationAdminRoles.SuperAdmin, AuthenticationSchemes = AuthCoreSchemes.Bearer)]
        public async Task DeleteUser(Guid userId, [FromServices] IClaimCache claimCache)
        {
            await roleManager.DeleteUser(userId);
            await OnUserDeleted(userId);
            await claimCache.Clear(userId);
        }

        public virtual Task OnUserDeleted(Guid userId)
        {
            return Task.CompletedTask;
        }
    }

    public abstract class RolesControllerBase<TRoleAssignments, TCollection> : RolesControllerBase<TRoleAssignments, TCollection, RolesQuery>
        where TRoleAssignments : IRoleAssignments, new()
        where TCollection : UserCollectionBase<TRoleAssignments>
    {
        public RolesControllerBase(IRoleManager roleManager, IHttpContextAccessor contextAccessor) : base(roleManager, contextAccessor)
        {
        }
    }
}
