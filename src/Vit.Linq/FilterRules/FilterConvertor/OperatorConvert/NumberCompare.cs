using System.Linq.Expressions;

using Vit.Linq.FilterRules.ComponentModel;

namespace Vit.Linq.FilterRules.FilterConvertor.OperatorConvert
{
    public class NumberCompare : IOperatorConvertor
    {
        public int priority => 2000;

        public (bool success, Expression expression) ConvertToCode(OperatorConvertArgument arg)
        {
            if (RuleOperator.Equal.Equals(arg.Operator, arg.comparison))
            {
                return (true, Expression.Equal(arg.leftValueExpression, arg.filterService.GetRightValueExpression(arg.filter, arg.parameter, arg.leftValueType)));
            }
            else if (RuleOperator.NotEqual.Equals(arg.Operator, arg.comparison))
            {
                return (true, Expression.NotEqual(arg.leftValueExpression, arg.filterService.GetRightValueExpression(arg.filter, arg.parameter, arg.leftValueType)));
            }
            else if (RuleOperator.GreaterThan.Equals(arg.Operator, arg.comparison))
            {
                return (true, Expression.GreaterThan(arg.leftValueExpression, arg.filterService.GetRightValueExpression(arg.filter, arg.parameter, arg.leftValueType)));
            }
            else if (RuleOperator.GreaterThanOrEqual.Equals(arg.Operator, arg.comparison))
            {
                return (true, Expression.GreaterThanOrEqual(arg.leftValueExpression, arg.filterService.GetRightValueExpression(arg.filter, arg.parameter, arg.leftValueType)));
            }
            else if (RuleOperator.LessThan.Equals(arg.Operator, arg.comparison))
            {
                return (true, Expression.LessThan(arg.leftValueExpression, arg.filterService.GetRightValueExpression(arg.filter, arg.parameter, arg.leftValueType)));
            }
            else if (RuleOperator.LessThanOrEqual.Equals(arg.Operator, arg.comparison))
            {
                return (true, Expression.LessThanOrEqual(arg.leftValueExpression, arg.filterService.GetRightValueExpression(arg.filter, arg.parameter, arg.leftValueType)));
            }

            return default;
        }



    }
}
