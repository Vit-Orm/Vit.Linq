using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq.ExpressionTree.ExpressionConvertor.MethodCalls.Queryable_Methods
{

    public class Methods : MethodConvertor_Base
    {

        public Type methodType { get; } = typeof(Queryable);

        static readonly List<string> methodNames = typeof(Queryable).GetMethods().Select(m => m.Name).ToList();
        public override int priority => 10000;

        public override bool PredicateToData(ToDataArgument arg, MethodCallExpression call)
        {
            // is method from Queryable
            return methodType == call.Method.DeclaringType;
        }


        public override bool PredicateToCode(ToCodeArgument arg, ExpressionNode_MethodCall call)
        {
            return methodType.Name == call.methodCall_typeName && methodNames.Contains(call.methodName);
        }

        public override Expression ToCode(ToCodeArgument arg, ExpressionNode_MethodCall call)
        {
            if (call.arguments?.Count() == 1)
            {
                var source = arg.convertService.ConvertToCode(arg, call.arguments[0]);
                var elementType = source.Type.GetGenericArguments()[0];

                var methodArguments = new[] { source };

                return Expression.Call(typeof(Queryable), call.methodName, new[] { elementType }, methodArguments);
            }

            throw new NotSupportedException($"Unsupported method typeName: {call.methodCall_typeName}, methodName: {call.methodName}");
        }
    }



}
