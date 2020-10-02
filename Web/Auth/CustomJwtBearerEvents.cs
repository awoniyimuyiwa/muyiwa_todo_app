using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Web.Auth
{
    class CustomJwtBearerEvents : JwtBearerEvents
    {
        public override async Task AuthenticationFailed(AuthenticationFailedContext context)
        {
            await base.AuthenticationFailed(context);
            //context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            //return Task.CompletedTask;
        }

        public override async Task TokenValidated(TokenValidatedContext context)
        {
            // Add custom local claims here

            await base.TokenValidated(context);
        }
    }
}
