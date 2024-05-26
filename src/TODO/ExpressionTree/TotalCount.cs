using System;
using System.Linq.Expressions;
using System.Linq;
using Vit.Linq.ExpressionTree.ComponentModel;
using Vit.Extensions.Linq_Extensions;

namespace Vit.Linq.ExpressionTree.ExpressionConvertor.MethodCalls.Queryable_Extensions_Methods
{

    public class TotalCount : MethodConvertor_Common
    {
        public override Type methodType { get; } = typeof(Queryable_Extensions);

        public override Expression ToCode(CodeConvertArgument arg, ExpressionNode_MethodCall call)
        {
            var source = arg.convertService.ToExpression(arg, call.arguments[0]);
            var elementType = source.Type.GetGenericArguments()[0];

            var methodArguments = new Expression[] { source };

            var method = (new Func<IQueryable<string>, int>(Queryable_Extensions.TotalCount<string>))
                            .Method.GetGenericMethodDefinition().MakeGenericMethod(elementType);
            return Expression.Call(method, methodArguments);
        }
    }




}
