using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Threax.AspNetCore.UserLookup.Mvc.ViewModels;

namespace Threax.AspNetCore.UserLookup.Mvc.Mappers
{
    public partial class AppMapper
    {
        public UserSearch MapValue(IUserSearch src, UserSearch dest)
        {
            return mapper.Map(src, dest);
        }
    }

    public partial class ValueProfile : Profile
    {
        public ValueProfile()
        {
            CreateMap<IUserSearch, UserSearch>();
        }
    }
}
