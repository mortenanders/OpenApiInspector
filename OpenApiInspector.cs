using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Services;
using Microsoft.OpenApi.Validations;

namespace OpenApiInspector
{
    public class OpenApiInspector : OpenApiVisitorBase, IValidationContext
    {
        private readonly OpenApiValidationRuleSet _ruleSet;
        private readonly IList<OpenApiValidatorError> _errors = new List<OpenApiValidatorError>();

        public OpenApiInspector(OpenApiValidationRuleSet ruleSet = null)
        {
            _ruleSet = ruleSet;
        }

        public IEnumerable<OpenApiValidatorError> Errors
        {
            get
            {
                return _errors;
            }
        }

        public void AddError(OpenApiValidatorError error)
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
                return;  // Required fields should be checked by higher level objects
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
                    rule.Evaluate(this as IValidationContext, item);
                }
            }

        }

        public override void Visit(OpenApiPaths item) => Validate(item);
        public override void Visit(OpenApiSchema item) => Validate(item);
        public override void Visit(OpenApiResponses item) => Validate(item);

    }

    public static class ValidationContextExtensions
    {
        public static void CreateError(this IValidationContext context, string ruleName, string message)
        {
            OpenApiValidatorError error = new OpenApiValidatorError(ruleName, context.PathString, message);
            context.AddError(error);
        }
    }
}