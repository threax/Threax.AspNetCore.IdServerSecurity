using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Threax.AspNetCore.UserSearchMvc.InputModels;
using Threax.AspNetCore.UserSearchMvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Threax.AspNetCore.Halcyon.Ext;
using Threax.AspNetCore.UserBuilder.Entities;
using Threax.AspNetCore.UserLookup;
using Threax.AspNetCore.UserLookup.Mvc.Mappers;

namespace Threax.AspNetCore.UserSearchMvc.Controllers
{
    [Route("api/[controller]")]
    [ResponseCache(NoStore = true)]
    [Authorize(AuthenticationSchemes = AuthCoreSchemes.Bearer)]
    public class UserSearchController : Controller
    {
        private IUserSearchService userSearchService;
        private AppMapper mapper;

        public UserSearchController(IUserSearchService userSearchService, AppMapper mapper)
        {
            this.userSearchService = userSearchService;
            this.mapper = mapper;
        }

        [HttpGet]
        [HalRel(CrudRels.List)]
        [Authorize(Roles = AuthorizationAdminRoles.EditRoles)]
        public async Task<UserSearchCollection> List([FromQuery] UserSearchQuery query)
        {
            var users = await userSearchService.List(query);
            var results = users.Results.Select(i => mapper.MapValue(i, new UserSearch()));
            return new UserSearchCollection(query, users.Total, results);
        }

        [HttpGet("{UserId}")]
        [HalRel(CrudRels.Get)]
        [Authorize(Roles = AuthorizationAdminRoles.EditRoles)]
        public async Task<UserSearch> Get(Guid userId)
        {
            var user = await userSearchService.Get(userId);
            return mapper.MapValue(user, new UserSearch());
        }
    }
}
