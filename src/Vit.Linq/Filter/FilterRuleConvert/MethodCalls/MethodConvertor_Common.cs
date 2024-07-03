using System;
using System.Linq.Expressions;

using Vit.Linq.Filter.ComponentModel;

namespace Vit.Linq.Filter.MethodCalls
{

    public abstract class MethodConvertor_Common : IMethodConvertor
    {
        public virtual int priority { get; set; }


        public string methodName;
        public virtual Type methodType { get => null; }

        public MethodConvertor_Common(string methodName = null, int priority = 100)
        {
            if (string.IsNullOrWhiteSpace(methodName)) methodName = GetType().Name;
            this.methodName = methodName;
            this.priority = priority;
        }

        public virtual bool PredicateToData(DataConvertArgument arg, MethodCallExpression call)
        {
            return call.Method.Name == methodName && (methodType == null || methodType == call.Method.DeclaringType);
        }

        public abstract FilterRule ToData(DataConvertArgument arg, MethodCallExpression call);

    }

}
