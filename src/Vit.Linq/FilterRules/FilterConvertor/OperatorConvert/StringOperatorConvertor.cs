using System.Linq.Expressions;

using Vit.Linq.FilterRules.ComponentModel;

namespace Vit.Linq.FilterRules.FilterConvertor.OperatorConvert
{
    public class StringOperatorConvertor : IOperatorConvertor
    {
        public int priority => 4000;

        public (bool success, Expression expression) ConvertToCode(OperatorConvertArgument arg)
        {
            return arg.filterService.checkNullForString ? ConvertToCode_CheckNull(arg) : ConvertToCode_Primitive(arg);
        }

        public (bool success, Expression expression) ConvertToCode_CheckNull(OperatorConvertArgument arg)
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
            else if (RuleOperator.NotContain.Equals(arg.Operator, arg.comparison))
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


        public (bool success, Expression expression) ConvertToCode_Primitive(OperatorConvertArgument arg)
        {
            if (RuleOperator.Contains.Equals(arg.Operator, arg.comparison))
            {
                var leftValueExpression = arg.leftValueExpression;
                var leftValueType = arg.leftValueType;
                var rightValueExpression = arg.filterService.GetRightValueExpression(arg.filter, arg.parameter, leftValueType);

                var contains = Expression.Call(leftValueExpression, "Contains", null, rightValueExpression);

                return (true, contains);
            }
            else if (RuleOperator.NotContain.Equals(arg.Operator, arg.comparison))
            {
                var leftValueExpression = arg.leftValueExpression;
                var leftValueType = arg.leftValueType;
                var rightValueExpression = arg.filterService.GetRightValueExpression(arg.filter, arg.parameter, leftValueType);

                var contains = Expression.Call(leftValueExpression, "Contains", null, rightValueExpression);

                return (true, Expression.Not(contains));
            }
            else if (RuleOperator.StartsWith.Equals(arg.Operator, arg.comparison))
            {
                var leftValueExpression = arg.leftValueExpression;
                var leftValueType = arg.leftValueType;
                var rightValueExpression = arg.filterService.GetRightValueExpression(arg.filter, arg.parameter, leftValueType);

                var startsWith = Expression.Call(leftValueExpression, "StartsWith", null, rightValueExpression);

                return (true, startsWith);
            }
            else if (RuleOperator.EndsWith.Equals(arg.Operator, arg.comparison))
            {
                var leftValueExpression = arg.leftValueExpression;
                var leftValueType = arg.leftValueType;
                var rightValueExpression = arg.filterService.GetRightValueExpression(arg.filter, arg.parameter, leftValueType);

                var endsWith = Expression.Call(leftValueExpression, "EndsWith", null, rightValueExpression);

                return (true, endsWith);
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
