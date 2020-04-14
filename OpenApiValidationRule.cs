using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Validations;

namespace OpenApiInspector{
    public abstract class OpenApiValidationRule
    {
        public abstract Type ElementType { get; }

        public abstract void Evaluate(IValidationContext context, object item);
    }

    public class OpenApiValidationRule<T> : OpenApiValidationRule where T : IOpenApiElement
    {
        private readonly Action<IValidationContext, T> _validate;
        public OpenApiValidationRule(Action<IValidationContext, T> validate)
        {
            _validate = validate;
        }

        public override Type ElementType
        {
            get { return typeof(T); }
        }

        public override void Evaluate(IValidationContext context, object item)
        {
            if (item == null)
            {
                return;
            }
            T typedItem = (T)item;
            this._validate(context, typedItem);
        }
    }

    public sealed class OpenApiValidationRuleSet : IEnumerable<OpenApiValidationRule>
    {
        private IDictionary<Type, IList<OpenApiValidationRule>> _rules = new Dictionary<Type, IList<OpenApiValidationRule>>();

        private IList<OpenApiValidationRule> _emptyRules = new List<OpenApiValidationRule>();

        public IList<OpenApiValidationRule> FindRules(Type type)
        {
            IList<OpenApiValidationRule> results = null;
            _rules.TryGetValue(type, out results);
            return results;
        }

        public OpenApiValidationRuleSet()
        {
        }

        public IList<OpenApiValidationRule> Rules
        {
            get
            {
                return _rules.Values.SelectMany(v => v).ToList();
            }
        }

        public void Add(OpenApiValidationRule rule)
        {
            if (!_rules.ContainsKey(rule.ElementType))
            {
                _rules[rule.ElementType] = new List<OpenApiValidationRule>();
            }

            _rules[rule.ElementType].Add(rule);
        }

        public IEnumerator<OpenApiValidationRule> GetEnumerator()
        {
            foreach (var ruleList in _rules.Values)
            {
                foreach (var rule in ruleList)
                {
                    yield return rule;
                }
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}