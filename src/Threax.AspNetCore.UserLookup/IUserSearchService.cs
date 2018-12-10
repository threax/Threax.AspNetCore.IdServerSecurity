using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Threax.AspNetCore.UserLookup
{
    public interface IUserSearchService
    {
        Task<IUserSearch> Get(Guid userId);
        Task<UserSearchResult> List(IUserSearchQuery query);
    }
}
