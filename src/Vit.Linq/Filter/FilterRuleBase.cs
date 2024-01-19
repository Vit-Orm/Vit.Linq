using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

namespace Vit.Linq.Filter
{
    /// <summary>
    /// This class is used to define a hierarchical filter for a given collection. This type can be serialized/deserialized by JSON.NET without needing to modify the data structure from QueryBuilder.
    /// </summary>
    [ExcludeFromCodeCoverage]
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

    }
}
