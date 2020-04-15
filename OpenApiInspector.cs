using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Services;

namespace OpenApiInspector
{
    public enum OpenApiInspectorErrorType { Route, Payload }
    public class OpenApiInspector : OpenApiVisitorBase
    {
        private readonly OpenApiValidationRuleSet _ruleSet;
        private readonly IList<ValidationError> _errors = new List<ValidationError>();

        public OpenApiInspector(OpenApiValidationRuleSet ruleSet = null)
        {
            _ruleSet = ruleSet;
        }

        public IEnumerable<ValidationError> Errors
        {
            get
            {
                return _errors;
            }
        }

        public void AddError(ValidationError error)
        {
            _errors.Add(error);
        }

        private void Validate<T>(T item)
        {
            var type = typeof(T);

            Validate(item, type);
        }

        private void Validate(object item, Type type)
        {
            if (item == null)
            {
                return;
            }

            // Validate unresolved references as references
            var potentialReference = item as IOpenApiReferenceable;
            if (potentialReference != null && potentialReference.UnresolvedReference)
            {
                type = typeof(IOpenApiReferenceable);
            }

            var rules = _ruleSet.FindRules(type);
            if (rules != null)
            {
                foreach (var rule in rules)
                {
                    rule.Evaluate(this, item);
                }
            }

        }

        public void CreateError(ValidationErrorCategory validationErrorCategory, string ruleName, string message)
        {
            AddError(new ValidationError(ruleName, PathString, message, validationErrorCategory));
        }

        public override void Visit(OpenApiPaths item) => Validate(item);
        public override void Visit(OpenApiSchema item) => Validate(item);
        public override void Visit(OpenApiResponses item) => Validate(item);

    }

}