using System.Linq.Expressions;

using Vit.Linq.Filter.ComponentModel;
using Vit.Linq.Filter.MethodCalls;

namespace Vit.Linq.Filter.FilterGenerator
{
    public interface IFilterGenerator
    {
        int priority { get; }
        FilterRule ConvertToData(FilterGenerateArgument arg, Expression expression);
    }
}
