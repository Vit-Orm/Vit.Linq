using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Vit.Linq.Filter.ComponentModel;

namespace Vit.Linq.Filter.FilterConvertor.OperatorConvert
{
    public class In : IOperatorConvertor
    {
        public int priority => 3000;

        public (bool success, Expression expression) ConvertToCode(OperatorConvertArgument arg)
        {
            if (RuleOperator.In.Equals(arg.Operator, arg.comparison))
            {
                return (true, ConvertIn(arg));
            }
            else if (RuleOperator.NotIn.Equals(arg.Operator, arg.comparison))
            {
                return (true, Expression.Not(ConvertIn(arg)));
            }

            return default;
        }


        public static Expression ConvertIn(OperatorConvertArgument arg)
        {
            var filter = arg.filter;
            var leftValueExpression = arg.leftValueExpression;
            var leftValueType = arg.leftValueType;

            // #1 using Enumerable<>.Contains
            //Type valueType = typeof(IEnumerable<>).MakeGenericType(leftValueType);
            //var rightValueExpression = this.GetRightValueExpression(rule, parameter, valueType);

            //return Expression.Call(typeof(System.Linq.Enumerable), "Contains", new[] { leftValueType }, rightValueExpression, leftValueExpression);

            //-------------------------------------
            // #2 using List<>.Contains
            Type valueType = typeof(List<>).MakeGenericType(leftValueType);
            var rightValueExpression = arg.filterService.GetRightValueExpression(filter, arg.parameter, valueType);

            return Expression.Call(rightValueExpression, "Contains", null, leftValueExpression);
        }
    }
}
