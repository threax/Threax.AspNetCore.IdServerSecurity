﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using System;
using Threax.AspNetCore.AuthCore;
using Threax.AspNetCore.IdServerAuth;
using Threax.AspNetCore.JwtCookieAuth;
using Threax.AspNetCore.UserBuilder;
using Threax.SharedHttpClient;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceExtensions
    {
        private static IdServerAuthOptions options; //Probably not the best idea to have this state, but this makes this way easier to use.

        /// <summary>
        /// Configure the services for id server authentication according to the convention established. There are 2 major roles an application can fulfill.
        /// The first is to act as a client, which means it has razor views that should be returned This will use jwts in cookies. 
        /// The other is as an api, which means it has controllers that return data. This will use jwts in bearer headers.
        /// An application can be both of these roles at once.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="optionsBuilder"></param>
        /// <returns></returns>
        public static IServiceCollection AddConventionalIdServerAuthentication(this IServiceCollection services, Action<IdServerAuthOptions> optionsBuilder)
        {
            if(optionsBuilder == null)
            {
                throw new InvalidOperationException("You must include an options builder");
            }

            options = new IdServerAuthOptions();
            optionsBuilder.Invoke(options);
            options.CookiePath = CookieUtils.FixPath(options.CookiePath);

            if(options.AppOptions == null)
            {
                throw new InvalidOperationException("You must provide the application specific options.");
            }

            if (String.IsNullOrEmpty(options.AppOptions.Authority))
            {
                throw new InvalidOperationException("You must provide an Authority (the url of the id server) in the options.");
            }

            if (String.IsNullOrEmpty(options.AppOptions.Scope))
            {
                throw new InvalidOperationException("You must provide a scope for the app.");
            }

            if (options.ActAsClient)
            {
                if (String.IsNullOrEmpty(options.AppOptions.ClientId))
                {
                    throw new InvalidOperationException("You must provide a client id for the app to use it as a client.");
                }
            }

            var authBuilder = services.AddAuthentication(o =>
            {
                if (options.ActAsClient)
                {
                    o.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                }

                //If the user provides a default scheme, override it here, leave this last so it will always override.
                if (options.DefaultScheme != null)
                {
                    o.DefaultScheme = options.DefaultScheme;
                }
            });

            services.AddThreaxSharedHttpClient();
            
            if (options.ActAsClient)
            {
                authBuilder.AddJwtCookie(AuthCoreSchemes.Cookies, o =>
                {
                    o.Authority = options.AppOptions.Authority;
                    o.ClientId = options.AppOptions.ClientId;
                    o.ClientSecret = options.AppOptions.ClientSecret;
                    o.CookiePath = options.CookiePath;
                    o.ChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                    o.AccessDeniedPath = options.AccessDeniedPath;
                    o.ValidateAudience = true;
                    o.ValidAudiences = new String[] { options.AppOptions.Scope };
                    o.ValidateLifetime = true;
                    o.ClockSkew = options.ClockSkew;
                    o.Events = new JwtCookieEvents()
                    {
                        OnValidatePrincipal = c => c.BuildUserWithRequestServices()
                    };
                    options.CustomizeCookies?.Invoke(o);
                })
                .AddOpenIdConnect(o =>
                {
                    o.SignInScheme = AuthCoreSchemes.Cookies;
                    o.Authority = options.AppOptions.Authority;
                    o.ClientSecret = options.AppOptions.ClientSecret;
                    o.ClientId = options.AppOptions.ClientId;
                    o.ResponseType = "code id_token";
                    o.SaveTokens = true;
                    o.UseTokenLifetime = true;
                    o.GetClaimsFromUserInfoEndpoint = false;
                    o.CallbackPath = options.CallbackPath;
                    o.SignedOutCallbackPath = options.SignedOutCallbackPath;
                    o.RemoteSignOutPath = options.RemoteSignOutPath;
                    o.SignedOutRedirectUri = options.SignedOutRedirectUri;
                    o.TokenValidationParameters.ValidAudiences = new String[] { options.AppOptions.Scope };
                    o.TokenValidationParameters.ValidateAudience = true;

                    o.Scope.Clear();
                    o.Scope.Add("openid");
                    o.Scope.Add("profile");
                    o.Scope.Add("offline_access");
                    o.Scope.Add(options.AppOptions.Scope);

                    if (options.AppOptions.AdditionalScopes != null)
                    {
                        foreach (var scope in options.AppOptions.AdditionalScopes)
                        {
                            o.Scope.Add(scope);
                        }
                    }

                    o.Events.OnSignedOutCallbackRedirect = async c =>
                    {
                        //When signed out endpoint is called, sign out of cookie auth
                        await c.HttpContext.SignOutAsync(JwtCookieAuthenticationDefaults.AuthenticationScheme);
                    };

                    options.CustomizeOpenIdConnect?.Invoke(o);
                });
            }

            if (options.ActAsApi)
            {
                var bearerEvents = new JwtBearerAuthenticationEvents(Microsoft.IdentityModel.Tokens.SecurityAlgorithms.RsaSha256, options.ClockSkew)
                {
                    OnAuthorizeUser = c => c.BuildUserWithRequestServices()
                };

                authBuilder.AddJwtBearer(AuthCoreSchemes.Bearer, o =>
                {
                    o.Authority = options.AppOptions.Authority;
                    o.RequireHttpsMetadata = true;
                    o.Audience = options.AppOptions.Scope;
                    o.Events = new JwtBearerEvents()
                    {
                        OnTokenValidated = bearerEvents.TokenValidated,
                        OnMessageReceived = s =>
                        {
                            s.Token = s.HttpContext.Request.Headers["bearer"];
                            return System.Threading.Tasks.Task.FromResult(0);
                        }
                    };
                    options.CustomizeBearer?.Invoke(o);
                });
            }

            return services;
        }

        /// <summary>
        /// Add the conventional id server metadata mvc controllers. Be sure to call this after AddConventionalIdServerAuthentication
        /// so the options are setup.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IMvcBuilder AddConventionalIdServerMvc(this IMvcBuilder builder)
        {
            if(options == null)
            {
                throw new InvalidOperationException("You must call AddIdServerAuthentication before calling AddIdServerMetadata so the options will be setup.");
            }

            if (options.EnableIdServerMetadata)
            {
                builder.AddIdServerMetadata(o =>
                {
                    o.Enabled = options.EnableIdServerMetadata;
                    if (options.ActAsApi)
                    {
                        o.CreateConventionalResource(options.AppOptions.Scope, options.AppOptions.DisplayName);
                    }
                    if (options.ActAsClient)
                    {
                        o.CreateConventionalClient(options.AppOptions.ClientId, options.AppOptions.DisplayName, options.AppOptions.Scope, options.SignedOutCallbackPath, options.AppOptions.AdditionalScopes);
                    }
                    if (options.CreateClientCredentialsMetadata)
                    {
                        o.CreateConventionalClientCredentials(options.AppOptions.ClientId, options.AppOptions.DisplayName, options.AppOptions.ClientCredentialsScopes);
                    }
                });
            }

            return builder;
        }
    }
}
