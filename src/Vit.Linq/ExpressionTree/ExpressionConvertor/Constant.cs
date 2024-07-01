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
                        return ExpressionNode.Constant(value: constant.Value, type: type);
                    }
                case NewArrayExpression newArray:
                case ListInitExpression listInit:
                    {
                        if (arg.CanCalculateToConstant(expression))
                        {
                            return ExpressionNode.Constant(value: DataConvertArgument.InvokeExpression(expression), type: expression.Type);
                        }
                        break;
                    }
                default:
                    {
                        if (arg.autoReduce && arg.CanCalculateToConstant(expression))
                        {
                            var value = DataConvertArgument.InvokeExpression(expression);
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

        public Expression ConvertToCode(CodeConvertArgument arg, ExpressionNode data)
        {
            if (data.nodeType != NodeType.Constant) return null;

            ExpressionNode_Constant constant = data;
            var value = constant.value;
            Type targetType = constant.valueType?.ToType();

            if (targetType == null) return Expression.Constant(value);

            if (value != null)
            {
                value = ComponentModel.ValueType.ConvertValueToType(value, targetType);
            }
            return Expression.Constant(value, targetType);
        }

    }
}
