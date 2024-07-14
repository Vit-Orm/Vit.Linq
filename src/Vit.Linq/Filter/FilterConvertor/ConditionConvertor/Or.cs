using System;
using System.Linq;
using System.Linq.Expressions;
using Vit.Linq.Filter.ComponentModel;

namespace Vit.Linq.Filter.FilterConvertor.ConditionConvertor
{
    public class Or : IConditionConvertor
    {
        public int priority { get; set; } = 10;
        public (bool success, Expression expression) ConvertToCode(FilterConvertArgument arg, IFilterRule filter, string condition)
        {
            if (RuleCondition.Or.Equals(condition, arg.comparison))
            {
                if (filter.rules?.Any() != true)
                    return (true, null);

                return (true, And.ConvertAnd(arg, filter.rules, isAnd: false));
            }
            return default;
        }

    }
}
