using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Vit.Linq.Filter.ComponentModel
{
    /// <summary>
    /// This class is used to define a hierarchical filter for a given collection. This type can be serialized/deserialized by JSON.NET without needing to modify the data structure from QueryBuilder.
    /// </summary>
    public class FilterRule : FilterRuleBase<FilterRule>
    {
    }

    /// <summary>
    /// This class is used to define a hierarchical filter for a given collection. This type can be serialized/deserialized by JSON.NET without needing to modify the data structure from QueryBuilder.
    /// </summary>
    public abstract class FilterRuleBase<RuleType> : IFilterRule
        where RuleType : IFilterRule
    {
        /// <summary>
        /// condition - acceptable values are "and" and "or".
        /// </summary>
        public virtual string condition { get; set; }


        public virtual string field { get; set; }


        public virtual string @operator { get; set; }

        /// <summary>
        ///  nested filter rules.
        /// </summary>
        public virtual List<RuleType> rules { get; set; }


        /// <summary>
        /// Gets or sets the value of the filter.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public virtual object value { get; set; }

        IEnumerable<IFilterRule> IFilterRule.rules => rules?.Select(r => (IFilterRule)r);


        public virtual Expression GetLeftValueExpression(Expression valueExpression)
        {
            if (!string.IsNullOrWhiteSpace(field))
                valueExpression = LinqHelp.GetFieldMemberExpression(valueExpression, field);
            return valueExpression;
        }

        public override bool Equals(object obj)
        {
            if (obj is FilterRuleBase<RuleType> rule)
            {
                // TODO: what if types are not same but values are same
                if (condition != rule.condition || field != rule.field || @operator != rule.@operator || value?.Equals(rule.value) == false)
                    return false;

                if (condition == null)
                {
                    return true;
                }
                else
                {
                    if ((rules?.Count ?? 0) != (rule.rules?.Count ?? 0))
                        return false;

                    for (int i = 0; i < rules.Count; i++)
                    {
                        if (!rules[i].Equals(rule.rules[i]))
                            return false;
                    }
                    return true;
                }
            }
            return false;
        }

    }



}
