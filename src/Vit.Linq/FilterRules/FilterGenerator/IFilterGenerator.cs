using System.Linq.Expressions;

using Vit.Linq.FilterRules.ComponentModel;
using Vit.Linq.FilterRules.MethodCalls;

namespace Vit.Linq.FilterRules.FilterGenerator
{
    public interface IFilterGenerator
    {
        int priority { get; }
        FilterRule ConvertToData(FilterGenerateArgument arg, Expression expression);
    }
}
