using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Vit.Linq.FilterRules.ComponentModel;

namespace Vit.Linq.FilterRules.FilterConvertor.ConditionConvertor
{
    public class And : IConditionConvertor
    {
        public int priority { get; set; } = 10;
        public (bool success, Expression expression) ConvertToCode(FilterConvertArgument arg, IFilterRule filter, string condition)
        {
            if (RuleCondition.And.Equals(condition, arg.comparison))
            {
                if (filter.rules?.Any() != true)
                    return (true, null);

                return (true, ConvertAnd(arg, filter.rules, isAnd: true));
            }
            return default;
        }



        public static Expression ConvertAnd(FilterConvertArgument arg, IEnumerable<IFilterRule> rules, bool isAnd = true)
        {
            if (rules?.Any() != true)
            {
                return null;
            }

            Expression expression = null;

            foreach (var rule in rules)
            {
                var result = arg.filterService.ConvertToCode(arg, rule);
                if (result.expression != null)
                    expression = Append(isAnd, expression, result.expression);
            }

            return expression;


            #region Method Append
            static Expression Append(bool isAnd, Expression exp1, Expression exp2)
            {
                if (exp1 == null)
                {
                    return exp2;
                }

                if (exp2 == null)
                {
                    return exp1;
                }
                return isAnd ? Expression.AndAlso(exp1, exp2) : Expression.OrElse(exp1, exp2);
            }
            #endregion

        }
    }
}
