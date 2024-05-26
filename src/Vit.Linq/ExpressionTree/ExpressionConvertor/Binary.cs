using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;

using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq.ExpressionTree.ExpressionConvertor
{

    public class Binary : IExpressionConvertor
    {
        public ExpressionNode ConvertToData(DataConvertArgument arg, Expression expression)
        {
            if (expression is IndexExpression index)
            {
                var left = arg.convertService.ConvertToData(arg, index.Object);
                var right = arg.convertService.ConvertToData(arg, index.Arguments[0]);
                return ExpressionNode.ArrayIndex(left: left, right: right);
            }
            else if (expression is BinaryExpression binary)
            {
                var left = arg.convertService.ConvertToData(arg, binary.Left);
                var right = arg.convertService.ConvertToData(arg, binary.Right);

                switch (binary.NodeType)
                {
                    case ExpressionType.AndAlso: return ExpressionNode.And(left: left, right: right);
                    case ExpressionType.OrElse: return ExpressionNode.Or(left: left, right: right);

                    //case ExpressionType.ArrayIndex: return ExpressionNode.ArrayIndex(left: left, right: right);

                    //case ExpressionType.Equal: return ExpressionNode.Equal(left: left, right: right);
                    //case ExpressionType.NotEqual: return ExpressionNode.NotEqual(left: left, right: right);
                    //case ExpressionType.GreaterThan: return ExpressionNode.GreaterThan(left: left, right: right);
                    //case ExpressionType.GreaterThanOrEqual: return ExpressionNode.GreaterThanOrEqual(left: left, right: right);
                    //case ExpressionType.LessThan: return ExpressionNode.LessThan(left: left, right: right);
                    //case ExpressionType.LessThanOrEqual: return ExpressionNode.LessThanOrEqual(left: left, right: right);

                    //case ExpressionType.Add: return new ExpressionNode { nodeType = binary.NodeType.ToString(), expressionType = "Binary", left = left, right = right };
                    case ExpressionType.Add:
                        {
                            ComponentModel.ValueType valueType = null;
                            if (binary.Left.Type == typeof(string) || binary.Right.Type == typeof(string))
                                valueType = ComponentModel.ValueType.FromType(typeof(string));
                            return new ExpressionNode { nodeType = binary.NodeType.ToString(), expressionType = "Binary", left = left, right = right, valueType = valueType };
                        }

                    default: return new ExpressionNode { nodeType = binary.NodeType.ToString(), expressionType = "Binary", left = left, right = right };
                }
                throw new NotSupportedException($"Unsupported binary operator: {binary.NodeType}");
            }
            return null;
        }


        public Expression ConvertToCode(CodeConvertArgument arg, ExpressionNode data)
        {
            if (data.expressionType != "Binary") return null;

            var left = arg.convertService.ToExpression(arg, data.left);
            var right = arg.convertService.ToExpression(arg, data.right);

            switch (data.nodeType)
            {
                case NodeType.And:
                    return Expression.AndAlso(left, right);

                case NodeType.Or:
                    return Expression.OrElse(left, right);

                case NodeType.ArrayIndex:
                    return Expression.ArrayIndex(left, right);

                //case NodeType.Equal: return Expression.Equal(ToExpression(arg, data.left) ?? Expression.Constant(null), ToExpression(arg, data.right) ?? Expression.Constant(null));
                //case NodeType.NotEqual: return Expression.NotEqual(ToExpression(arg, data.left) ?? Expression.Constant(null), ToExpression(arg, data.right) ?? Expression.Constant(null));
                //case NodeType.LessThan: return Expression.LessThan(ToExpression(arg, data.left) ?? Expression.Constant(null), ToExpression(arg, data.right) ?? Expression.Constant(null));
                //case NodeType.LessThanOrEqual: return Expression.LessThanOrEqual(ToExpression(arg, data.left) ?? Expression.Constant(null), ToExpression(arg, data.right) ?? Expression.Constant(null));
                //case NodeType.GreaterThan: return Expression.GreaterThan(ToExpression(arg, data.left) ?? Expression.Constant(null), ToExpression(arg, data.right) ?? Expression.Constant(null));
                //case NodeType.GreaterThanOrEqual: return Expression.GreaterThanOrEqual(ToExpression(arg, data.left) ?? Expression.Constant(null), ToExpression(arg, data.right) ?? Expression.Constant(null));

                case nameof(ExpressionType.Add):
                    {
                        left ??= Expression.Constant(null);
                        right ??= Expression.Constant(null);
                        if (left.Type == typeof(string) || right.Type == typeof(string))
                        {
                            var method = typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object) });
                            return Expression.Call(method, new[] { left, right });
                        }
                        return Expression.Add(left, right);
                    }
                default:
                    {
                        left ??= Expression.Constant(null);
                        right ??= Expression.Constant(null);

                        var method = typeof(Expression).GetMethod(data.nodeType, new[] { typeof(Expression), typeof(Expression) });
                        return method?.Invoke(null, new object[] { left, right }) as Expression;
                    }
            }

        }



    }
}
