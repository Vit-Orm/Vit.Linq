using System.Linq.Expressions;

using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq.ExpressionTree
{
    public interface IExpressionConvertor
    {
        ExpressionNode ConvertToData(ToDataArgument arg, Expression expression);
        Expression ConvertToCode(ToCodeArgument arg, ExpressionNode data);
    }
}
