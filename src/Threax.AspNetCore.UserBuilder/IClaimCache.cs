using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Threax.AspNetCore.UserBuilder
{
    public interface IClaimCache
    {
        Task<bool> GetClaims(ClaimsPrincipal user);
        Task UpdateClaims(ClaimsPrincipal user);
        Task Clear();
        Task Clear(Guid userId);
    }
}