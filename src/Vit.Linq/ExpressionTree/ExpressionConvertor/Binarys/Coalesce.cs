using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq.ExpressionTree.ExpressionConvertor.Binarys
{

    public class Coalesce : IExpressionConvertor
    {
        public ExpressionNode ConvertToData(DataConvertArgument arg, Expression expression)
        {
            // a??b
            if (expression is BinaryExpression binary && binary.NodeType == ExpressionType.Coalesce)
            {
                var left = arg.convertService.ConvertToData(arg, binary.Left);
                var right = arg.convertService.ConvertToData(arg, binary.Right);

                return ExpressionNode.Binary(nameof(ExpressionType.Coalesce), left: left, right: right);
            }
            return null;
        }

        public Expression ConvertToCode(CodeConvertArgument arg, ExpressionNode data)
        {
            // a??b
            if (data.nodeType == ExpressionType.Coalesce.ToString())
            {
                var left = arg.convertService.ToExpression(arg, data.left) ?? Expression.Constant(null);
                var right = arg.convertService.ToExpression(arg, data.right) ?? Expression.Constant(null);
                return Expression.Coalesce(left, right);
            }
            return null;
        }
    }
}
