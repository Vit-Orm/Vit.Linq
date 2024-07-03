using System;
using System.Collections.Generic;
using System.Linq;

using Vit.Linq.Filter.ComponentModel;

namespace Vit.Linq
{
    public static partial class FilterRule_Extensions
    {
        public static FilterRule And<FilterRule>(this FilterRule left, params FilterRule[] rights)
            where FilterRule : FilterRuleBase<FilterRule>, new()
        {
            var rules = new List<FilterRule> { left };
            rules.AddRange(rights);
            return new FilterRule { condition = "And", rules = rules.Where(rule => rule != null).ToList() };
        }
        public static FilterRule Or<FilterRule>(this FilterRule left, params FilterRule[] rights)
             where FilterRule : FilterRuleBase<FilterRule>, new()
        {
            var rules = new List<FilterRule> { left };
            rules.AddRange(rights);
            return new FilterRule { condition = "Or", rules = rules.Where(rule => rule != null).ToList() };
        }

        public static FilterRule Not<FilterRule>(this FilterRule left)
            where FilterRule : FilterRuleBase<FilterRule>, new()
        {
            if (string.IsNullOrEmpty(left.condition)) left.condition = "Not";
            else if (left.condition.StartsWith("not", StringComparison.OrdinalIgnoreCase))
            {
                left.condition = left.condition.Substring(3);
            }
            else
            {
                left.condition = "Not" + left.condition;
            }
            return left;
        }

    }
}