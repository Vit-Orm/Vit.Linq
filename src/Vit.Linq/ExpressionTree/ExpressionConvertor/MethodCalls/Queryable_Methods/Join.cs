using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq.ExpressionTree.ExpressionConvertor.MethodCalls.Queryable_Methods
{

    public class Join : MethodConvertor_Common
    {
        public override Type methodType { get; } = typeof(Queryable);

        public override Expression ToCode(CodeConvertArgument arg, ExpressionNode_MethodCall call)
        {
            //var instance = arg.convertService.ToExpression(arg, call.@object);
            //var methodArguments = call.arguments?.Select(node => convertService.ToExpression(arg, node)).ToArray();

            //   IQueryable<TResult> Join<TOuter, TInner, TKey, TResult>
            //      (this IQueryable<TOuter> outer,
            //          IEnumerable<TInner> inner,
            //          Expression<Func<TOuter, TKey>> outerKeySelector,
            //          Expression<Func<TInner, TKey>> innerKeySelector,
            //          Expression<Func<TOuter, TInner, TResult>> resultSelector);
            var outer = arg.convertService.ToExpression(arg, call.arguments[0]);
            var TOuter = outer.Type.GetGenericArguments()[0];

            var inner = arg.convertService.ToExpression(arg, call.arguments[1]);
            var TInner = inner.Type.GetGenericArguments()[0];

            var outerKeySelector = arg.convertService.ToLambdaExpression(arg, call.arguments[2], TOuter);
            var innerKeySelector = arg.convertService.ToLambdaExpression(arg, call.arguments[3], TInner);

            var resultSelector = arg.convertService.ToLambdaExpression(arg, call.arguments[4], TOuter, TInner);

            var TKey = outerKeySelector.ReturnType;
            var TResult = resultSelector.ReturnType;


            Type[] typeArguments = new[] { TOuter, TInner, TKey, TResult };
            Expression[] arguments = new[] { outer, inner, outerKeySelector, innerKeySelector, resultSelector };



            var method = (new Func<
                IQueryable<string>,
                IEnumerable<string>,
                Expression<Func<string, string>>,
                Expression<Func<string, string>>,
                Expression<Func<string, string, string>>,
                IQueryable<string>>(Queryable.Join)
            ).Method.GetGenericMethodDefinition().MakeGenericMethod(typeArguments);

            return Expression.Call(method, arguments);
        }




    }


}
