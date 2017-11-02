using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Threax.AspNetCore.AuthCore;

namespace Threax.AspNetCore.UserBuilder
{
    public class ClaimCache : IClaimCache
    {
        private AsyncReaderWriterLock locker = new AsyncReaderWriterLock();
        private Dictionary<Guid, List<Claim>> cache = new Dictionary<Guid, List<Claim>>();
        private List<String> cacheClaimTypes;

        public ClaimCache(IEnumerable<String> cacheClaimTypes)
        {
            this.cacheClaimTypes = new List<string>(cacheClaimTypes);
        }

        public async Task UpdateClaims(ClaimsPrincipal user)
        {
            using(await locker.WriterLockAsync())
            {
                cache[user.GetUserGuid()] = user.Claims.Where(i => this.cacheClaimTypes.Contains(i.Type)).ToList();
            }
        }

        public async Task<bool> GetClaims(ClaimsPrincipal user)
        {
            List<Claim> claims = null;
            using (await locker.ReaderLockAsync())
            {
                cache.TryGetValue(user.GetUserGuid(), out claims);
            }

            if(claims != null)
            {
                var claimsId = user.Identity as ClaimsIdentity;
                if(claimsId != null)
                {
                    claimsId.AddClaims(claims);
                    return true;
                }
            }

            return false;
        }

        public async Task Clear()
        {
            using (await locker.WriterLockAsync())
            {
                cache.Clear();
            }
        }

        public async Task Clear(Guid userId)
        {
            using (await locker.WriterLockAsync())
            {
                cache.Remove(userId);
            }
        }
    }
}
