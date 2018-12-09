using Threax.AspNetCore.UserSearchMvc.InputModels;
using Threax.AspNetCore.UserSearchMvc.ViewModels;
using System;
using System.Threading.Tasks;

namespace Threax.AspNetCore.UserSearchMvc.Services
{
    public interface IUserSearchService
    {
        Task<UserSearch> Get(Guid userId);
        Task<UserSearchCollection> List(UserSearchQuery query);
    }
}