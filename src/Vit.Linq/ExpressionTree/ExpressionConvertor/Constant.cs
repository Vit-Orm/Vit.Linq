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
                if (value != null && DataConvertArgument.IsQueryableArgument(type))
                {
                    return arg.CreateParameter(value, type);
                }
                return ExpressionNode.Constant(value: constant.Value, type: type);
            }
            else if (expression is NewArrayExpression newArray)
            {
                if (DataConvertArgument.CanCalculateToConstant(newArray))
                {
                    return ExpressionNode.Constant(value: DataConvertArgument.InvokeExpression(expression), type: expression.Type);
                }
            }
            else if (expression is ListInitExpression listInit)
            {
                if (DataConvertArgument.CanCalculateToConstant(listInit))
                {
                    return ExpressionNode.Constant(value: DataConvertArgument.InvokeExpression(expression), type: expression.Type);
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
                value = ComponentModel.ValueType.ConvertToType(value, targetType);
            }
            return Expression.Constant(value, targetType);
        }

    }
}
