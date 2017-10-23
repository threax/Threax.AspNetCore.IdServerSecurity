using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Threax.AspNetCore.JwtCookieAuth;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.Builder
{
    public static class JwtCookieExtensions
    {
        public static AuthenticationBuilder AddJwtCookie(this AuthenticationBuilder builder)
            => builder.AddJwtCookie(JwtCookieAuthenticationDefaults.AuthenticationScheme);

        public static AuthenticationBuilder AddJwtCookie(this AuthenticationBuilder builder, string authenticationScheme)
            => builder.AddJwtCookie(authenticationScheme, configureOptions: null);

        public static AuthenticationBuilder AddJwtCookie(this AuthenticationBuilder builder, Action<JwtCookieAuthenticationOptions> configureOptions)
            => builder.AddJwtCookie(JwtCookieAuthenticationDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddJwtCookie(this AuthenticationBuilder builder, string authenticationScheme, Action<JwtCookieAuthenticationOptions> configureOptions)
            => builder.AddJwtCookie(authenticationScheme, displayName: null, configureOptions: configureOptions);

        public static AuthenticationBuilder AddJwtCookie(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<JwtCookieAuthenticationOptions> configureOptions)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<JwtCookieAuthenticationOptions>, PostConfigureJwtCookieAuthenticationOptions>());
            builder.AddScheme<JwtCookieAuthenticationOptions, JwtCookieAuthenticationHandler>(authenticationScheme, displayName, configureOptions);
            return builder;
        }
    }
}
