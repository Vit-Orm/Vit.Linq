using System.Linq.Expressions;

using Vit.Linq.Filter.ComponentModel;

namespace Vit.Linq.Filter.MethodCalls
{
    public interface IMethodConvertor
    {
        int priority { get; }

        bool PredicateToData(FilterGenerateArgument arg, MethodCallExpression call);
        FilterRule ToData(FilterGenerateArgument arg, MethodCallExpression call);
    }
}
