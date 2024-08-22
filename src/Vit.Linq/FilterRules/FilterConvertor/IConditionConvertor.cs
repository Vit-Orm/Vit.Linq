using System.Linq.Expressions;

using Vit.Linq.FilterRules.ComponentModel;

namespace Vit.Linq.FilterRules.FilterConvertor
{
    public interface IConditionConvertor
    {
        int priority { get; }
        (bool success, Expression expression) ConvertToCode(FilterConvertArgument arg, IFilterRule filter, string condition);
    }
}
