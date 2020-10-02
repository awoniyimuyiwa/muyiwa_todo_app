using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Web.Auth
{
    class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        readonly IConfiguration Configuration;

        public CustomCookieAuthenticationEvents(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public override Task RedirectToLogin(RedirectContext<CookieAuthenticationOptions> redirectContext)
        {
            if (redirectContext.Request.Path.StartsWithSegments("/api"))
            {
                redirectContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }
            else
            {
                redirectContext.Response.Redirect(redirectContext.RedirectUri);
            }

            return Task.CompletedTask;
        }

        public override Task RedirectToAccessDenied(RedirectContext<CookieAuthenticationOptions> redirectContext)
        {
            redirectContext.Response.StatusCode = StatusCodes.Status403Forbidden;

            return Task.CompletedTask;
        }

        public override Task RedirectToReturnUrl(RedirectContext<CookieAuthenticationOptions> redirectContext)
        {
            var defaultRedirectUri = "/swagger";
            var redirectUri = redirectContext.RedirectUri ?? defaultRedirectUri;

            try
            {
                var uri = new Uri(redirectUri);
                if (uri.Host != Configuration.GetValue<string>("AppClientHost") ||
                    uri.Port.ToString() != Configuration.GetValue<string>("AppClientPort"))
                {
                    redirectUri = defaultRedirectUri;
                }
            }
            catch (Exception)
            {
                redirectUri = defaultRedirectUri;
            }

            redirectContext.Response.Redirect(redirectUri);

            return Task.CompletedTask;
        }
    }
}
