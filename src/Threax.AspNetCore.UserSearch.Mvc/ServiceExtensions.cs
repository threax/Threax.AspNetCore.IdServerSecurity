using System;
using System.Collections.Generic;
using System.Text;
using Spc.AspNetCore.Users.Mvc;
using Spc.AspNetCore.Users.Mvc.Controllers;
using Spc.AspNetCore.Users.Mvc.Services;

namespace Microsoft.Extensions.DependencyInjection.Extensions
{
    public static class ServiceExtensions
    {
        public static IMvcBuilder AddUserSearchMvc(this IMvcBuilder builder)
        {
            builder.Services.TryAddScoped<AppMapper>();
            builder.Services.TryAddScoped<IUserSearchService, UserSearchService>();

            return builder;
        }

    }
}
