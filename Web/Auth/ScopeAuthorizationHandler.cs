using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Auth
{
    class ScopeAuthorizationHandler : AuthorizationHandler<ScopeRequirement>
    {
        readonly IHttpContextAccessor HttpContextAccessor;

        public ScopeAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// This handler has support for both cookie and bearer authentication schemes.
        /// 
        /// When cookie authentication scheme is used, only the id_token recieved from the IDP is used to create the authenticated user and its claims.
        /// id_token has only a sub claim to identify a user. 
        /// To get the client scope claim an accesss_token is required and has to be fetched separately.
        /// </summary>
        /// <param name="authorizationHandlerContext"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext authorizationHandlerContext, ScopeRequirement requirement)
        {
            if (!authorizationHandlerContext.User.Identity.IsAuthenticated) { return; }

            // When Bearer authentication scheme is used, the access_token containing scope claim
            // will be used to create the currently authenticated user and its cliams 
            var claims = authorizationHandlerContext.User.Claims;
            if (claims != null && claims.Any(
                claim => claim.Type.Equals("scope") &&
                claim.Value.Equals(requirement.ScopeIdentifier)))
            {
                authorizationHandlerContext.Succeed(requirement);
                return;
            }

            // When cookie authentication scheme (the deafult authetication scheme) is used, only the id_token recieved from the IDP
            // is used to create the authenticated user and its claims. id_token has only a sub claim to identify a user. 
            // To get the client's scope claim, the accesss_token is required and has to be fetched separately 
            // using identity model's token management services.
            var accessToken = await HttpContextAccessor.HttpContext.GetTokenAsync("access_token");
            if (accessToken == null) { return; }

            var handler = new JwtSecurityTokenHandler();
            var decodedAccessToken = handler.ReadJwtToken(accessToken);
            claims = decodedAccessToken.Claims;
            if (claims.Any(
                claim => claim.Type.Equals("scope") && 
                claim.Value.Equals(requirement.ScopeIdentifier)))
            {
                authorizationHandlerContext.Succeed(requirement);
            }
            
            return;
        }
    }

    class ScopeRequirement : IAuthorizationRequirement
    {
        public readonly string ScopeIdentifier;

        public ScopeRequirement(string scopeIdentifier)
        {
            ScopeIdentifier = scopeIdentifier;
        }
    }
}
