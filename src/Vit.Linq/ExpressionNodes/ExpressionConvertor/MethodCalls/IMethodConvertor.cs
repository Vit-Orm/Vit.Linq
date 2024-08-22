using System.Linq.Expressions;

using Vit.Linq.ExpressionNodes.ComponentModel;

namespace Vit.Linq.ExpressionNodes.ExpressionConvertor.MethodCalls
{
    public interface IMethodConvertor
    {
        int priority { get; }
        (bool success, ExpressionNode node) ToData(ToDataArgument arg, MethodCallExpression call);
        (bool success, Expression expression) ToCode(ToCodeArgument arg, ExpressionNode_MethodCall call);

    }
}
