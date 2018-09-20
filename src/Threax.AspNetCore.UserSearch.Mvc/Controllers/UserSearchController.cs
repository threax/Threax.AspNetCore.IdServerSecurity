using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spc.AspNetCore.Users.Mvc.InputModels;
using Spc.AspNetCore.Users.Mvc.Services;
using Spc.AspNetCore.Users.Mvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Threax.AspNetCore.Halcyon.Ext;
using Threax.AspNetCore.UserBuilder.Entities;

namespace Spc.AspNetCore.Users.Mvc.Controllers
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
