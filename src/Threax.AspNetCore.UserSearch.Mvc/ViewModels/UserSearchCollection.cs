using Halcyon.HAL.Attributes;
using Spc.AspNetCore.Users.Mvc.Controllers;
using Spc.AspNetCore.Users.Mvc.InputModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Threax.AspNetCore.Halcyon.Ext;

namespace Spc.AspNetCore.Users.Mvc.ViewModels
{
    [HalModel]
    [HalSelfActionLink(typeof(UserSearchController), nameof(UserSearchController.List))]
    [HalActionLink(typeof(UserSearchController), nameof(UserSearchController.Get), DocsOnly = true)] //This provides access to docs for showing items
    [HalActionLink(typeof(UserSearchController), nameof(UserSearchController.List), DocsOnly = true)] //This provides docs for searching the list
    [DeclareHalLink(typeof(UserSearchController), nameof(UserSearchController.List), PagedCollectionView<Object>.Rels.Next, ResponseOnly = true)]
    [DeclareHalLink(typeof(UserSearchController), nameof(UserSearchController.List), PagedCollectionView<Object>.Rels.Previous, ResponseOnly = true)]
    [DeclareHalLink(typeof(UserSearchController), nameof(UserSearchController.List), PagedCollectionView<Object>.Rels.First, ResponseOnly = true)]
    [DeclareHalLink(typeof(UserSearchController), nameof(UserSearchController.List), PagedCollectionView<Object>.Rels.Last, ResponseOnly = true)]
    public class UserSearchCollection : PagedCollectionViewWithQuery<UserSearch, UserSearchQuery>
    {
        public UserSearchCollection(UserSearchQuery query, int total, IEnumerable<UserSearch> items) : base(query, total, items)
        {
        }
    }
}
