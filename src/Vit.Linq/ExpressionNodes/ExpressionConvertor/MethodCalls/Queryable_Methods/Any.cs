using System;
using System.Linq;
using System.Linq.Expressions;

using Vit.Linq.ExpressionNodes.ComponentModel;

namespace Vit.Linq.ExpressionNodes.ExpressionConvertor.MethodCalls.Queryable_Methods
{

    /// <summary>
    ///  Queryable.Any
    /// </summary>
    public class Any : MethodConvertor_Common
    {
        // public static bool Any<TSource>(this IQueryable<TSource> source)
        // public static bool Any<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)

        public override Type methodType { get; } = typeof(Queryable);

        public override Expression ToCode(ToCodeArgument arg, ExpressionNode_MethodCall call)
        {
            //var instance = convertService.ToExpression(arg, call.instance);
            //var methodArguments = call.methodArguments?.Select(node => convertService.ToExpression(arg, node)).ToArray();

            var expSource = arg.convertService.ConvertToCode(arg, call.arguments[0]);
            var elementType = expSource.Type.GetGenericArguments()[0];

            // #1 public static bool Any<TSource>(this IQueryable<TSource> source)
            if (call.arguments?.Length == 1)
            {
                var methodArguments = new[] { expSource };

                var method = (new Func<IQueryable<string>, bool>(Queryable.Any<string>))
                                .Method.GetGenericMethodDefinition().MakeGenericMethod(elementType);
                return Expression.Call(method, methodArguments);
            }

            // #2 public static bool Any<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
            else if (call.arguments?.Length == 2)
            {
                var lambda = call.arguments[1] as ExpressionNode_Lambda;

                var expPredicate = arg.convertService.ConvertToCode_LambdaExpression(arg, lambda, elementType);

                var methodArguments = new[] { expSource, expPredicate };

                var method = (new Func<IQueryable<string>, Expression<Func<string, bool>>, bool>(Queryable.Any<string>))
                                .Method.GetGenericMethodDefinition().MakeGenericMethod(elementType);
                return Expression.Call(method, methodArguments);
            }

            throw new NotSupportedException($"Method not supported: {call.methodName}");
        }
    }



}
