using System;
using System.Linq.Expressions;

using Vit.Linq.Filter.ComponentModel;

namespace Vit.Linq.Filter.FilterConvertor.OperatorConvert
{
    public class IsNull : IOperatorConvertor
    {
        public int priority => 1000;

        public (bool success, Expression expression) ConvertToCode(OperatorConvertArgument arg)
        {
            if (RuleOperator.IsNull.Equals(arg.Operator, arg.comparison))
            {
                return (true, ConvertIsNull(arg));
            }

            return default;
        }


        public static Expression ConvertIsNull(OperatorConvertArgument arg)
        {
            var leftValueExpression = arg.leftValueExpression;
            var leftValueType = arg.leftValueType;

            var isNullable = !leftValueType.IsValueType || Nullable.GetUnderlyingType(leftValueType) != null;

            if (isNullable)
            {
                var nullValue = Expression.Constant(null, leftValueType);
                return Expression.Equal(leftValueExpression, nullValue);
            }
            return Expression.Constant(false, typeof(bool));
        }
    }
}
