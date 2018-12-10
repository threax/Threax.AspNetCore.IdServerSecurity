using System;
using System.Collections.Generic;
using System.Text;

namespace Threax.AspNetCore.UserLookup
{
    public class UserSearchResult
    {
        public int Total { get; set; }

        public IEnumerable<IUserSearch> Results { get; set; }
    }
}
