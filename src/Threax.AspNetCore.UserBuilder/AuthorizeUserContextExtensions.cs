using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Threax.AspNetCore.AuthCore;
using Microsoft.Extensions.DependencyInjection;

namespace Threax.AspNetCore.UserBuilder
{
    public static class AuthorizeUserContextExtensions
    {
        public static async Task BuildUserWithRequestServices(this AuthorizeUserContext context)
        {
            var userBuilder = context.HttpContext.RequestServices.GetService<IUserBuilder>();
            if (!await userBuilder.ValidateAndBuildUser(context.ClaimsPrincipal))
            {
                context.Reject();
            }
        }
    }
}
