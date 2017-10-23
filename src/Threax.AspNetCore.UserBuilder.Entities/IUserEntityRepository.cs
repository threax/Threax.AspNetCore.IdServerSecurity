using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Threax.AspNetCore.UserBuilder;

namespace Threax.AspNetCore.UserBuilder.Entities
{
    public interface IUserEntityRepository : IUsersRepository
    {
        Task<User> GetUser(Guid id);

        Task<List<User>> GetUsers(IEnumerable<Guid> ids);

        Task AddUser(User user, IEnumerable<String> roles);

        Task UpdateUser(User user, IEnumerable<Tuple<String, bool>> roles);

        IQueryable<User> GetUsers();

        Task DeleteUser(Guid userId);
    }
}
