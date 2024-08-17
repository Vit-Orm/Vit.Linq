using System;
using System.Linq.Expressions;

using Vit.Linq.ExpressionNodes.ComponentModel;

namespace Vit.Linq.ExpressionNodes.ExpressionConvertor
{

    public class Constant : IExpressionConvertor
    {
        public virtual int priority { get; set; } = 100;
        public ExpressionNode ConvertToData(ToDataArgument arg, Expression expression)
        {
            switch (expression)
            {
                case ConstantExpression constant:
                    {
                        var type = expression.Type;
                        var value = constant.Value;
                        if (arg.IsArgument(constant))
                        {
                            return arg.CreateParameter(value, type);
                        }
                        return ExpressionNode.Constant(value: constant.Value, type: type);
                    }
                case NewArrayExpression:
                case ListInitExpression:
                    {
                        if (arg.CanCalculateToConstant(expression))
                        {
                            return ExpressionNode.Constant(value: arg.CalculateToConstant(expression), type: expression.Type);
                        }
                        break;
                    }
                default:
                    {
                        if (arg.autoReduce && arg.CanCalculateToConstant(expression))
                        {
                            var value = arg.CalculateToConstant(expression);
                            var type = expression.Type;

                            if (arg.IsArgument(value: value, type: type))
                            {
                                return arg.CreateParameter(value, type);
                            }

                            return ExpressionNode.Constant(value: value, type: type);
                        }
                        break;
                    }
            }
            return null;
        }

        public Expression ConvertToCode(ToCodeArgument arg, ExpressionNode data)
        {
            if (data.nodeType != NodeType.Constant) return null;

            ExpressionNode_Constant constant = data;
            var value = constant.value;
            Type targetType = constant.Constant_GetType() ?? constant.valueType?.ToType();

            if (targetType == null) return Expression.Constant(value);

            if (value != null)
            {
                value = ComponentModel.NodeValueType.ConvertValueToType(value, targetType);
            }

            var constExp = Expression.Constant(value, targetType);

            //// Nullable<>
            //if (targetType.IsValueType && targetType.IsGenericType)
            //{
            //    return Expression.Convert(constExp, targetType);
            //}

            return constExp;
        }

    }
}
