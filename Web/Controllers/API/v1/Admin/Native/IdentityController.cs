using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web.Services.Abstracts;
using Web.Utils;

namespace Web.Controllers.API.v1.Admin.Native
{
    /// <summary>
    /// Handles identity requests from non-browser native clients that can securely store access tokens 
    /// for users with admin permissions (assigned to them on the identity server) for this API, 
    /// </summary>
    /// <remarks>
    /// The endpoints on this contoller use the bearer authentication scheme and no csrf protection. 
    /// </remarks>
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiVersion("1.0")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api")] // The /admin/native path should have been included in this attribute but it was moved to actions to get past swagger's unique method/path restriction
    [Route("api/v{version:apiVersion}")]
    public class IdentityController : ControllerBase
    {
        readonly IOidcUserInfoService OidcUserInfoService;

        /// <summary>
        /// Initializes fields
        /// </summary>
        /// <param name="oidcUserInfoService"></param>
        public IdentityController(IOidcUserInfoService oidcUserInfoService)
        {
            OidcUserInfoService = oidcUserInfoService;
        }

        /// <summary>
        /// Returns a list of the currently authenticated user's claims.
        /// </summary>
        /// <response code="200">When there are no errors</response>
        /// <response code="401">When authentication fails</response>
        [HttpGet("admin/native/identity")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Get()
        {
            var accessToken = await HttpContext.GetTokenAsync(JwtBearerDefaults.AuthenticationScheme, "access_token");
            var idpUserClaims = await OidcUserInfoService.GetClaims(accessToken);

            // Include only non-sensitive claims for security reasons.
            IEnumerable<object> result = from claim in idpUserClaims 
                                            where claim.Type == JwtClaimTypes.FamilyName ||
                                            claim.Type == JwtClaimTypes.GivenName ||
                                            claim.Type == JwtClaimTypes.MiddleName ||
                                            claim.Type == JwtClaimTypes.Name ||
                                            claim.Type == JwtClaimTypes.NickName ||
                                            claim.Type == JwtClaimTypes.PreferredUserName ||
                                            claim.Type == Constants.CustomClaimTypes.Permission
                                         select new { claim.Type, claim.Value };

            return Ok(result);
        }
    }
}
