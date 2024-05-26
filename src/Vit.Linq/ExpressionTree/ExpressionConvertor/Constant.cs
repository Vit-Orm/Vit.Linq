using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;

using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq.ExpressionTree.ExpressionConvertor
{

    public class Constant : IExpressionConvertor
    {
        public ExpressionNode ConvertToData(DataConvertArgument arg, Expression expression)
        {
            if (expression is ConstantExpression constant)
            {
                var type = expression.Type;
                var value = constant.Value;
                if (value != null && !IsTransportableType(type))
                {
                    return arg.GetParameter(value, type);
                }
                return ExpressionNode.Constant(value: constant.Value, type: type);
            }

            return null;
        }

        public Expression ConvertToCode(CodeConvertArgument arg, ExpressionNode data)
        {
            if (data.nodeType != NodeType.Constant) return null;

            ExpressionNode_Constant constant = data;
            var value = constant.value;
            Type targetType = constant.valueType?.ToType();

            if (targetType == null) return Expression.Constant(value);

            if (value != null)
            {
                value = ComponentModel.ValueType.ConvertToType(value, targetType);
            }
            return Expression.Constant(value, targetType);
        }



        static bool IsTransportableType(Type type)
        {
            if (IsBasicType(type)) return true;

            if (type.IsArray && IsTransportableType(type.GetElementType()))
            {
                return true;
            }

            if (type.IsGenericType)
            {
                if (type.GetGenericArguments().Any(t => !IsTransportableType(t))) return false;

                if (typeof(IList).IsAssignableFrom(type)
                    || typeof(ICollection).IsAssignableFrom(type)
                    )
                    return true;
            }

            return false;

            #region Method IsBasicType
            // is valueType of Nullable 
            static bool IsBasicType(Type type)
            {
                return
                    type.IsEnum || // enum
                    type == typeof(string) || // string
                    type.IsValueType ||  //int
                    (type.IsGenericType && typeof(Nullable<>) == type.GetGenericTypeDefinition()); // int?
            }
            #endregion
        }

    }
}
