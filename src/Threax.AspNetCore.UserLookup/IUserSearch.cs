using System;
using System.Collections.Generic;
using System.Text;

namespace Threax.AspNetCore.UserLookup
{
    public interface IUserSearch
    {
        Guid UserId { get; set; }

        string UserName { get; set; }
    }
}
