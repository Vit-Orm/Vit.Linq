using System.Linq.Expressions;
using System.Reflection;

namespace Vit.Linq.ExpressionTree
{
    public partial class DataConvertArgument
    {
        public bool autoReduce { get; set; } = false;

        public bool ReduceValue<T>(Expression expression, out T value)
        {
            try
            {
                if (autoReduce && CanCalculateToConstant(expression))
                {
                    value = (T)CalculateToConstant(expression);
                    return true;
                }
            }
            catch { }
            value = default;
            return false;
        }


        public bool CanCalculateToConstant(Expression expression)
        {
            return GetEValueType(expression) == EValueType.constant;
        }

        public static bool CalculateToConstant_ManuallyReduceMember { get; set; } = true;

        public object CalculateToConstant(Expression expression)
        {
            if (CalculateToConstant_ManuallyReduceMember
                && expression is MemberExpression member
                && member.Expression is ConstantExpression constant
                && member.Member is FieldInfo memberInfo
                )
            {
                var v = constant.Value;
                if (v == null) return default;

                return memberInfo.GetValue(v);
            }

            return Expression.Lambda(expression).Compile().DynamicInvoke();
        }


    }



}
