using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Utils;

namespace Web.OpenApi
{
    internal static class SecuritySchemes
    {
        public static OpenApiSecurityScheme OAuth2AuthCodeSchemeNative(IConfiguration configuration) => new OpenApiSecurityScheme
        {
            BearerFormat = "JWT",
            Description = "JWT bearer authorization with an access token retrieved using OAuth2 authorization code grant",
            In = ParameterLocation.Header,
            Name = "Authorization",
            OpenIdConnectUrl = new Uri(configuration.GetValue<string>("AppOidcAuthority")),
            Scheme = "Bearer",
            Type = SecuritySchemeType.OAuth2,
           
            Flows = new OpenApiOAuthFlows
            {
                AuthorizationCode = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri($"{configuration.GetValue<string>("AppOidcAuthority")}/connect/authorize"),
                    TokenUrl = new Uri($"{configuration.GetValue<string>("AppOidcAuthority")}/connect/token"),
                    Scopes = new Dictionary<string, string>
                    {
                        { "openid", "Permission to retrieve and use your open-id token" },
                        { "profile", "Permission to access your profile information" },                        
                        { Constants.AuthorizationPolicyNames.DeleteTodoItemScope, "Permission to delete your todo-items" },
                        { Constants.AuthorizationPolicyNames.ReadTodoItemScope, "Permission to read your todo-items" },
                        { Constants.AuthorizationPolicyNames.WriteTodoItemScope, "Permission to write your todo-items" },                       
                    }
                }
            }
        };

        public static OpenApiSecurityScheme OAuth2AuthCodeSchemeAdminNative(IConfiguration configuration) => new OpenApiSecurityScheme
        {
            BearerFormat = "JWT",
            Description = "JWT bearer authorization with an access token retrieved using OAuth2 authorization code grant",
            In = ParameterLocation.Header,
            Name = "Authorization",
            OpenIdConnectUrl = new Uri(configuration.GetValue<string>("AppOidcAuthority")),
            Scheme = "Bearer",
            Type = SecuritySchemeType.OAuth2,

            Flows = new OpenApiOAuthFlows
            {
                AuthorizationCode = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri($"{configuration.GetValue<string>("AppOidcAuthority")}/connect/authorize"),
                    TokenUrl = new Uri($"{configuration.GetValue<string>("AppOidcAuthority")}/connect/token"),
                    Scopes = new Dictionary<string, string>
                    {
                        { "openid", "Permission to retrieve and use your open-id token" },
                        { "profile", "Permission to access your profile information" },
                        { Constants.CustomClaimTypes.Permission, "Permission to access your permissions" }                       
                    }
                }
            }
        };

        public static OpenApiSecurityScheme OAuth2ClientCredentialsScheme(IConfiguration configuration) => new OpenApiSecurityScheme
        {
            BearerFormat = "JWT",
            Description = "JWT bearer authorization with an access token retrieved using OAuth2 client credentials grant ",
            In = ParameterLocation.Header,
            Name = "Authorization",
            OpenIdConnectUrl = new Uri(configuration.GetValue<string>("AppOidcAuthority")),
            Scheme = "Bearer",
            Type = SecuritySchemeType.OAuth2,

            Flows = new OpenApiOAuthFlows
            {
                ClientCredentials = new OpenApiOAuthFlow
                {
                    //AuthorizationUrl = new Uri($"{configuration.GetValue<string>("AppOidcAuthority")}/connect/authorize"),
                    TokenUrl = new Uri($"{configuration.GetValue<string>("AppOidcAuthority")}/connect/token"),
                    Scopes = new Dictionary<string, string>
                    {                        
                        { Constants.AuthorizationPolicyNames.WorkerTodoItemScope, "Permission to access all todo-items in the API" }
                    }
                }
            }
        };
    }
}
