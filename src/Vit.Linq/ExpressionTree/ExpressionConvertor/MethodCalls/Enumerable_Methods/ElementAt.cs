using System;
using System.Linq.Expressions;
using System.Linq;
using Vit.Linq.ExpressionTree.ComponentModel;
using System.Collections.Generic;

namespace Vit.Linq.ExpressionTree.ExpressionConvertor.MethodCalls.Enumerable_Methods
{

    /// <summary>
    /// Enumerable.ElementAt
    /// </summary>
    public class ElementAt : MethodConvertor_Common
    {
        public override Type methodType { get; } = typeof(Enumerable);
        public override Expression ToCode(CodeConvertArgument arg, ExpressionNode_MethodCall call)
        {
            //var instance = convertService.ToExpression(arg, call.instance);
            var methodArguments = call.arguments?.Select(node => arg.convertService.ToExpression(arg, node)).ToArray();


            // Enumerable.ElementAt
            var type = methodArguments[0].Type.GetGenericArguments()[0];
            var method = (new Func<IEnumerable<string>, int, string>(Enumerable.ElementAt<string>))
                            .Method.GetGenericMethodDefinition().MakeGenericMethod(type);
            return Expression.Call(method, methodArguments);
        }
    }

}
