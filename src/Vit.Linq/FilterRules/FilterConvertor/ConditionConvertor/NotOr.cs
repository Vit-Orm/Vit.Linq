using System.Linq;
using System.Linq.Expressions;

using Vit.Linq.FilterRules.ComponentModel;

namespace Vit.Linq.FilterRules.FilterConvertor.ConditionConvertor
{
    public class NotOr : IConditionConvertor
    {
        public int priority { get; set; } = 10;
        public (bool success, Expression expression) ConvertToCode(FilterConvertArgument arg, IFilterRule filter, string condition)
        {
            if (RuleCondition.NotOr.Equals(condition, arg.comparison))
            {
                if (filter.rules?.Any() != true)
                    return (true, null);

                return (true, Expression.Not(And.ConvertAnd(arg, filter.rules, isAnd: false)));
            }
            return default;
        }

    }
}
