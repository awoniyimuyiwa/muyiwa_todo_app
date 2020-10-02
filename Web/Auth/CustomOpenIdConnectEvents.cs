using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Threading.Tasks;

namespace Web.Auth
{
    class CustomOpenIdConnectEvents : OpenIdConnectEvents
    {
        public override async Task TicketReceived(TicketReceivedContext context)
        {
            await base.TicketReceived(context);

            // Add custom claims to context.Principal here
        }
    }
}
