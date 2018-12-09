using Spc.AspNetCore.Users.Mvc.InputModels;
using Spc.AspNetCore.Users.Mvc.Services;
using Spc.AspNetCore.Users.Mvc.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spc.AspNetCore.Users.Mvc.Services
{
    public class UserSearchService : IUserSearchService
    {
        private Threax.IdServer.Client.EntryPointsInjector injector;

        public UserSearchService(Threax.IdServer.Client.EntryPointsInjector injector)
        {
            this.injector = injector;
        }

        public async Task<Spc.AspNetCore.Users.Mvc.ViewModels.UserSearch> Get(Guid userId)
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

        public async Task<UserSearchCollection> List(UserSearchQuery query)
        {
            var entry = await injector.Load();
            var users = await entry.ListIdServerUsers(new Threax.IdServer.Client.IdServerUserQuery()
            {
                Limit = query.Limit,
                Offset= query.Offset,
                UserId = query.UserId,
                UserName = query.UserName
            });
            var results = users.Items.Select(user =>
                new UserSearch()
                {
                    UserId = user.Data.UserId,
                    UserName = user.Data.UserName
                });
            return new UserSearchCollection(query, users.Data.Total, results);
        }
    }
}
