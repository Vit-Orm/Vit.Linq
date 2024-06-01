using System;
using System.Linq.Expressions;
using System.Linq;
using Vit.Linq.ExpressionTree.ComponentModel;
using Vit.Extensions.Linq_Extensions;
using System.Collections.Generic;

namespace Vit.Linq.ExpressionTree.ExpressionConvertor.MethodCalls.Enumerable_Methods
{

    /// <summary>
    ///  Queryable.Any
    /// </summary>
    public class Any : MethodConvertor_Common
    {
        // public static bool Any<TSource>(this IEnumerable<TSource> source);
        // public static bool Any<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate);
        public override Type methodType { get; } = typeof(Enumerable);

        public override Expression ToCode(CodeConvertArgument arg, ExpressionNode_MethodCall call)
        {
            //var instance = convertService.ToExpression(arg, call.instance);
            //var methodArguments = call.methodArguments?.Select(node => convertService.ToExpression(arg, node)).ToArray();

            var expSource = arg.convertService.ToExpression(arg, call.arguments[0]);
            var elementType = expSource.Type.GetGenericArguments()[0];

            // #1 public static bool Any<TSource>(this IQueryable<TSource> source)
            if (call.arguments?.Length == 1)
            {
                var methodArguments = new[] { expSource };

                var method = (new Func<IEnumerable<string>, bool>(Enumerable.Any<string>))
                                .Method.GetGenericMethodDefinition().MakeGenericMethod(elementType);
                return Expression.Call(method, methodArguments);
            }

            // #2 public static bool Any<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
            else if (call.arguments?.Length == 2)
            {
                var lambda = call.arguments[1] as ExpressionNode_Lambda;

                var expPredicate = arg.convertService.ToLambdaExpression(arg, lambda, elementType);

                var methodArguments = new[] { expSource, expPredicate };

                var method = (new Func<IEnumerable<string>, Func<string, bool>, bool>(Enumerable.Any<string>))
                                .Method.GetGenericMethodDefinition().MakeGenericMethod(elementType);
                return Expression.Call(method, methodArguments);
            }

            throw new NotSupportedException($"Method not supported: {call.methodName}");
        }
    }



}
