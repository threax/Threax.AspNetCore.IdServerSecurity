using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Threax.AspNetCore.Xsrf;

namespace Threax.AspNetCore.IdServerAuth
{
    public class TokenConfigTagHelper : TagHelper
    {
        private IXsrfTokenCookieManager xsrfCookieManager;
        private XsrfOptions options;
        private IUrlHelperFactory urlHelperFactory;

        public TokenConfigTagHelper(IXsrfTokenCookieManager xsrfCookieManager, XsrfOptions options, IUrlHelperFactory urlHelperFactory)
        {
            this.xsrfCookieManager = xsrfCookieManager;
            this.options = options;
            this.urlHelperFactory = urlHelperFactory;
        }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "script";
            output.Attributes.Add("type", "text/javascript");

            xsrfCookieManager.SetupXsrfCookie();

            var urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);

            IEnumerable<String> paths = new String[] { "~/AccessToken" };
            if (options.Paths != null)
            {
                paths = paths.Concat(options.Paths);
            }
            paths = paths.Select(i => urlHelper.Content(i));

            var jObj = new JObject();
            jObj.Add("AccessTokenPath", urlHelper.Content("~/AccessToken"));
            jObj.Add("XsrfCookie", options.TokenCookie.Name);
            jObj.Add("XsrfPaths", JToken.FromObject(paths));

            //Convert to html this way so we don't escape the settings object.
            var html = String.Format(content, jObj.ToString());
            output.Content.SetHtmlContent(html);
        }

        //0 - json
        private const String content = "window.xsrfConfig = {0};";
    }
}
