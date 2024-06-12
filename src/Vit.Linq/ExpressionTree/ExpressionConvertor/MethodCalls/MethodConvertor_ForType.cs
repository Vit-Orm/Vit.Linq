using System;
using System.Linq.Expressions;
using System.Linq;
using Vit.Linq.ExpressionTree.ComponentModel;
using System.Collections.Generic;

namespace Vit.Linq.ExpressionTree.ExpressionConvertor.MethodCalls
{

    public class MethodConvertor_ForType : MethodConvertor_Base
    {
        public MethodConvertor_ForType(Type methodType, List<string> methodNames = null, int priority = 10000)
        {
            this.methodType = methodType;
            this.methodNames = methodNames ?? methodType.GetMethods().Select(m => m.Name).ToList();
            this.priority = priority;
        }

        public Type methodType { get; protected set; }

        public List<string> methodNames { get; protected set; }
        public override int priority { get; set; }

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
            throw new NotSupportedException($"Unsupported method typeName: {call.methodCall_typeName}, methodName: {call.methodName}");
        }
    }



}
