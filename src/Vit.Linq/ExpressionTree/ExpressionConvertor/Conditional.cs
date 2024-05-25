using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq.ExpressionTree.ExpressionConvertor
{

    /// <summary>
    ///  a > b ? a : b
    ///  Node arguments: [  condition, valueForTrue, valueForFalse ]
    /// </summary>
    public class Conditional : IExpressionConvertor
    {
        public ExpressionNode ConvertToData(DataConvertArgument arg, Expression expression)
        {
            if (expression is ConditionalExpression conditionExp)
            {
                var condition = arg.convertService.ConvertToData(arg, conditionExp.Test);
                var valueForTrue = arg.convertService.ConvertToData(arg, conditionExp.IfTrue);
                var valueForFalse = arg.convertService.ConvertToData(arg, conditionExp.IfFalse);

                return new ExpressionNode
                {
                    nodeType = "Conditional",
                    arguments = new ExpressionNode[] { condition, valueForTrue, valueForFalse }
                };
            }
            return null;
        }

        public Expression ConvertToCode(CodeConvertArgument arg, ExpressionNode data)
        {
            if (data.nodeType == "Conditional")
            {
                var condition = arg.convertService.ToExpression(arg, data.arguments[0]);
                var valueForTrue = arg.convertService.ToExpression(arg, data.arguments[1]);
                var valueForFalse = arg.convertService.ToExpression(arg, data.arguments[2]);

                return Expression.Condition(condition, valueForTrue, valueForFalse);
            }
            return null;
        }
    }
}
