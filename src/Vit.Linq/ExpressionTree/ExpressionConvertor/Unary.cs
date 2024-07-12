using System;
using System.Linq.Expressions;

using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq.ExpressionTree.ExpressionConvertor
{

    public class Unary : IExpressionConvertor
    {
        public ExpressionNode ConvertToData(ToDataArgument arg, Expression expression)
        {
            if (expression is UnaryExpression unary)
            {
                return unary.NodeType switch
                {
                    ExpressionType.Convert => ExpressionNode.Convert(
                                                valueType: ComponentModel.NodeValueType.FromType(unary.Type),
                                                body: arg.convertService.ConvertToData(arg, unary.Operand)
                                                ).SetCodeArg("Convert_Type", unary.Type),
                    ExpressionType.Quote => arg.convertService.ConvertToData(arg, unary.Operand),
                    // ExpressionType.Not:
                    // ExpressionType.ArrayLength:
                    _ => ExpressionNode.Unary(nodeType: unary.NodeType.ToString(), body: arg.convertService.ConvertToData(arg, unary.Operand)),
                };
            }
            else if (expression is DefaultExpression defaultExp)
            {
                var type = defaultExp.Type;
                var body = ExpressionNode.Constant(value: null, type: type).SetCodeArg("Default_Type", type);
                return ExpressionNode.Unary(nodeType: nameof(ExpressionType.Default), body: body);
            }

            return null;
        }

        public Expression ConvertToCode(ToCodeArgument arg, ExpressionNode data)
        {
            if (data.expressionType != "Unary") return null;

            switch (data.nodeType)
            {
                case nameof(ExpressionType.Default):
                    {
                        ExpressionNode_Unary unary = data;
                        var type = unary.body?.GetCodeArg("Default_Type") as Type;
                        type ??= unary.body?.valueType?.ToType();
                        return Expression.Default(type);
                    }
                case nameof(ExpressionType.Convert):
                    {
                        ExpressionNode_Convert convert = data;
                        var value = arg.convertService.ConvertToCode(arg, convert.body);
                        var type = convert.GetCodeArg("Convert_Type") as Type;
                        type ??= convert.valueType?.ToType() ?? value?.Type;
                        return Expression.Convert(value, type);
                    }

                //case nameof(ExpressionType.Not):
                //case nameof(ExpressionType.ArrayLength):

                //case nameof(ExpressionType.Negate):
                //    {
                //        ExpressionNode_Unary unary = data;
                //        var operand = arg.convertService.ToExpression(arg, unary.body);
                //        return Expression.Negate(operand);
                //    }

                default:
                    {
                        ExpressionNode_Unary unary = data;
                        var operand = arg.convertService.ConvertToCode(arg, unary.body);

                        var method = typeof(Expression).GetMethod(data.nodeType, new[] { typeof(Expression) });
                        return method?.Invoke(null, new object[] { operand }) as Expression;
                    }
            }

        }

    }
}
