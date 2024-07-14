using System.Linq.Expressions;

using Vit.Linq.Filter.ComponentModel;

namespace Vit.Linq.Filter.FilterConvertor.OperatorConvert
{
    public class StringOperatorConvertor : IOperatorConvertor
    {
        public int priority => 4000;

        public (bool success, Expression expression) ConvertToCode(OperatorConvertArgument arg)
        {
            if (RuleOperator.Contains.Equals(arg.Operator, arg.comparison))
            {
                var leftValueExpression = arg.leftValueExpression;
                var leftValueType = arg.leftValueType;
                var rightValueExpression = arg.filterService.GetRightValueExpression(arg.filter, arg.parameter, leftValueType);

                var nullCheck = Expression.Call(typeof(string), "IsNullOrEmpty", null, leftValueExpression);
                var contains = Expression.Call(leftValueExpression, "Contains", null, rightValueExpression);

                return (true, Expression.AndAlso(Expression.Not(nullCheck), contains));
            }
            else if (RuleOperator.NotContains.Equals(arg.Operator, arg.comparison))
            {
                var leftValueExpression = arg.leftValueExpression;
                var leftValueType = arg.leftValueType;
                var rightValueExpression = arg.filterService.GetRightValueExpression(arg.filter, arg.parameter, leftValueType);

                var nullCheck = Expression.Call(typeof(string), "IsNullOrEmpty", null, leftValueExpression);
                var contains = Expression.Call(leftValueExpression, "Contains", null, rightValueExpression);

                return (true, Expression.OrElse(nullCheck, Expression.Not(contains)));
            }
            else if (RuleOperator.StartsWith.Equals(arg.Operator, arg.comparison))
            {
                var leftValueExpression = arg.leftValueExpression;
                var leftValueType = arg.leftValueType;
                var rightValueExpression = arg.filterService.GetRightValueExpression(arg.filter, arg.parameter, leftValueType);

                var nullCheck = Expression.Not(Expression.Call(typeof(string), "IsNullOrEmpty", null, leftValueExpression));
                var startsWith = Expression.Call(leftValueExpression, "StartsWith", null, rightValueExpression);

                return (true, Expression.AndAlso(nullCheck, startsWith));
            }
            else if (RuleOperator.EndsWith.Equals(arg.Operator, arg.comparison))
            {
                var leftValueExpression = arg.leftValueExpression;
                var leftValueType = arg.leftValueType;
                var rightValueExpression = arg.filterService.GetRightValueExpression(arg.filter, arg.parameter, leftValueType);

                var nullCheck = Expression.Not(Expression.Call(typeof(string), "IsNullOrEmpty", null, leftValueExpression));
                var endsWith = Expression.Call(leftValueExpression, "EndsWith", null, rightValueExpression);

                return (true, Expression.AndAlso(nullCheck, endsWith));
            }
            else if (RuleOperator.IsNullOrEmpty.Equals(arg.Operator, arg.comparison))
            {
                var leftValueExpression = arg.leftValueExpression;

                return (true, Expression.Call(typeof(string), "IsNullOrEmpty", null, leftValueExpression));
            }
            else if (RuleOperator.IsNotNullOrEmpty.Equals(arg.Operator, arg.comparison))
            {
                var leftValueExpression = arg.leftValueExpression;

                return (true, Expression.Not(Expression.Call(typeof(string), "IsNullOrEmpty", null, leftValueExpression)));
            }

            return default;
        }


    }
}
