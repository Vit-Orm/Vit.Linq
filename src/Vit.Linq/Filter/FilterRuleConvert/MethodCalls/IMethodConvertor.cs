using System.Linq.Expressions;
using Vit.Linq.ExpressionTree.ComponentModel;
using Vit.Linq.Filter.ComponentModel;

namespace Vit.Linq.Filter.MethodCalls
{
    public interface IMethodConvertor
    {
        int priority { get; }

        bool PredicateToData(DataConvertArgument arg, MethodCallExpression call);
        FilterRule ToData(DataConvertArgument arg, MethodCallExpression call);
    }
}
