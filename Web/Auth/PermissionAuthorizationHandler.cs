using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;
using Web.Services.Abstracts;
using Web.Utils;

namespace Web.Auth
{
    class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        readonly IOidcUserInfoService OidcUserInfoService;
        readonly IHttpContextAccessor HttpContextAccessor;

        public PermissionAuthorizationHandler(
            IOidcUserInfoService oidcUserInfoService, IHttpContextAccessor httpContextAccessor)
        {
            OidcUserInfoService = oidcUserInfoService;
            HttpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// This handler has support for both cookie and bearer authentication schemes.        
        /// </summary>
        /// <param name="authorizationHandlerContext"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext authorizationHandlerContext, PermissionRequirement requirement)
        {
            if (!authorizationHandlerContext.User.Identity.IsAuthenticated) { return; }

            // When cookie authentication scheme (the default authentication scheme) is used, only the id_token recieved from the IDP
            // is used to create the authenticated user and its claims. id_token has only a sub claim to identify a user. 
            // To get other user claims, the accesss_token is required and has to be fetched separately 
            // using identity model's token management services.
            var accessToken = await HttpContextAccessor.HttpContext.GetTokenAsync("access_token");
            if (accessToken == null) 
            {
                // Probably bearer authentication scheme was used
                accessToken = await HttpContextAccessor.HttpContext.GetTokenAsync(JwtBearerDefaults.AuthenticationScheme, "access_token");
                if (accessToken == null) { return; }
            }

            // Fetch other user claims
            var claims = await OidcUserInfoService.GetClaims(accessToken);

            if (claims.Any(
                claim => claim.Type.Equals(Constants.CustomClaimTypes.Permission) && 
                claim.Value.Equals(requirement.PermissionIdentifier)))
            {
                authorizationHandlerContext.Succeed(requirement);
            }

            return;
        }
    }

    class PermissionRequirement : IAuthorizationRequirement
    {
        public readonly string PermissionIdentifier;

        public PermissionRequirement(string permissionIdentifier)
        {
            PermissionIdentifier = permissionIdentifier;
        }
    }
}
