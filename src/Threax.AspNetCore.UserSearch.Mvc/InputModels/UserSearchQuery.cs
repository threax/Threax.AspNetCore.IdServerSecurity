using Halcyon.HAL.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Threax.AspNetCore.Halcyon.Ext;
using Threax.AspNetCore.Models;

namespace Spc.AspNetCore.Users.Mvc.InputModels
{
    [HalModel]
    public class UserSearchQuery : PagedCollectionQuery
    {
        [HiddenUiType]
        public Guid? UserId { get; set; }

        [UiOrder]
        public string UserName { get; set; }
    }
}
