using Spc.AspNetCore.Users.Mvc.InputModels;
using Spc.AspNetCore.Users.Mvc.ViewModels;
using System;
using System.Threading.Tasks;

namespace Spc.AspNetCore.Users.Mvc.Services
{
    public interface IUserSearchService
    {
        Task<UserSearch> Get(Guid userId);
        Task<UserSearchCollection> List(UserSearchQuery query);
    }
}