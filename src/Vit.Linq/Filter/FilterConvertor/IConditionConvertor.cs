using System.Linq.Expressions;

using Vit.Linq.Filter.ComponentModel;

namespace Vit.Linq.Filter.FilterConvertor
{
    public interface IConditionConvertor
    {
        int priority { get; }
        (bool success, Expression expression) ConvertToCode(FilterConvertArgument arg, IFilterRule filter,string condition);
    }
}
