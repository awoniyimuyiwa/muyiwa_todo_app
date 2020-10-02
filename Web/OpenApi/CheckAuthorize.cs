using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;
using Web.Utils;

namespace Web.OpenApi
{
    internal class CheckAuthorize : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasAuthorizeAttribute =
                context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any()
                || context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();
            
            if (!hasAuthorizeAttribute) { return; }

            var hasHttpMethodAttribute = context.MethodInfo.GetCustomAttributes(true).OfType<HttpMethodAttribute>().Any();
            if (!hasHttpMethodAttribute) { return; }

            var httpMethodAttribute = context.MethodInfo.GetCustomAttributes(true).OfType<HttpMethodAttribute>().First();
            if (httpMethodAttribute == null || httpMethodAttribute.Template == null) { return; }

            if (httpMethodAttribute.Template.StartsWith("worker"))
            {
                // For /worker endpoints for server to server operations
                var oauth2ClientCredentialsScheme = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = Constants.SwaggerSecurityDefinitionNames.OAuth2ClientCredentials
                    }
                };

                operation.Security = new List<OpenApiSecurityRequirement>    
                {
                    new OpenApiSecurityRequirement     
                    {
                        [oauth2ClientCredentialsScheme] = new[]     
                        {
                            Constants.AuthorizationPolicyNames.WorkerTodoItemScope 
                        }    
                    }
                };
            }
            else if (httpMethodAttribute.Template.StartsWith("native"))
            {
                // For /native endpoints
                var oauth2AuthCodeScheme = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = Constants.SwaggerSecurityDefinitionNames.OAuth2AuthCodeNative
                    }
                };

                operation.Security = new List<OpenApiSecurityRequirement>    
                {
                    new OpenApiSecurityRequirement    
                    {
                        [oauth2AuthCodeScheme] = new[]    
                        {
                            "openid", "profile",        
                            Constants.AuthorizationPolicyNames.DeleteTodoItemScope,    
                            Constants.AuthorizationPolicyNames.ReadTodoItemScope,
                            Constants.AuthorizationPolicyNames.WriteTodoItemScope
                        } 
                    }    
                };
            }
            else if (httpMethodAttribute.Template.StartsWith("admin/native"))
            {
                // For /admin/native endpoints
                var oauth2AuthCodeScheme = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = Constants.SwaggerSecurityDefinitionNames.OAuth2AuthCodeAdminNative
                    }
                };

                operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        [oauth2AuthCodeScheme] = new[]
                        {
                            "openid", "profile", Constants.CustomClaimTypes.Permission
                        }
                    }
                };
            }
        }
    }
}
