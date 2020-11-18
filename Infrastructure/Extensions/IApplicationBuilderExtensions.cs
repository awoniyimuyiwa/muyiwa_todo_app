using Microsoft.AspNetCore.Builder;

namespace Infrastructure.Extensions
{
    /// <summary>
    /// IApplicationBuilder extensions
    /// </summary>
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseInfrastructureMigrationsEndPoint(this IApplicationBuilder applicationBuilder)
        {
            return applicationBuilder.UseMigrationsEndPoint();
        }
    }
}
