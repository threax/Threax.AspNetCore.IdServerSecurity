using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Threax.AspNetCore.UserBuilder.Entities
{
    public interface IRoleQuery
    {
        int Offset { get; set; }
        
        int Limit { get; set; }

        IQueryable<User> Create(IQueryable<User> query);
    }
}
