using Halcyon.HAL.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Threax.AspNetCore.Halcyon.Ext;
using Threax.AspNetCore.Models;
using Threax.AspNetCore.UserLookup;

namespace Threax.AspNetCore.UserLookup.Mvc.InputModels
{
    [HalModel]
    public class UserSearchQuery : PagedCollectionQuery, IUserSearchQuery
    {
        [HiddenUiType]
        public Guid? UserId { get; set; }

        [UiOrder]
        public string UserName { get; set; }
    }
}
