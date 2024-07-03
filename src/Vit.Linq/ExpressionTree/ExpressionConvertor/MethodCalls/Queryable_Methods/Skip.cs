using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq.ExpressionTree.ExpressionConvertor.MethodCalls.Queryable_Methods
{

    public class Skip : MethodConvertor_Common
    {
        public override Type methodType { get; } = typeof(Queryable);

        public override Expression ToCode(CodeConvertArgument arg, ExpressionNode_MethodCall call)
        {
            var methodArguments = call.arguments?.Select(node => arg.convertService.ToExpression(arg, node)).ToArray();

            MethodInfo method;
            var argType = methodArguments[0].Type;
            var modelType = argType.GetGenericArguments()[0];

            // Queryable.Skip
            method = (new Func<IQueryable<string>, int, IQueryable<string>>(Queryable.Skip<string>))
                            .Method.GetGenericMethodDefinition().MakeGenericMethod(modelType);

            return Expression.Call(method, methodArguments);
        }

    }

}
