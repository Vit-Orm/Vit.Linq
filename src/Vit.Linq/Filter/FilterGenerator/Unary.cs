using System;
using System.Linq.Expressions;

using Vit.Linq.Filter.ComponentModel;
using Vit.Linq.Filter.MethodCalls;

namespace Vit.Linq.Filter.FilterGenerator
{

    public class Unary : IFilterGenerator
    {
        public virtual int priority { get; set; } = 300;

        public FilterRule ConvertToData(FilterGenerateArgument arg, Expression expression)
        {
            if (expression is not UnaryExpression unary) return null;

            switch (unary.NodeType)
            {
                case ExpressionType.Not:
                    var rule = arg.convertService.ConvertToData(arg, unary.Operand);
                    if (rule.condition?.StartsWith(RuleCondition.Not, StringComparison.OrdinalIgnoreCase) == true)
                    {
                        rule.condition = rule.condition.Substring(3);
                    }
                    else
                    {
                        rule.condition = RuleCondition.Not + rule.condition;
                    }
                    return rule;
                case ExpressionType.Quote:
                    var exp = unary.Operand;
                    return arg.convertService.ConvertToData(arg, exp);
            }
            throw new NotSupportedException($"Unsupported binary operator: {unary.NodeType}");

        }


    }
}
