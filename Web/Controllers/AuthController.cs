using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Services.Abstracts;
using Web.ViewModels;
using System.Threading.Tasks;

namespace Web.Controllers
{
    /// <summary>
    /// Handles OIDC authentication for SPAs or web apps
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("auth")]
    public class AuthController : Controller
    {
        /// <summary>
        /// Initializes authentication for a user
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("login")]
        public IActionResult Login([FromQuery(Name = "redirect_uri")] string redirectUri = "/swagger")
        {
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                RedirectUri = redirectUri
            };

            return Challenge(authProperties, OpenIdConnectDefaults.AuthenticationScheme);
        }

        /// <summary>
        /// Confirms termination of an authenticated user's session
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("logout")]
        public async Task<IActionResult> GetLogout([FromQuery(Name = "redirect_uri")] string redirectUri = "/swagger")
        {
            var logoutViewModel = new LogoutViewModel
            {
                RedirectUri = redirectUri
            };

            var viewModel = new LayoutViewModel<LogoutViewModel>();
            await viewModel.Initialize(logoutViewModel, "Logout", HttpContext);
           
            return View("Logout", viewModel);
        }

        /// <summary>
        /// Terminates an authenticated user's session
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost("logout")]
        [ValidateAntiForgeryToken]
        public IActionResult PostLogout([FromQuery(Name = "redirect_uri")] string redirectUri = "/swagger")
        {
            var authProperties = new AuthenticationProperties
            {
                RedirectUri = redirectUri
            };

            return SignOut(
                authProperties,
                CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme);
        }
    }
}
