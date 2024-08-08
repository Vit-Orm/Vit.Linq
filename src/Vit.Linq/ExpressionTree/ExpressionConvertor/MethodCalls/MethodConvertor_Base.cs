using System.Linq.Expressions;

using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq.ExpressionTree.ExpressionConvertor.MethodCalls
{
    public abstract class MethodConvertor_Base : IMethodConvertor
    {
        public virtual int priority { get; set; } = 100;
        public abstract bool PredicateToCode(ToCodeArgument arg, ExpressionNode_MethodCall call);
        public abstract Expression ToCode(ToCodeArgument arg, ExpressionNode_MethodCall call);

        public abstract bool PredicateToData(ToDataArgument arg, MethodCallExpression call);

        public virtual ExpressionNode ToData(ToDataArgument arg, MethodCallExpression call) => MethodCall.ConvertToData(arg, call);

    }
}
