using System.Linq.Expressions;

using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq.ExpressionTree.ExpressionConvertor
{

    /// <summary>
    ///  a > b ? a : b
    /// </summary>
    public class Conditional : IExpressionConvertor
    {
        public virtual int priority { get; set; } = 100;
        public ExpressionNode ConvertToData(ToDataArgument arg, Expression expression)
        {
            if (expression is ConditionalExpression conditionExp)
            {
                var test = arg.convertService.ConvertToData(arg, conditionExp.Test);
                var ifTrue = arg.convertService.ConvertToData(arg, conditionExp.IfTrue);
                var ifFalse = arg.convertService.ConvertToData(arg, conditionExp.IfFalse);

                return ExpressionNode.Conditional(test: test, ifTrue: ifTrue, ifFalse: ifFalse);
            }
            return null;
        }

        public Expression ConvertToCode(ToCodeArgument arg, ExpressionNode data)
        {
            if (data.nodeType == nameof(ExpressionType.Conditional))
            {
                ExpressionNode_Conditional conditional = data;

                var test = arg.convertService.ConvertToCode(arg, conditional.Conditional_GetTest());
                var ifTrue = arg.convertService.ConvertToCode(arg, conditional.Conditional_GetIfTrue());
                var ifFalse = arg.convertService.ConvertToCode(arg, conditional.Conditional_GetIfFalse());

                return Expression.Condition(test, ifTrue, ifFalse);
            }
            return null;
        }
    }
}
