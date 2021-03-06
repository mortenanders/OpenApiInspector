using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Validations;
using System.Linq;

namespace OpenApiInspector
{
    public static class OpenApiResponseRules
    {
        public static ValidationRule<OpenApiResponses> MustUseCollectionEnvelope =>
            new ValidationRule<OpenApiResponses>(
                (context, responses) =>
                {
                    var okResponses = responses.Where(r => r.Key == "200")
                        .SelectMany(r => r.Value.Content.Values)
                        .Select(m => m.Schema);

                    foreach (var schema in okResponses)
                    {
                        if (schema.Type == "array")
                        {
                            context.CreateError(ValidationErrorCategory.Payload, nameof(MustUseCollectionEnvelope), "If the intent is to return a list, the collection envelope should be used");
                        }

                    }

                });
    }
}