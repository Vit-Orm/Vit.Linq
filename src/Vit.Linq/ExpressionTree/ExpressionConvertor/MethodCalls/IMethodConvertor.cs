using System.Linq.Expressions;

using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq.ExpressionTree.ExpressionConvertor.MethodCalls
{
    public interface IMethodConvertor
    {
        int priority { get; }
        bool PredicateToCode(CodeConvertArgument arg, ExpressionNode_MethodCall call);
        Expression ToCode(CodeConvertArgument arg, ExpressionNode_MethodCall call);

        bool PredicateToData(DataConvertArgument arg, MethodCallExpression call);
        ExpressionNode ToData(DataConvertArgument arg, MethodCallExpression call);
    }
}
