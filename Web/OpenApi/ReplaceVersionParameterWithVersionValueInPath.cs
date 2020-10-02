using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Web.OpenApi
{
    internal class ReplaceVersionParameterWithVersionValueInPath : IDocumentFilter
    {
        public void Apply(OpenApiDocument apiDoc, DocumentFilterContext context)
        {
            var paths = new OpenApiPaths();
            foreach(var path in apiDoc.Paths)
            {
                var key = path.Key.Replace("v{version}", apiDoc.Info.Version);
                paths.Add(key, path.Value);
            }

            apiDoc.Paths = paths;
        }
    }
}
