using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Threax.AspNetCore.UserSearchMvc.InputModels;
using Threax.AspNetCore.UserSearchMvc.Services;
using Threax.AspNetCore.UserSearchMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Threax.AspNetCore.Halcyon.Ext;
using Threax.AspNetCore.UserBuilder.Entities;

namespace Threax.AspNetCore.UserSearchMvc.Controllers
{
    [Route("api/[controller]")]
    [ResponseCache(NoStore = true)]
    [Authorize(AuthenticationSchemes = AuthCoreSchemes.Bearer)]
    public class UserSearchController : Controller
    {
        private IUserSearchService userSearchService;

        public UserSearchController(IUserSearchService userSearchService)
        {
            this.userSearchService = userSearchService;
        }

        [HttpGet]
        [HalRel(CrudRels.List)]
        [Authorize(Roles = AuthorizationAdminRoles.EditRoles)]
        public Task<UserSearchCollection> List([FromQuery] UserSearchQuery query)
        {
            return userSearchService.List(query);
        }

        [HttpGet("{UserId}")]
        [HalRel(CrudRels.Get)]
        [Authorize(Roles = AuthorizationAdminRoles.EditRoles)]
        public Task<UserSearch> Get(Guid userId)
        {
            return userSearchService.Get(userId);
        }
    }
}
