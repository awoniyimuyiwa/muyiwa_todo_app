using Microsoft.AspNetCore.Builder;

namespace Infrastructure.Extensions
{
    /// <summary>
    /// IApplicationBuilder extensions
    /// </summary>
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseInfrastructureDatabaseErrorPage(this IApplicationBuilder applicationBuilder)
        {
            return applicationBuilder.UseDatabaseErrorPage();
        }
    }
}
