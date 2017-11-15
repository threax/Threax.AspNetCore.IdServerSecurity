using System;
using System.Collections.Generic;
using System.Text;
using Threax.AspNetCore.JwtCookieAuth;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Threax.AspNetCore.AccessTokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Threax.AspNetCore.IdServerAuth
{
    public class IdServerAuthOptions
    {
        /// <summary>
        /// The app specific options, you must fill this out with your application's specific info.
        /// The defaults for the other properties should be correct for most applications.
        /// </summary>
        public IdServerAuthAppOptions AppOptions { get; set; }

        /// <summary>
        /// Set this to true (default) to enable the access token middleware to allow this application to return
        /// access tokens.
        /// </summary>
        public bool EnableAccessTokenMiddleware { get; set; } = true;

        /// <summary>
        /// The options for access tokens, used if EnableAccessTokenMiddleware is true.
        /// </summary>
        public AccessTokenOptions AccessTokenOptions { get; set; } = new AccessTokenOptions();

        /// <summary>
        /// Set this to true (default) to enable id server metadata.
        /// </summary>
        public bool EnableIdServerMetadata { get; set; } = true;

        /// <summary>
        /// The path to use for any auth cookies created.
        /// This can be null, but you should set it to your app's base path.
        /// </summary>
        public String CookiePath { get; set; }

        /// <summary>
        /// Set this to true (default) to allow your application to act as a client. This means
        /// that it will enable jwt cookie auth and expose access tokens for the client side (if configured).
        /// Use this if you are going to have razor views that this app will return.
        /// </summary>
        public bool ActAsClient { get; set; } = true;

        /// <summary>
        /// Set this to true (default) to allow your application to act as an api. This will enable
        /// bearer auth and setup other api related properties.
        /// </summary>
        public bool ActAsApi { get; set; } = true;

        /// <summary>
        /// If you need to override the default scheme, set it here. By default this will not be set to anything
        /// and you must explicitly list which schemes will be used in your Authorize attributes.
        /// </summary>
        public String DefaultScheme { get; set; }

        /// <summary>
        /// The signout path to use when signing out. Defaults to "/Account/SignoutCleanup".
        /// </summary>
        public String RemoteSignOutPath { get; set; } = "/Account/SignoutCleanup";

        /// <summary>
        /// The signout path to use when signing out. Defaults to "/Account/SignoutCleanup".
        /// </summary>
        public String AccessDeniedPath { get; set; }

        /// <summary>
        /// Customize the jwt bearer options, this is called after the automatic configuration, so you can override any
        /// settings you need. This is only called if your app supports bearer auth.
        /// </summary>
        public Action<JwtBearerOptions> CustomizeBearer { get; set; }

        /// <summary>
        /// Customize cookie options. This is called after automatic configuration, so you can override any settings you need.
        /// This is only called if your app supports cookie auth.
        /// </summary>
        public Action<JwtCookieAuthenticationOptions> CustomizeCookies { get; set; }

        /// <summary>
        /// Customize the open id connect options. This is called after automatic configuration, so you can override any settings you need.
        /// This is only called if your app supports cookie auth.
        /// </summary>
        public Action<OpenIdConnectOptions> CustomizeOpenIdConnect { get; set; }
    }
}
