using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Validations;

namespace OpenApiInspector
{
    public static class OpenApiPropertyRules
    {
        public static ValidationRule<OpenApiSchema> PropertiesMustBePascalCased =>
            new ValidationRule<OpenApiSchema>(
                (context, schema) =>
                {
                    foreach (var property in schema.Properties)
                    {
                        var firstLetter = property.Key.Substring(0, 1);
                        if (firstLetter != firstLetter.ToUpper())
                        {
                            context.Enter(property.Key);
                            context.CreateError(ValidationErrorCategory.Payload, nameof(PropertiesMustBePascalCased), string.Format("The property '{0}' is not Pascalcased", property.Key));
                            context.Exit();
                        }
                    }
                });
    }
}