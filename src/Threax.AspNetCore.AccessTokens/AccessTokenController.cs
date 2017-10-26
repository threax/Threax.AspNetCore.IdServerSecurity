﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Threax.AspNetCore.AuthCore;

namespace Threax.AspNetCore.AccessTokens
{
    [Route("[controller]")]
    [ResponseCache(NoStore = true)]
    public class AccessTokenController : Controller
    {
        private AccessTokenOptions options;

        public AccessTokenController(AccessTokenOptions options)
        {
            this.options = options;
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var result = await HttpContext.AuthenticateAsync(options.AuthenticationScheme);

            if (result.Succeeded)
            {
                return this.Json(new AccessTokenModel
                {
                    AccessToken = result.Principal.GetAccessToken(),
                    HeaderName = options.BearerHeaderName
                });
            }
            else
            {
                return new UnauthorizedResult();
            }
        }
    }
}