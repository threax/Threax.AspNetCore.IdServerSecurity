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

        IQueryable<User> Create(UsersDbContext userDbContext);
    }

    public static class IRoleQueryExtensions
    {
        /// <summary>
        /// Get the page to skip to within total, this will alter the offset
        /// stored in this class to 0 if the skip to target is > total. If you don't want
        /// the offset to alter pass null for total (or no arg) and it will just calculate the skip
        /// value.
        /// </summary>
        /// <remarks>
        /// This makes it easy to use with the common pattern for CollectionViews since you can
        /// just pass the query along after calling this function and it will have been
        /// altered as needed.
        /// </remarks>
        /// <param name="query">The query.</param>
        /// <param name="total">The total count of items in the collection to query or null for no check.</param>
        /// <returns></returns>
        public static int SkipTo(this IRoleQuery query, int? total = null)
        {
            var skipTo = query.Offset * query.Limit;
            if (total.HasValue && skipTo > total)
            {
                skipTo = 0;
                query.Offset = 0;
            }
            return skipTo;
        }
    }
}
