using System;
using System.Linq;
using System.Linq.Expressions;

using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq.ExpressionTree.ExpressionConvertor.MethodCalls.Queryable_Methods
{
    public class Select : MethodConvertor_Common
    {
        public override Type methodType { get; } = typeof(Queryable);

        public override Expression ToCode(ToCodeArgument arg, ExpressionNode_MethodCall call)
        {
            var source = arg.convertService.ConvertToCode(arg, call.arguments[0]);
            var elementType = source.Type.GetGenericArguments()[0];
            var nodeSelector = call.arguments[1] as ExpressionNode_Lambda;

            // #1 IQueryable<TResult> Select<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector)
            if (nodeSelector?.parameterNames?.Length == 1)
            {
                var resultSelector = arg.convertService.ConvertToCode_LambdaExpression(arg, nodeSelector, elementType);

                var TResult = resultSelector.ReturnType;

                var methodArguments = new[] { source, resultSelector };

                var method = (new Func<
                                    IQueryable<string>,
                                    Expression<Func<string, string>>,
                                    IQueryable<string>
                                >(Queryable.Select))
                                .Method.GetGenericMethodDefinition().MakeGenericMethod(elementType, TResult);
                return Expression.Call(method, methodArguments);
            }

            // #2 IQueryable<TResult> Select<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, int, TResult>> selector)
            if (nodeSelector?.parameterNames?.Length == 2)
            {
                var resultSelector = arg.convertService.ConvertToCode_LambdaExpression(arg, nodeSelector, elementType, typeof(int));

                var TResult = resultSelector.ReturnType;

                var methodArguments = new[] { source, resultSelector };

                var method = (new Func<
                                    IQueryable<string>,
                                    Expression<Func<string, int, string>>,
                                    IQueryable<string>
                                 >(Queryable.Select))
                                .Method.GetGenericMethodDefinition().MakeGenericMethod(elementType, TResult);
                return Expression.Call(method, methodArguments);
            }

            throw new NotSupportedException($"Method not supported: {call.methodName}");
        }
    }



}
