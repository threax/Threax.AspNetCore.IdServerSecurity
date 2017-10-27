using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
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
        public async Task<IActionResult> Post([FromServices] IAntiforgery antiforgery)
        {
            //Make sure user is authenticated
            var result = await HttpContext.AuthenticateAsync(options.AuthenticationScheme);
            if (!result.Succeeded)
            {
                return new UnauthorizedResult();
            }

            //Validate antiforgery, have to set user manually
            HttpContext.User = result.Ticket.Principal;
            await antiforgery.ValidateRequestAsync(HttpContext);

            //Won't get here unless everything was valid
            return this.Json(new AccessTokenModel
            {
                AccessToken = result.Principal.GetAccessToken(),
                HeaderName = options.BearerHeaderName
            });
        }
    }
}
