using Halcyon.HAL.Attributes;
using Threax.AspNetCore.UserSearchMvc.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Threax.AspNetCore.Halcyon.Ext;
using Threax.AspNetCore.Models;
using Threax.AspNetCore.UserLookup;

namespace Threax.AspNetCore.UserSearchMvc.ViewModels
{
    [HalModel]
    [HalSelfActionLink(typeof(UserSearchController), nameof(UserSearchController.Get))]
    public class UserSearch : IUserSearch
    {
        [UiOrder]
        public Guid UserId { get; set; }

        [UiOrder]
        public string UserName { get; set; }
    }
}
