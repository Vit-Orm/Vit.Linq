using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq.ExpressionTree.ExpressionConvertor
{

    public class Binary : IExpressionConvertor
    {
        public ExpressionNode ConvertToData(ToDataArgument arg, Expression expression)
        {
            switch (expression)
            {
                case TypeBinaryExpression typeIs when expression.NodeType == ExpressionType.TypeIs:
                    {
                        var left = arg.convertService.ConvertToData(arg, typeIs.Expression);
                        var type = typeIs.TypeOperand;
                        var right = ExpressionNode.Constant(value: null, type: type).SetCodeArg("TypeIs_Type", type);
                        return ExpressionNode.Binary(nodeType: nameof(ExpressionType.TypeIs), left: left, right: right);
                    }
                case UnaryExpression typeAs when expression.NodeType == ExpressionType.TypeAs:
                    {
                        var left = arg.convertService.ConvertToData(arg, typeAs.Operand);
                        var type = typeAs.Type;
                        var right = ExpressionNode.Constant(value: null, type: type).SetCodeArg("TypeAs_Type", type);
                        return ExpressionNode.Binary(nodeType: nameof(ExpressionType.TypeAs), left: left, right: right);
                    }
                case IndexExpression arrayIndex when expression.NodeType == ExpressionType.ArrayIndex:
                    {
                        var left = arg.convertService.ConvertToData(arg, arrayIndex.Object);
                        var pi = arrayIndex.Indexer;
                        var right = arg.convertService.ConvertToData(arg, arrayIndex.Arguments[0]);
                        return ExpressionNode.ArrayIndex(left: left, right: right);
                    }
                case BinaryExpression binary:
                    {
                        var left = arg.convertService.ConvertToData(arg, binary.Left);
                        var right = arg.convertService.ConvertToData(arg, binary.Right);

                        switch (binary.NodeType)
                        {
                            case ExpressionType.Add:
                                {
                                    return ExpressionNode.Add(left: left, right: right, valueType: binary.Type);
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
            }
            return null;
        }


        public Expression ConvertToCode(ToCodeArgument arg, ExpressionNode data)
        {
            if (data.expressionType != "Binary") return null;

            switch (data.nodeType)
            {
                case nameof(ExpressionType.ArrayIndex):
                    {
                        var left = arg.convertService.ConvertToCode(arg, data.left);
                        var right = arg.convertService.ConvertToCode(arg, data.right);

                        left ??= Expression.Constant(null);
                        right ??= Expression.Constant(null);
                        return Expression.ArrayIndex(left, right);
                    }
                case nameof(ExpressionType.TypeIs):
                    {
                        var value = arg.convertService.ConvertToCode(arg, data.left);
                        var type = data.right?.GetCodeArg("TypeIs_Type") as Type;
                        type ??= data.right?.valueType?.ToType();
                        return Expression.TypeIs(value, type);
                    }
                case nameof(ExpressionType.TypeAs):
                    {
                        var value = arg.convertService.ConvertToCode(arg, data.left);
                        var type = data.right?.GetCodeArg("TypeAs_Type") as Type;
                        type ??= data.right?.valueType?.ToType();
                        return Expression.TypeAs(value, type);
                    }

                case nameof(ExpressionType.Add):
                    {
                        var left = arg.convertService.ConvertToCode(arg, data.left);
                        var right = arg.convertService.ConvertToCode(arg, data.right);

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
                        var left = arg.convertService.ConvertToCode(arg, data.left);
                        var right = arg.convertService.ConvertToCode(arg, data.right);

                        left ??= Expression.Constant(null);
                        right ??= Expression.Constant(null);


                        //var method = typeof(Expression).GetMethod(data.nodeType, new[] { typeof(Expression), typeof(Expression) });
                        //return method?.Invoke(null, new object[] { left, right }) as Expression;

                        if (binaryMethodCache.TryGetValue(data.nodeType, out var method))
                        {
                            return method.Invoke(null, new object[] { left, right }) as Expression;
                        }
                        return default;
                    }
            }

        }

        static Dictionary<string, MethodInfo> binaryMethodCache = typeof(Expression).GetMethods()
                          .Where(method => method.IsPublic && method.IsStatic && method.GetParameters().Length == 2 && method.GetParameters().All(param => param?.ParameterType == typeof(Expression)))
                          .ToDictionary(m => m.Name, m => m);

    }
}
