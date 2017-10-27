using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Threax.AspNetCore.AuthCore;

namespace Threax.AspNetCore.IdServerAuth
{
    public class JwtBearerAuthenticationEvents
    {
        String securityTokenAlgo;

        public Func<AuthorizeUserContext, Task> OnAuthorizeUser { get; set; }

        public JwtBearerAuthenticationEvents(String securityTokenAlgo)
        {
            this.securityTokenAlgo = securityTokenAlgo;
        }

        public async Task TokenValidated(TokenValidatedContext context)
        {
            //Add the access token to the claims
            var claimsId = context.Principal.Identity as ClaimsIdentity;
            if (claimsId != null)
            {
                claimsId.AddClaim(new Claim(AuthCore.ClaimTypes.AccessToken, (context.SecurityToken as JwtSecurityToken).RawData));
            }

            var jwt = context.SecurityToken as JwtSecurityToken;

            //This algorithm check NEEDS to stay, we have to make sure it is not set to none
            //If this were to be set to none, the signature check would pass since it
            //would be set to none, this would make it trivial to forge jwts.
            if (!jwt.Header.Alg.Equals(securityTokenAlgo, StringComparison.Ordinal))
            {
                throw new InvalidOperationException($"Algorithm must be '{securityTokenAlgo}'");
            }

            var now = DateTime.UtcNow;
            if (now < jwt.ValidFrom || now > jwt.ValidTo)
            {
                //Check dates, return unauthorized if they do not match
                context.Fail("Dates not valid.");
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }

            var authContext = new AuthorizeUserContext(context.Principal, context.HttpContext);

            if (OnAuthorizeUser != null)
            {
                await OnAuthorizeUser.Invoke(authContext);
            }
            
            if (authContext.IsRejected)
            {
                //Check that the rejected claim was not set somewhere, keep this last to ensure its picked up
                context.Fail("Principal was rejected.");
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            }
        }
    }
}
