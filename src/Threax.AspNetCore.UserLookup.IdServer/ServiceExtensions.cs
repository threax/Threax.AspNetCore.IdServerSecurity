using Threax.AspNetCore.UserLookup;
using Threax.AspNetCore.UserLookup.IdServer;

namespace Microsoft.Extensions.DependencyInjection.Extensions
{
    public static class ServiceExtensions
    {
        public static UserLookupOptions UseIdServer(this UserLookupOptions options)
        {
            options.UserSearchServiceType = typeof(UserSearchService);

            return options;
        }
    }
}
