using System;
using System.Linq.Expressions;

using Vit.Linq.ExpressionTree.ComponentModel;

using static System.Net.Mime.MediaTypeNames;

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

                    case ExpressionType.Add:
                        {
                            ComponentModel.ValueType valueType = null;
                            if (binary.Left.Type == typeof(string) || binary.Right.Type == typeof(string))
                                valueType = ComponentModel.ValueType.FromType(typeof(string));
                            return new ExpressionNode { nodeType = binary.NodeType.ToString(), expressionType = "Binary", left = left, right = right, valueType = valueType };
                        }

                    //case ExpressionType.AndAlso:
                    //case ExpressionType.OrElse:

                    //case ExpressionType.ArrayIndex:
                    //case ExpressionType.Divide:

                    //case ExpressionType.Equal:
                    //case ExpressionType.NotEqual:
                    //case ExpressionType.GreaterThan:
                    //case ExpressionType.GreaterThanOrEqual:
                    //case ExpressionType.LessThan:
                    //case ExpressionType.LessThanOrEqual:

                    default: return ExpressionNode.Binary(nodeType: binary.NodeType.ToString(), left: left, right: right);
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

                //case nameof(ExpressionType.AndAlso):  return Expression.AndAlso(left, right);
                //case nameof(ExpressionType.OrElse):

                //case nameof(ExpressionType.ArrayIndex):
                //case nameof(ExpressionType.Divide):

                //case nameof(ExpressionType.Equal): return Expression.Equal(left ?? Expression.Constant(null), right ?? Expression.Constant(null));
                //case nameof(ExpressionType.NotEqual):
                //case nameof(ExpressionType.LessThan):
                //case nameof(ExpressionType.LessThanOrEqual):
                //case nameof(ExpressionType.GreaterThan):
                //case nameof(ExpressionType.GreaterThanOrEqual):


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
