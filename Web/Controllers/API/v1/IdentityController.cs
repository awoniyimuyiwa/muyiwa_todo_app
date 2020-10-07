using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web.Services.Abstracts;
using Web.Utils;

namespace Web.Controllers.API.v1
{
    /// <summary>
    /// Handles identity requests from browser based clients/single page applications
    /// </summary>
    /// <remarks> 
    /// Endpoints exposed by this controller use cookie authentication scheme
    /// </remarks>
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    [ApiVersion("1.0")]
    [Authorize]
    [Route("api/identity")]
    [Route("api/v{version:apiVersion}/identity")]
    public class IdentityController : ControllerBase
    {
        readonly IOidcUserInfoService OidcUserInfoService;
        readonly IAntiforgery Antiforgery;

        /// <summary>
        /// Initializes fields
        /// </summary>
        /// <param name="oidcUserInfoService"></param>
        /// <param name="antiforgery"></param>
        public IdentityController(IOidcUserInfoService oidcUserInfoService, IAntiforgery antiforgery)
        {
            OidcUserInfoService = oidcUserInfoService;
            Antiforgery = antiforgery;
        }

        /// <summary>
        /// Returns a list of the currently authenticated user's claims.
        /// </summary>
        /// <response code="200">When there are no errors</response>
        /// <response code="401">When authentication fails</response>
        /// <remarks>
        /// 1) SPAs should have an in memory state they check first upon launching to see if the users's claims are in memory.
        /// If the claims are not yet in memory, SPAs should immediately call this endpoint to retrieve the user's claims.
        /// 
        /// 2) When SPAs get a 401 response from this endpoint or any other endpoint, 
        /// they should immediately redirect the browser to /auth/login while passing along an optional redirect_uri query string parameter. 
        /// After the user authenticates, the user's browser will be redirected to the SPA and the SPA should repeat step 1.
        /// 
        /// 3) When SPAs get a 200 response from this endpoint, they should save the json repsonse containing the claims in memory
        /// and set an in memory boolean that indicates that the user is authenticated.  
        /// 
        /// 4) SPAs should redirect to /auth/logout to terminate a user's session 
        /// </remarks>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Get()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
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

            // Call csrf service to generate and store csrf token in cookies if they don't already exist
            var tokens = Antiforgery.GetAndStoreTokens(HttpContext);

            if (tokens != null)
            {
                // Add claims that make it possible to also send csrf token manually in request header or form request
                result = result.Union(new List<object> { 
                   new { Type = "csrf_token", Value = tokens.RequestToken }, 
                   new { Type = "csrf_form_field_name", Value = tokens.FormFieldName } 
                });

                // Add support for some Javascript Http clients like Angular and Axios that automaticatlly fetch XSRF-TOKEN cookie and send it as X-XSRF-TOKEN header in each request 
                HttpContext.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken, new CookieOptions { HttpOnly = false });
            }

            return Ok(result);
        }
    }
}
