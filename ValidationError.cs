using Microsoft.OpenApi.Validations;

namespace OpenApiInspector
{
    public enum ValidationErrorCategory { Route, Payload }
    public class ValidationError : OpenApiValidatorError
    {
        public ValidationErrorCategory ValidationErrorCategory { get; }
        public ValidationError(string ruleName, string pointer, string message, ValidationErrorCategory validationErrorCategory) : base(ruleName, pointer, message)
        {
            ValidationErrorCategory = validationErrorCategory;
        }

        public override string ToString() => string.Format("{0}: {1}", ValidationErrorCategory.ToString(), Message + (!string.IsNullOrEmpty(Pointer) ? " [" + Pointer + "]" : ""));
    }
}