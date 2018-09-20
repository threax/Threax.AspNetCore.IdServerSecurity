using Halcyon.HAL.Attributes;
using Spc.AspNetCore.Users.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Threax.AspNetCore.Halcyon.Ext;
using Threax.AspNetCore.Models;

namespace Spc.AspNetCore.Users.Mvc.ViewModels
{
    [HalModel]
    [HalSelfActionLink(typeof(UserSearchController), nameof(UserSearchController.Get))]
    public class UserSearch
    {
        [UiOrder]
        public Guid UserId { get; set; }

        [UiOrder]
        public string UserName { get; set; }
    }
}
