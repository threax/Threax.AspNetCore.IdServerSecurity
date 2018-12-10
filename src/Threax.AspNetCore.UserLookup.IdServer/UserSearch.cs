using System;
using System.Collections.Generic;
using System.Text;

namespace Threax.AspNetCore.UserLookup.IdServer
{
    public class UserSearch : IUserSearch
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
    }
}
