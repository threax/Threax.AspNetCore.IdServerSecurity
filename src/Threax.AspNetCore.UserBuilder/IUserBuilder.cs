using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Threax.AspNetCore.UserBuilder
{
    /// <summary>
    /// An interface to determine if a user is valid.
    /// </summary>
    public interface IUserBuilder
    {
        /// <summary>
        /// Validate and build up the ClaimsPrincipal for the current user.
        /// Will return true if the user can build the website. This process
        /// will also customize the user with any additional roles that they
        /// are configured to have.
        /// </summary>
        /// <param name="principal">The principal to inspect.</param>
        /// <returns>True if valid, false otherwise.</returns>
        Task<bool> ValidateAndBuildUser(ClaimsPrincipal principal);
    }
}
