using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using System.Linq;
using Web.MiddleWares;

namespace Web.Extensions
{
    /// <summary>
    /// IApplicationBuilder extensions
    /// </summary>
    public static class IApplicationBuilderExtensions
    {
        /// <summary>
        ///  Set up global exception handling for the app using a middleware
        /// </summary>
        /// <param name="appBuilder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder appBuilder)
        {
            appBuilder.UseMiddleware<ExceptionHandlerMiddleware>();

            return appBuilder;
        }
    }
}
