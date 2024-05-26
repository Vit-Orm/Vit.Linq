using System.Linq.Expressions;
using System.Linq;
using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq.ExpressionTree.ExpressionConvertor.MethodCalls
{
    #region #99 InstanceMethod
    // InstanceMethod:
    //  String.StartsWith
    //  String.EndsWith
    public class InstanceMethod : MethodConvertor_Base
    {
        public override int priority => 10000;

        public override bool PredicateToData(DataConvertArgument arg, MethodCallExpression call)
        {
            return call.Object != null;
            //return true;
        }

        public override bool PredicateToCode(CodeConvertArgument arg, ExpressionNode_MethodCall call)
        {
            return call.@object != null;
        }

        public override Expression ToCode(CodeConvertArgument arg, ExpressionNode_MethodCall call)
        {
            var instance = arg.convertService.ToExpression(arg, call.@object);
            var methodArguments = call.arguments?.Select(node => arg.convertService.ToExpression(arg, node)).ToArray();

            return Expression.Call(instance, call.methodName, null, methodArguments);
        }
    }
    #endregion

}
