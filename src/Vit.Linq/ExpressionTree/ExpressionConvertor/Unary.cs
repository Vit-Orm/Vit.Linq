using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;

using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq.ExpressionTree.ExpressionConvertor
{

    public class Unary : IExpressionConvertor
    {
        public ExpressionNode ConvertToData(DataConvertArgument arg, Expression expression)
        {
            if (expression is UnaryExpression unary)
            {
                switch (unary.NodeType)
                {
                    case ExpressionType.Convert:
                        return ExpressionNode.Convert(
                            valueType: ComponentModel.ValueType.FromType(unary.Type),
                            body: arg.convertService.ConvertToData(arg, unary.Operand)
                            );
                    case ExpressionType.Quote:
                        return arg.convertService.ConvertToData(arg, unary.Operand);

                    case ExpressionType.Not:
                        return ExpressionNode.Not(body: arg.convertService.ConvertToData(arg, unary.Operand));

                    default:
                        return new ExpressionNode
                        {
                            nodeType = unary.NodeType.ToString(),
                            expressionType = "Unary",
                            body = arg.convertService.ConvertToData(arg, unary.Operand),
                        };
                }
                throw new NotSupportedException($"Unsupported binary operator: {unary.NodeType}");
            }

            return null;
        }

        public Expression ConvertToCode(CodeConvertArgument arg, ExpressionNode data)
        {
            if (data.expressionType != "Unary") return null;

            switch (data.nodeType)
            {
                case NodeType.Convert:
                    {
                        ExpressionNode_Convert convert = data;
                        var value = arg.convertService.ToExpression(arg, convert.body);

                        Type type = convert.valueType?.ToType();
                        if (type == null) type = value?.Type;

                        return Expression.Convert(value, type);
                    }

                case NodeType.Not:
                    {
                        ExpressionNode_Not not = data;
                        return Expression.Not(arg.convertService.ToExpression(arg, not.body));
                    }
                default:
                    {
                        var operand = arg.convertService.ToExpression(arg, data.body);

                        var method = typeof(Expression).GetMethod(data.nodeType);
                        return method?.Invoke(null, new object[] { operand }) as Expression;
                    }
            }
            return null;
        }




    }
}
