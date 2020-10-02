using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace Web.OpenApi
{
    internal class CheckAntiforgery : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var methodInfo = context.MethodInfo;
            var methodAttributes = methodInfo.GetCustomAttributes(true);
            var hasAntiforgeryAttribute =
                (methodInfo.DeclaringType.GetCustomAttributes(true).OfType<ValidateAntiForgeryTokenAttribute>().Any() 
                && !methodAttributes.OfType<IgnoreAntiforgeryTokenAttribute>().Any())
                || methodAttributes.OfType<ValidateAntiForgeryTokenAttribute>().Any();

            if (!hasAntiforgeryAttribute) { return; }

            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            operation.Parameters.Add(new OpenApiParameter
            {
                Description = "After authenticating at /auth/login, retrieve the value of csrf_token from the json response or the XSRF-TOKEN cookie that will be set in your browser when a request is made to /api/identity",
                In = ParameterLocation.Header,
                Name = "X-XSRF-TOKEN",
                Required = true
            });
        }
    }
}
