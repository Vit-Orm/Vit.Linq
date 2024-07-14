using System;
using System.Linq;
using System.Linq.Expressions;

using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq.ExpressionTree.ExpressionConvertor.MethodCalls.Queryable_Methods
{

    /// <summary>
    ///  Queryable.Where
    /// </summary>
    public class Where : MethodConvertor_Common
    {
        // public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        // public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int, bool>> predicate)
        public override Type methodType { get; } = typeof(Queryable);

        public override Expression ToCode(ToCodeArgument arg, ExpressionNode_MethodCall call)
        {
            var expSource = arg.convertService.ConvertToCode(arg, call.arguments[0]);
            var elementType = expSource.Type.GetGenericArguments()[0];

            var lambda = call.arguments[1] as ExpressionNode_Lambda;

            // #1 IQueryable<TSource> Where<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
            if (lambda?.parameterNames?.Length == 1)
            {
                var expPredicate = arg.convertService.ConvertToCode_LambdaExpression(arg, lambda, elementType);

                var methodArguments = new[] { expSource, expPredicate };

                var method = (new Func<IQueryable<string>, Expression<Func<string, bool>>, IQueryable<string>>(Queryable.Where<string>))
                                .Method.GetGenericMethodDefinition().MakeGenericMethod(elementType);
                return Expression.Call(method, methodArguments);
            }

            // #2 IQueryable<TSource> Where<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int, bool>> predicate)
            else if (lambda?.parameterNames?.Length == 2)
            {
                var expPredicate = arg.convertService.ConvertToCode_LambdaExpression(arg, lambda, elementType, typeof(int));

                var methodArguments = new[] { expSource, expPredicate };

                var method = (new Func<IQueryable<string>, Expression<Func<string, int, bool>>, IQueryable<string>>(Queryable.Where<string>))
                                .Method.GetGenericMethodDefinition().MakeGenericMethod(elementType);
                return Expression.Call(method, methodArguments);
            }

            throw new NotSupportedException($"Method not supported: {call.methodName}");
        }
    }


}
