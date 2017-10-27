using Microsoft.AspNetCore.Antiforgery.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Claims;
using Threax.AspNetCore.AuthCore;
using Microsoft.Extensions.ObjectPool;

namespace Threax.AspNetCore.AccessTokens
{
    class ClaimUidExtractor : IClaimUidExtractor
    {
        private readonly ObjectPool<AntiforgerySerializationContext> _pool;

        public ClaimUidExtractor(ObjectPool<AntiforgerySerializationContext> pool)
        {
            this._pool = pool;
        }

        public string ExtractClaimUid(ClaimsPrincipal claimsPrincipal)
        {
            var claimUidBytes = ComputeSha256(new String[] { claimsPrincipal.GetUserGuid().ToString() });
            return Convert.ToBase64String(claimUidBytes);
        }

        private byte[] ComputeSha256(IEnumerable<string> parameters)
        {
            var serializationContext = _pool.Get();

            try
            {
                var writer = serializationContext.Writer;
                foreach (string parameter in parameters)
                {
                    writer.Write(parameter); // also writes the length as a prefix; unambiguous
                }

                writer.Flush();

                var sha256 = serializationContext.Sha256;
                var stream = serializationContext.Stream;
                var bytes = sha256.ComputeHash(stream.ToArray(), 0, checked((int)stream.Length));

                return bytes;
            }
            finally
            {
                _pool.Return(serializationContext);
            }
        }
    }
}
