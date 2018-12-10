using System;
using System.Collections.Generic;
using System.Text;

namespace Threax.AspNetCore.UserLookup
{
    public interface IUserSearchQuery
    {
        Guid? UserId { get; set; }

        string UserName { get; set; }

        int Offset { get; set; }
        
        int Limit { get; set; }
    }
}
