using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Threax.AspNetCore.UserLookup.IdServer
{
    public class UserSearchService : IUserSearchService
    {
        private Threax.IdServer.Client.EntryPointsInjector injector;

        public UserSearchService(Threax.IdServer.Client.EntryPointsInjector injector)
        {
            this.injector = injector;
        }

        public async Task<IUserSearch> Get(Guid userId)
        {
            var entry = await injector.Load();
            var users = await entry.ListIdServerUsers(new Threax.IdServer.Client.IdServerUserQuery()
            {
                UserId = userId,
                Limit = int.MaxValue
            });
            var user = users.Items.First();
            return new UserSearch()
            {
                UserId = user.Data.UserId,
                UserName = user.Data.UserName
            };
        }

        public async Task<UserSearchResult> List(IUserSearchQuery query)
        {
            var entry = await injector.Load();
            var users = await entry.ListIdServerUsers(new Threax.IdServer.Client.IdServerUserQuery()
            {
                Limit = query.Limit,
                Offset = query.Offset,
                UserId = query.UserId,
                UserName = query.UserName
            });

            return new UserSearchResult()
            {
                Total = users.Data.Total,
                Results = users.Items.Select(user =>
                new UserSearch()
                {
                    UserId = user.Data.UserId,
                    UserName = user.Data.UserName
                })
            };
        }
    }
}
