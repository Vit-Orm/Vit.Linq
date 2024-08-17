using System.Linq.Expressions;
using System.Reflection;

namespace Vit.Linq.ExpressionNodes
{
    public partial class ToDataArgument
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
            return GetEValueType(expression) == EDataValueType.constant;
        }

        public static bool CalculateToConstant_ManuallyReduceMember { get; set; } = true;

        public object CalculateToConstant(Expression expression)
        {
            if (CalculateToConstant_ManuallyReduceMember && expression is MemberExpression member)
            {
                if (TryReduceMember(member, out var value))
                    return value;
            }

            return Expression.Lambda(expression).Compile().DynamicInvoke();
        }


        public static bool TryReduceMember(MemberExpression member, out object value)
        {
            value = null;
            switch (member.Member)
            {
                case FieldInfo fieldInfo:
                    {
                        object innerValue =
                           member.Expression switch
                           {
                               ConstantExpression constant => constant.Value,
                               MemberExpression innerMember => TryReduceMember(innerMember, out innerValue) ? innerValue : null,
                               _ => null,
                           };

                        if (innerValue == null) return false;

                        value = fieldInfo.GetValue(innerValue);
                        return true;
                    }
                case PropertyInfo fieldInfo:
                    {
                        object innerValue =
                           member.Expression switch
                           {
                               ConstantExpression constant => constant.Value,
                               MemberExpression innerMember => TryReduceMember(innerMember, out innerValue) ? innerValue : null,
                               _ => null,
                           };

                        if (innerValue == null) return false;

                        value = fieldInfo.GetValue(innerValue);
                        return true;
                    }
            }

            return false;
        }


    }



}
