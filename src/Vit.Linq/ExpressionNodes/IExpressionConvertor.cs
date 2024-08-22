using System.Linq.Expressions;

using Vit.Linq.ExpressionNodes.ComponentModel;

namespace Vit.Linq.ExpressionNodes
{
    public interface IExpressionConvertor
    {
        int priority { get; }
        ExpressionNode ConvertToData(ToDataArgument arg, Expression expression);
        Expression ConvertToCode(ToCodeArgument arg, ExpressionNode data);
    }
}
