using System;
using System.Linq.Expressions;
using System.Linq;
using Vit.Linq.ExpressionTree.ComponentModel;
using System.Collections.Generic;
using Vit.Extensions.Linq_Extensions;

namespace Vit.Linq.ExpressionTree.ExpressionConvertor.MethodCalls.Queryable_Extensions_Methods
{

    public class Methods : MethodConvertor_Base
    {

        public Type methodType { get; } = typeof(Queryable_Extensions);

        static readonly List<string> methodNames = typeof(Queryable_Extensions).GetMethods().Select(m => m.Name).ToList();
        public override int priority => 10000;

        public override bool PredicateToData(DataConvertArgument arg, MethodCallExpression call)
        {
            // is method from Queryable
            return methodType == call.Method.DeclaringType;
        }


        public override bool PredicateToCode(CodeConvertArgument arg, ExpressionNode_MethodCall call)
        {
            return methodType.Name == call.methodCall_typeName && methodNames.Contains(call.methodName);
        }

        public override Expression ToCode(CodeConvertArgument arg, ExpressionNode_MethodCall call)
        {
            if (call.arguments?.Count() == 1)
            {
                var source = arg.convertService.ToExpression(arg, call.arguments[0]);
                var elementType = source.Type.GetGenericArguments()[0];

                var methodArguments = new[] { source };

                return Expression.Call(typeof(Queryable), call.methodName, new[] { elementType }, methodArguments);
            }

            throw new NotSupportedException($"Unsupported method typeName: {call.methodCall_typeName}, methodName: {call.methodName}");
        }
    }



}
