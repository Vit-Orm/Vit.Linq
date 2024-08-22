using System;
using System.Linq.Expressions;

using Vit.Linq.ExpressionNodes.ComponentModel;

namespace Vit.Linq.ExpressionNodes.ExpressionConvertor.MethodCalls
{

    public class MethodConvertor_Delegate : MethodConvertor_Base
    {
        public string methodName;
        /// <summary>
        /// Expression ToCode(convertService, CodeConvertArgument arg, ExpressionNode_Call call)
        /// </summary>
        public Func<ToCodeArgument, ExpressionNode_MethodCall, Expression> FuncToCode;
        public MethodConvertor_Delegate(string methodName, Func<ToCodeArgument, ExpressionNode_MethodCall, Expression> FuncToCode)
        {
            this.methodName = methodName;
            this.FuncToCode = FuncToCode;
        }

        public override bool PredicateToData(ToDataArgument arg, MethodCallExpression call)
        {
            return call.Method.Name == methodName;
        }

        public override bool PredicateToCode(ToCodeArgument arg, ExpressionNode_MethodCall call)
        {
            return call.methodName == methodName;
        }

        public override Expression ToCode(ToCodeArgument arg, ExpressionNode_MethodCall call)
        {
            return FuncToCode(arg, call);
        }
    }

}
