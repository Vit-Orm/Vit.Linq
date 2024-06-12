using System;
using System.Linq.Expressions;

using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq.ExpressionTree.ExpressionConvertor.MethodCalls
{

    public class MethodConvertor_Delegate : MethodConvertor_Base
    {
        public string methodName;
        /// <summary>
        /// Expression ToCode(convertService, CodeConvertArgument arg, ExpressionNode_Call call)
        /// </summary>
        public Func<CodeConvertArgument, ExpressionNode_MethodCall, Expression> FuncToCode;
        public MethodConvertor_Delegate(string methodName, Func<CodeConvertArgument, ExpressionNode_MethodCall, Expression> FuncToCode)
        {
            this.methodName = methodName;
            this.FuncToCode = FuncToCode;
        }

        public override bool PredicateToData(DataConvertArgument arg, MethodCallExpression call)
        {
            return call.Method.Name == methodName;
        }

        public override bool PredicateToCode(CodeConvertArgument arg, ExpressionNode_MethodCall call)
        {
            return call.methodName == methodName;
        }

        public override Expression ToCode(CodeConvertArgument arg, ExpressionNode_MethodCall call)
        {
            return FuncToCode(arg, call);
        }
    }

}
