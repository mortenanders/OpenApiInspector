using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Validations;

namespace OpenApiInspector
{
    public static class OpenApiPathRules
    {
        public static OpenApiValidationRule<OpenApiPaths> RouteSegmentsMustBePlural =>
            new OpenApiValidationRule<OpenApiPaths>(
                (context, paths) =>
                {
                    foreach(var path in paths){
                        AnalyzePath(context, path.Key);
                    }
                });

        private static void AnalyzePath(IValidationContext context, string path)
        {
            var segments = path.Split("/");
            foreach(var segment in segments){
                if (!(segment.StartsWith("{") && segment.EndsWith("}"))){
                    if (!string.IsNullOrEmpty(segment) && !segment.EndsWith("s")){
                        context.CreateError(nameof(RouteSegmentsMustBePlural), string.Format("In the route '{1}', the segment '{0}' is not plural", segment, path));
                    }
                }
            }
        }
    }

    

}