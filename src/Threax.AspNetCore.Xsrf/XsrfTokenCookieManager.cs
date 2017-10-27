using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Threax.AspNetCore.Xsrf
{
    public class XsrfTokenCookieManager : IXsrfTokenCookieManager
    {
        private IAntiforgery antiforgery;
        private HttpContext httpContext;
        private XsrfOptions options;

        public XsrfTokenCookieManager(IAntiforgery antiforgery, IHttpContextAccessor httpContextAccessor, XsrfOptions options)
        {
            this.antiforgery = antiforgery;
            this.httpContext = httpContextAccessor.HttpContext;
            this.options = options;
        }

        public void SetupXsrfCookie()
        {
            var tokens = antiforgery.GetAndStoreTokens(httpContext);
            httpContext.Response.Cookies.Append(options.TokenCookie.Name, tokens.RequestToken, options.TokenCookie.Build(httpContext));
        }
    }
}
