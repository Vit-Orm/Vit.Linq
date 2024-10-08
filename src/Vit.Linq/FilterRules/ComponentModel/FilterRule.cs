﻿using System.Collections.Generic;
using System.Linq;

namespace Vit.Linq.FilterRules.ComponentModel
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
    public abstract class FilterRuleBase<FilterRuleType> : IFilterRule
        where FilterRuleType : IFilterRule
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
        public virtual List<FilterRuleType> rules { get; set; }


        /// <summary>
        /// Gets or sets the value of the filter.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public virtual object value { get; set; }

        IEnumerable<IFilterRule> IFilterRule.rules => rules?.Select(r => (IFilterRule)r);

        public override bool Equals(object obj)
        {
            if (obj is FilterRuleBase<FilterRuleType> rule)
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
