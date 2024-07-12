using System;
using System.Linq.Expressions;

using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq.ExpressionTree.ExpressionConvertor
{

    public class Constant : IExpressionConvertor
    {
        public ExpressionNode ConvertToData(DataConvertArgument arg, Expression expression)
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
                        return ExpressionNode.Constant(value: constant.Value, type: type).SetCodeArg("Constant_Type", type);
                    }
                case NewArrayExpression:
                case ListInitExpression:
                    {
                        if (arg.CanCalculateToConstant(expression))
                        {
                            return ExpressionNode.Constant(value: arg.CalculateToConstant(expression), type: expression.Type).SetCodeArg("Constant_Type", expression.Type);
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

                            return ExpressionNode.Constant(value: value, type: type).SetCodeArg("Constant_Type", type);
                        }
                        break;
                    }
            }
            return null;
        }

        public Expression ConvertToCode(CodeConvertArgument arg, ExpressionNode data)
        {
            if (data.nodeType != NodeType.Constant) return null;

            ExpressionNode_Constant constant = data;
            var value = constant.value;
            Type targetType = (constant.GetCodeArg("Constant_Type") as Type) ?? constant.valueType?.ToType();

            if (targetType == null) return Expression.Constant(value);

            if (value != null)
            {
                value = ComponentModel.ValueType.ConvertValueToType(value, targetType);
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
