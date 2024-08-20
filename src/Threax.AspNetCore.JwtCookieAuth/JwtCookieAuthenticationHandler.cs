using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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
        private readonly ITokenValidationParametersProvider tokenValidationParametersProvider;
        private readonly ISharedHttpClient sharedHttpClient;
        private readonly ICookieManager cookieManager;

        private String responseRedirectPath = null;

        public JwtCookieAuthenticationHandler(
            IOptionsMonitor<JwtCookieAuthenticationOptions> options, 
            ILoggerFactory logger, 
            UrlEncoder encoder, 
            ISystemClock clock, 
            ITokenValidationParametersProvider tokenValidationParametersProvider, 
            ISharedHttpClient sharedHttpClient)
            : base(options, logger, encoder, clock)
        {
            this.tokenValidationParametersProvider = tokenValidationParametersProvider;
            this.sharedHttpClient = sharedHttpClient;
            this.cookieManager = new ChunkingCookieManager();
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            String accessTokenString = cookieManager.GetRequestCookie(Context, BearerCookieName);
            if (accessTokenString == null)
            {
                return AuthenticateResult.NoResult();
            }

            //Parse the access token and see if we should refresh it
            var handler = new JwtSecurityTokenHandler();
            ClaimsPrincipal principal = null;
            SecurityToken token = null;

            var accessTokenValidationParameters = await this.tokenValidationParametersProvider.GetTokenValidationParameters(Options);
            bool setupUserClaims = true;
            bool expired = false;

            try
            {
                principal = ValidateAndGetAccessToken(accessTokenString, handler, accessTokenValidationParameters, out token);

                //If not already expired and if we are beyond halfway through the refresh period, refresh the token from the id server.
                var timeSpan = token.ValidTo - token.ValidFrom;
                timeSpan = new TimeSpan(timeSpan.Ticks / 2);
                var endTime = token.ValidFrom + timeSpan;
                expired = DateTime.UtcNow > endTime;
            }
            catch (SecurityTokenExpiredException ex)
            {
                Logger.LogInformation($"SecurityTokenExpiredException: Will attempt to refresh. Exception Message: '{ex.Message}'");
                expired = true;
            }

            if (expired)
            {
                //Refresh from id server
                String refreshToken = cookieManager.GetRequestCookie(Context, RefreshCookieName);
                if (refreshToken == null)
                {
                    EraseCookies();
                    responseRedirectPath = Context.Request.GetDisplayUrl();
                    Logger.LogInformation($"No refresh token found. Cannot refresh credentials.");
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
                    EraseCookies();
                    responseRedirectPath = Context.Request.GetDisplayUrl();
                    Logger.LogInformation($"Error refreshing token. Erasing Cookies and Redirecting to '{responseRedirectPath}'. Error Info: '{response.Error}' '{response.ErrorDescription}' '{response.ErrorType}'");
                    return AuthenticateResult.Fail($"Could not refresh access token from {connectUri.AbsoluteUri}. Http Status Code: {response.HttpStatusCode} Message: {response.Error}");
                }
                else
                {
                    accessTokenString = response.AccessToken;
                    principal = ValidateAndGetAccessToken(accessTokenString, handler, accessTokenValidationParameters, out token);
                    SetTokenCookies(accessTokenString, token, response.RefreshToken);

                    Logger.LogInformation($"Sucessfully refreshed access token.");
                }
            }

            var authUserContext = new AuthorizeUserContext(principal, Context.RequestServices);

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

            //Validate the token
            var handler = new JwtSecurityTokenHandler();
            var accessTokenValidationParameters = await this.tokenValidationParametersProvider.GetTokenValidationParameters(Options);
            ValidateAndGetAccessToken(accessTokenString, handler, accessTokenValidationParameters, out var token);

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

            cookieManager.DeleteCookie(Context, BearerCookieName, new CookieOptions()
            {
                Secure = true,
                HttpOnly = Options.BearerHttpOnly,
                Path = cookiePath
            });

            cookieManager.DeleteCookie(Context, RefreshCookieName, new CookieOptions()
            {
                Secure = true,
                HttpOnly = Options.RefreshHttpOnly,
                Path = cookiePath
            });
        }

        private void SetTokenCookies(String accessToken, SecurityToken token, String refreshToken)
        {
            //There is no end to the pain of trying to get this right, fix the path here to ensure its correct.
            var cookiePath = CookieUtils.FixPath(Options.CookiePath);

            var expires = Options.StoreCookiesInSession ? default(DateTimeOffset?) : token.ValidTo;

            cookieManager.AppendResponseCookie(Context, BearerCookieName, accessToken, new CookieOptions()
            {
                Secure = true,
                HttpOnly = Options.BearerHttpOnly,
                Path = cookiePath,
                Expires = expires,
                SameSite = Options.SameSite
            });

            cookieManager.AppendResponseCookie(Context, RefreshCookieName, refreshToken, new CookieOptions()
            {
                Secure = true,
                HttpOnly = Options.RefreshHttpOnly,
                Path = cookiePath,
                Expires = expires,
                SameSite = Options.SameSite
            });
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            //If there is a bearer cookie for any reason, consider the user to be forbidden, dont challenge them in this case.
            if (cookieManager.GetRequestCookie(Context, BearerCookieName) != null)
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

        /// <summary>
        /// The name of the cookie for the bearer token.
        /// </summary>
        private String BearerCookieName => $"{Options.ClientId}.BearerToken";

        /// <summary>
        /// The name of the cookie for the refresh token.
        /// </summary>
        private String RefreshCookieName => $"{Options.ClientId}.RefreshToken";

        private ClaimsPrincipal ValidateAndGetAccessToken(string accessTokenString, JwtSecurityTokenHandler handler, TokenValidationParameters accessTokenValidationParameters, out SecurityToken token)
        {
            var principal = handler.ValidateToken(accessTokenString, accessTokenValidationParameters, out token);

            //This algorithm check NEEDS to stay, we have to make sure it is not set to none
            //If this were to be set to none, the signature check would pass since it
            //would be set to none, this would make it trivial to forge jwts.
            var jwt = token as JwtSecurityToken;
            if (!jwt.Header.Alg.Equals(Options.SecurityTokenAlgo, StringComparison.Ordinal))
            {
                throw new InvalidOperationException($"Algorithm must be '{Options.SecurityTokenAlgo}'");
            }

            return principal;
        }
    }
}
