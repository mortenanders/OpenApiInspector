using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Validations;
using System.Linq;

namespace OpenApiInspector
{
    public static class OpenApiPathRules
    {
        private static IEnumerable<string> ExtractNonParameterSegments(string path)
        {
            return path.Split("/").Select(segment => segment.Trim()).Where(segment => !string.IsNullOrEmpty(segment) && (!(segment.StartsWith("{") && segment.EndsWith("}"))));
        }

        private static IEnumerable<string> ExtractParameterSegments(string path)
        {
            return path.Split("/").Select(segment => segment.Trim()).Where(segment => !string.IsNullOrEmpty(segment) && (segment.StartsWith("{") && segment.EndsWith("}")));
        }

        public static OpenApiValidationRule<OpenApiPaths> RouteSegmentsMustBePlural =>
            new OpenApiValidationRule<OpenApiPaths>(
                (context, paths) =>
                {
                    foreach (var path in paths)
                    {
                        foreach (var segment in ExtractNonParameterSegments(path.Key))
                        {
                            if (!segment.EndsWith("s"))
                            {
                                context.CreateError(nameof(RouteSegmentsMustBePlural), string.Format("In the route '{1}', the segment '{0}' is not plural", segment, path.Key));
                            }
                        }
                    }
                });

        public static OpenApiValidationRule<OpenApiPaths> RoutesMustBeLowercased =>
            new OpenApiValidationRule<OpenApiPaths>(
                (context, paths) =>
                {
                    foreach (var path in paths)
                    {
                        foreach (var segment in ExtractNonParameterSegments(path.Key))
                        {
                            if (segment.ToLower() != segment)
                            {
                                context.CreateError(nameof(RoutesMustBeLowercased), string.Format("In the route '{1}', the segment '{0}' is not lowercased", segment, path.Key));
                            }
                        }
                    }
                });

        public static OpenApiValidationRule<OpenApiPaths> RoutesMustNotUseDelimiters =>
            new OpenApiValidationRule<OpenApiPaths>(
                (context, paths) =>
                {
                    foreach (var path in paths)
                    {
                        foreach (var segment in ExtractNonParameterSegments(path.Key))
                        {
                            if (new Regex("[^a-z]+", RegexOptions.IgnoreCase).IsMatch(segment))
                            {
                                context.CreateError(nameof(RoutesMustNotUseDelimiters), string.Format("In the route '{1}', the segment '{0}' contains non-alphanumeric characters", segment, path.Key));
                            }
                        }
                    }
                });

        public static OpenApiValidationRule<OpenApiPaths> RouteParametersMustBePascalcased =>
            new OpenApiValidationRule<OpenApiPaths>(
                (context, paths) =>
                {
                    foreach (var path in paths)
                    {
                        foreach (var segment in ExtractParameterSegments(path.Key))
                        {
                            var secondLetter = segment.Substring(1, 1);
                            if (secondLetter != secondLetter.ToUpper())
                            {
                                context.CreateError(nameof(RouteParametersMustBePascalcased), string.Format("In the route '{1}', the parameter '{0}' is not pascalcased", segment, path.Key));
                            }
                        }
                    }
                });
    }



}