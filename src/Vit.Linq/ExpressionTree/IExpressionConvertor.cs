using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq.ExpressionTree
{
    public interface IExpressionConvertor
    {
        ExpressionNode ConvertToData(DataConvertArgument arg, Expression expression);
        Expression ConvertToCode(CodeConvertArgument arg, ExpressionNode data);
    }
}
