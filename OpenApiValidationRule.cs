using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Interfaces;

namespace OpenApiInspector
{
    public abstract class ValidationRule
    {
        public abstract Type ElementType { get; }

        public abstract void Evaluate(OpenApiInspector context, object item);
    }

    public class ValidationRule<T> : ValidationRule where T : IOpenApiElement
    {
        private readonly Action<OpenApiInspector, T> _validate;
        public ValidationRule(Action<OpenApiInspector, T> validate)
        {
            _validate = validate;
        }

        public override Type ElementType
        {
            get { return typeof(T); }
        }

        public override void Evaluate(OpenApiInspector context, object item)
        {
            if (item == null)
            {
                return;
            }
            T typedItem = (T)item;
            this._validate(context, typedItem);
        }
    }

    public sealed class OpenApiValidationRuleSet : IEnumerable<ValidationRule>
    {
        private IDictionary<Type, IList<ValidationRule>> _rules = new Dictionary<Type, IList<ValidationRule>>();

        private IList<ValidationRule> _emptyRules = new List<ValidationRule>();

        public IList<ValidationRule> FindRules(Type type)
        {
            IList<ValidationRule> results = null;
            _rules.TryGetValue(type, out results);
            return results;
        }

        public OpenApiValidationRuleSet()
        {
        }

        public IList<ValidationRule> Rules
        {
            get
            {
                return _rules.Values.SelectMany(v => v).ToList();
            }
        }

        public void Add(ValidationRule rule)
        {
            if (!_rules.ContainsKey(rule.ElementType))
            {
                _rules[rule.ElementType] = new List<ValidationRule>();
            }

            _rules[rule.ElementType].Add(rule);
        }

        public IEnumerator<ValidationRule> GetEnumerator()
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