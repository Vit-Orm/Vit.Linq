using System.Linq.Expressions;

using Vit.Linq.FilterRules.ComponentModel;

namespace Vit.Linq.FilterRules.FilterConvertor.ConditionConvertor
{
    public class Not : IConditionConvertor
    {
        public int priority { get; set; } = 10;
        public (bool success, Expression expression) ConvertToCode(FilterConvertArgument arg, IFilterRule filter, string condition)
        {
            if (RuleCondition.Not.Equals(condition, arg.comparison))
            {
                condition = filter.condition;
                filter.condition = null;
                try
                {
                    var result = arg.filterService.ConvertToCode(arg, filter);
                    if (!result.success)
                        return (true, null);

                    return (true, Expression.Not(result.expression));
                }
                finally
                {
                    filter.condition = condition;
                }
            }
            return default;
        }

    }
}
