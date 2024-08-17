using System.Linq.Expressions;

using Vit.Linq.ExpressionNodes.ComponentModel;

namespace Vit.Linq.ExpressionNodes.ExpressionConvertor.MethodCalls
{
    public interface IMethodConvertor
    {
        int priority { get; }
        bool PredicateToCode(ToCodeArgument arg, ExpressionNode_MethodCall call);
        Expression ToCode(ToCodeArgument arg, ExpressionNode_MethodCall call);

        bool PredicateToData(ToDataArgument arg, MethodCallExpression call);
        ExpressionNode ToData(ToDataArgument arg, MethodCallExpression call);
    }
}
