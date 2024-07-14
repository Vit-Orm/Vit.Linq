using System.Linq.Expressions;

namespace Vit.Linq.Filter.FilterConvertor
{
    public interface IOperatorConvertor
    {
        int priority { get; }
        (bool success, Expression expression) ConvertToCode(OperatorConvertArgument arg);
    }
}
