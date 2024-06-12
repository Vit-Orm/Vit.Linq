using System;
using System.Linq.Expressions;

using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq.ExpressionTree.ExpressionConvertor.MethodCalls
{

    public abstract class MethodConvertor_Common : MethodConvertor_Base
    {
        public string methodName;
        public virtual Type methodType { get => null; }

        public MethodConvertor_Common(string methodName = null, int priority = 100)
        {
            if (string.IsNullOrWhiteSpace(methodName)) methodName = GetType().Name;
            this.methodName = methodName;
            this.priority = priority;
        }

        public override bool PredicateToData(DataConvertArgument arg, MethodCallExpression call)
        {
            return call.Method.Name == methodName && (methodType == null || methodType == call.Method.DeclaringType);
        }

        public override bool PredicateToCode(CodeConvertArgument arg, ExpressionNode_MethodCall call)
        {
            return call.methodName == methodName && (methodType == null || methodType.Name == call.methodCall_typeName);
        }
    }

}
