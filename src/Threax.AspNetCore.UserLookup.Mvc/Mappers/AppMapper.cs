using System;
using System.Collections.Generic;
using System.Text;
using Threax.AspNetCore.UserLookup.Mvc.ViewModels;

namespace Threax.AspNetCore.UserLookup.Mvc.Mappers
{
    /// <summary>
    /// The app mapper defines all the object mappings that this application can perform.
    /// Usually this is just a thin wrapper over automapper, but it establishes what mappings
    /// are supported and enables more advanced mappings between multiple objects.
    /// </summary>
    public class AppMapper
    {
        public AppMapper()
        {
            
        }

        public UserSearch MapValue(IUserSearch src, UserSearch dest)
        {
            dest.UserId = src.UserId;
            dest.UserName = src.UserName;

            return dest;
        }
    }
}
