using System.Linq.Expressions;

using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq.ExpressionTree.ExpressionConvertor.MethodCalls
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
