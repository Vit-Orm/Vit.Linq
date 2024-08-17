using System.Linq;
using System.Linq.Expressions;

using Vit.Linq.ExpressionNodes.ComponentModel;

namespace Vit.Linq.ExpressionNodes.ExpressionConvertor.MethodCalls
{

    // InstanceMethod:
    //  String.StartsWith
    //  String.EndsWith
    public class InstanceMethod : MethodConvertor_Base
    {
        public override int priority => 10000;

        public override bool PredicateToData(ToDataArgument arg, MethodCallExpression call)
        {
            return call.Object != null;
            //return true;
        }

        public override bool PredicateToCode(ToCodeArgument arg, ExpressionNode_MethodCall call)
        {
            return call.@object != null;
        }

        public override Expression ToCode(ToCodeArgument arg, ExpressionNode_MethodCall call)
        {
            var instance = arg.convertService.ConvertToCode(arg, call.@object);
            var methodArguments = call.arguments?.Select(node => arg.convertService.ConvertToCode(arg, node)).ToArray();

            return Expression.Call(instance, call.methodName, null, methodArguments);
        }
    }


}
