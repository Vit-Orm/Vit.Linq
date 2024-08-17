using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Vit.Linq.ExpressionNodes.ComponentModel;

namespace Vit.Linq.ExpressionNodes.ExpressionConvertor.MethodCalls.Queryable_Methods
{

    public class SelectMany : MethodConvertor_Common
    {

        public override Type methodType { get; } = typeof(Queryable);

        public override Expression ToCode(ToCodeArgument arg, ExpressionNode_MethodCall call)
        {

            //   IQueryable<TResult> SelectMany<TSource, TCollection, TResult>(
            //      this IQueryable<TSource> source,
            //      Expression<Func<TSource, IEnumerable<TCollection>>> collectionSelector,
            //      Expression<Func<TSource, TCollection, TResult>> resultSelector
            //   )
            var source = arg.convertService.ConvertToCode(arg, call.arguments[0]);
            var TSource = source.Type.GetGenericArguments()[0];

            var collectionSelector = arg.convertService.ConvertToCode(arg, call.arguments[1]);
            var TCollection = collectionSelector.Type.GetGenericArguments()[0];

            var resultSelector = arg.convertService.ConvertToCode_LambdaExpression(arg, call.arguments[2], TSource, TCollection);

            var TResult = resultSelector.ReturnType;


            Type[] typeArguments = new[] { TSource, TCollection, TResult };
            Expression[] arguments = new[] { source, collectionSelector, resultSelector };


            var method = (new Func<
                IQueryable<string>,
                Expression<Func<string, IEnumerable<string>>>,
                Expression<Func<string, string, string>>,
                IQueryable<string>>(Queryable.SelectMany)
            ).Method.GetGenericMethodDefinition().MakeGenericMethod(typeArguments);

            return Expression.Call(method, arguments);
        }




    }


}
