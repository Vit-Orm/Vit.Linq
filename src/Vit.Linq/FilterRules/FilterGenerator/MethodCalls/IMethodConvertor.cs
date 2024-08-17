using System.Linq.Expressions;

using Vit.Linq.FilterRules.ComponentModel;

namespace Vit.Linq.FilterRules.MethodCalls
{
    public interface IMethodConvertor
    {
        int priority { get; }

        bool PredicateToData(FilterGenerateArgument arg, MethodCallExpression call);
        FilterRule ToData(FilterGenerateArgument arg, MethodCallExpression call);
    }
}
