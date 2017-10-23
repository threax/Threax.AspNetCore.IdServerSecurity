using System;
using System.Collections.Generic;
using System.Text;

namespace Threax.AspNetCore.UserBuilder.Entities
{
    public class SystemUserRoleInfo<TRoleAssignmentType>
         where TRoleAssignmentType : IRoleAssignments, new()
    {
        public IEnumerable<TRoleAssignmentType> Results { get; set; }

        public int Total { get; set; }
    }
}
