using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Vit.Linq.ExpressionNodes.ComponentModel;

namespace Vit.Linq.ExpressionNodes.ExpressionConvertor.MethodCalls.Queryable_Methods
{

    public class Methods : MethodConvertor_Base
    {
        static readonly Type methodType = typeof(Queryable);

        static readonly List<string> methodNames = methodType
            .GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Select(m => m.Name).ToList();
        public override int priority => 10000;

        public override bool PredicateToData(ToDataArgument arg, MethodCallExpression call)
        {
            return methodType == call.Method.DeclaringType;
        }


        public override bool PredicateToCode(ToCodeArgument arg, ExpressionNode_MethodCall call)
        {
            return methodType.Name == call.methodCall_typeName && methodNames.Contains(call.methodName) && call.arguments?.Length == 1;
        }

        public override Expression ToCode(ToCodeArgument arg, ExpressionNode_MethodCall call)
        {
            var source = arg.convertService.ConvertToCode(arg, call.arguments[0]);
            var elementType = source.Type.GetGenericArguments()[0];

            var methodArguments = new[] { source };

            return Expression.Call(methodType, call.methodName, new[] { elementType }, methodArguments);
        }
    }



}
