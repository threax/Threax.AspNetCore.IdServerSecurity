using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Threax.AspNetCore.AuthCore;
using Threax.SharedHttpClient;

namespace Threax.AspNetCore.JwtCookieAuth
{
    public class JwtCookieAuthenticationHandler :
        AuthenticationHandler<JwtCookieAuthenticationOptions>,
        IAuthenticationSignInHandler,
        IAuthenticationSignOutHandler
    {
        private ITokenValidationParametersProvider tokenValidationParametersProvider;
        private String responseRedirectPath = null;
        private ISharedHttpClient sharedHttpClient;

        public JwtCookieAuthenticationHandler(IOptionsMonitor<JwtCookieAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, ITokenValidationParametersProvider tokenValidationParametersProvider, ISharedHttpClient sharedHttpClient)
            : base(options, logger, encoder, clock)
        {
            this.tokenValidationParametersProvider = tokenValidationParametersProvider;
            this.sharedHttpClient = sharedHttpClient;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            String accessTokenString;
            if (!Request.Cookies.TryGetValue(BearerCookieName, out accessTokenString))
            {
                return AuthenticateResult.NoResult();
            }

            //Parse the access token and see if we should refresh it
            var handler = new JwtSecurityTokenHandler();
            ClaimsPrincipal principal = null;
            SecurityToken token = null;

            var accessTokenValidationParameters = await this.tokenValidationParametersProvider.GetTokenValidationParameters(Options);

            try
            {
                principal = handler.ValidateToken(accessTokenString, accessTokenValidationParameters, out token);
            }
            catch (SecurityTokenExpiredException ex)
            {
                EraseCookies();
                responseRedirectPath = Context.Request.GetDisplayUrl();
                Logger.LogInformation($"SecurityTokenExpiredException: Erasing Cookies and Redirecting to {responseRedirectPath}. Exception Message:\n{ex.Message}");
                return AuthenticateResult.Fail(ex);
            }
            CheckSecurityAlgo(token);

            //If we are beyond halfway through the refresh period, refresh the token from the id server.
            var timeSpan = token.ValidTo - token.ValidFrom;
            timeSpan = new TimeSpan(timeSpan.Ticks / 2);
            var endTime = token.ValidFrom + timeSpan;
            bool setupUserClaims = true;

            if (DateTime.UtcNow > endTime)
            {
                //Refresh from id server
                String refreshToken;
                if (!Request.Cookies.TryGetValue(RefreshCookieName, out refreshToken))
                {
                    return AuthenticateResult.Fail("No refresh token.");
                }

                var connectUri = new Uri(new Uri(Options.Authority), "/connect/token");
                var client = new TokenClient(sharedHttpClient.Client, new TokenClientOptions()
                {
                    Address = connectUri.AbsoluteUri,
                    ClientId = Options.ClientId,
                    ClientSecret = Options.ClientSecret
                });
                var response = await client.RequestRefreshTokenAsync(refreshToken);
                if (response.IsError)
                {
                    setupUserClaims = false;
                    return AuthenticateResult.Fail($"Could not refresh access token from {connectUri.AbsoluteUri}. Http Status Code: {response.HttpStatusCode} Message: {response.Error}");
                }
                else
                {
                    //Update the cookie
                    SetTokenCookies(response.AccessToken, token, response.RefreshToken);
                    accessTokenString = response.AccessToken;
                }
            }

            var authUserContext = new AuthorizeUserContext(principal, Context);

            if (setupUserClaims)
            {
                //Add the access token to the claims for the user so we can pass it on easily
                var claimsId = principal.Identity as ClaimsIdentity;
                if (claimsId != null)
                {
                    //Be sure to remove the old access tokens, this is a list not a dictionary and they will leak if not removed.
                    var oldAccessTokenClaim = claimsId.Claims.FirstOrDefault(i => i.Type == AuthCore.ClaimTypes.AccessToken);
                    if (oldAccessTokenClaim != null)
                    {
                        claimsId.RemoveClaim(oldAccessTokenClaim);
                    }
                    claimsId.AddClaim(new Claim(AuthCore.ClaimTypes.AccessToken, accessTokenString));
                }

                //Call client customizations, only done if we got a valid user up to this point.
                if (Options.Events != null)
                {
                    await Options.Events.ValidatePrincipal(authUserContext);
                }
            }

            if (authUserContext.IsRejected)
            {
                return AuthenticateResult.Fail("User was rejected.");
            }

            var authTicket = new AuthenticationTicket(principal, null, Scheme.Name);
            return AuthenticateResult.Success(authTicket);
        }

        public async Task SignInAsync(ClaimsPrincipal user, AuthenticationProperties properties)
        {
            var accessTokenString = properties.Items[".Token.access_token"];

            //Parse the access token
            var handler = new JwtSecurityTokenHandler();
            ClaimsPrincipal principal = null;
            SecurityToken token = null;

            //Validate the token
            var accessTokenValidationParameters = await this.tokenValidationParametersProvider.GetTokenValidationParameters(Options);
            principal = handler.ValidateToken(accessTokenString, accessTokenValidationParameters, out token);
            CheckSecurityAlgo(token);

            SetTokenCookies(accessTokenString, token, properties.Items[".Token.refresh_token"]);
        }

        public Task SignOutAsync(AuthenticationProperties properties)
        {
            EraseCookies();

            return Task.FromResult(0);
        }

        private void EraseCookies()
        {
            //There is no end to the pain of trying to get this right, fix the path here to ensure its correct.
            var cookiePath = CookieUtils.FixPath(Options.CookiePath);

            Response.Cookies.Delete(BearerCookieName, new CookieOptions()
            {
                Secure = true,
                HttpOnly = true,
                Path = cookiePath
            });

            Response.Cookies.Delete(RefreshCookieName, new CookieOptions()
            {
                Secure = true,
                HttpOnly = true,
                Path = cookiePath
            });
        }

        private void SetTokenCookies(String accessToken, SecurityToken token, String refreshToken)
        {
            //There is no end to the pain of trying to get this right, fix the path here to ensure its correct.
            var cookiePath = CookieUtils.FixPath(Options.CookiePath);

            var expires = Options.StoreCookiesInSession ? default(DateTimeOffset?) : token.ValidTo;

            Response.Cookies.Append(BearerCookieName, accessToken, new CookieOptions()
            {
                Secure = true,
                HttpOnly = true,
                Path = cookiePath,
                Expires = expires
            });

            Response.Cookies.Append(RefreshCookieName, refreshToken, new CookieOptions()
            {
                Secure = true,
                HttpOnly = true,
                Path = cookiePath,
                Expires = expires
            });
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            //If there is a bearer cookie for any reason, consider the user to be forbidden, dont challenge them in this case.
            if (Request.Cookies.ContainsKey(BearerCookieName))
            {
                return this.ForbidAsync(properties);
            }
            else
            {
                return Context.ChallengeAsync(Options.ChallengeScheme);
            }
        }

        protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            if (!String.IsNullOrEmpty(responseRedirectPath))
            {
                Response.Redirect(responseRedirectPath);
                return Task.CompletedTask;
            }

            if (!String.IsNullOrEmpty(Options.AccessDeniedPath))
            {
                Response.Redirect(BuildRedirectUri(Options.AccessDeniedPath));
                return Task.CompletedTask;
            }

            return base.HandleForbiddenAsync(properties);
        }

        private void CheckSecurityAlgo(SecurityToken token)
        {
            //This algorithm check NEEDS to stay, we have to make sure it is not set to none
            //If this were to be set to none, the signature check would pass since it
            //would be set to none, this would make it trivial to forge jwts.
            var jwt = token as JwtSecurityToken;
            if (!jwt.Header.Alg.Equals(Options.SecurityTokenAlgo, StringComparison.Ordinal))
            {
                throw new InvalidOperationException($"Algorithm must be '{Options.SecurityTokenAlgo}'");
            }
        }

        /// <summary>
        /// The name of the cookie for the bearer token.
        /// </summary>
        private String BearerCookieName => $"{Options.ClientId}.BearerToken";

        /// <summary>
        /// The name of the cookie for the refresh token.
        /// </summary>
        private String RefreshCookieName => $"{Options.ClientId}.RefreshToken";
    }
}
