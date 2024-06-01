using System;
using System.Linq.Expressions;
using System.Linq;
using Vit.Linq.ExpressionTree.ComponentModel;
using Vit.Extensions.Linq_Extensions;
using System.Collections.Generic;

namespace Vit.Linq.ExpressionTree.ExpressionConvertor.MethodCalls.Enumerable_Methods
{

    /// <summary>
    ///  Enumerable.Contains
    /// </summary>
    public class Contains : MethodConvertor_Common
    {
        // public static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value);
        public override Type methodType { get; } = typeof(Enumerable);

        public override Expression ToCode(CodeConvertArgument arg, ExpressionNode_MethodCall call)
        {
            //var instance = convertService.ToExpression(arg, call.instance);
            //var methodArguments = call.methodArguments?.Select(node => convertService.ToExpression(arg, node)).ToArray();

            var expSource = arg.convertService.ToExpression(arg, call.arguments[0]);
            var elementType = expSource.Type.GetGenericArguments()[0];

            // #1 public static bool Any<TSource>(this IEnumerable<TSource> source)
            if (call.arguments?.Length == 1)
            {
                var value = arg.convertService.ToExpression(arg, call.arguments[1]);


                var methodArguments = new[] { expSource, value };

                var method = (new Func<IEnumerable<string>, bool>(Enumerable.Any<string>))
                                .Method.GetGenericMethodDefinition().MakeGenericMethod(elementType);
                return Expression.Call(method, methodArguments);
            }

            throw new NotSupportedException($"Method not supported: {call.methodName}");
        }
    }



}
