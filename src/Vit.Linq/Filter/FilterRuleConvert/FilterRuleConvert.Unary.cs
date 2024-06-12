using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Vit.Extensions.Linq_Extensions;
using Vit.Linq.ComponentModel;
using Vit.Linq.Filter.ComponentModel;

namespace Vit.Linq.Filter
{
    public partial class FilterRuleConvert
    {
        protected virtual FilterRule ConvertToData_Unary(QueryAction queryAction, UnaryExpression unary)
        {
            switch (unary.NodeType)
            {
                case ExpressionType.Not:
                    var rule = ConvertToFilterRule(queryAction, unary.Operand);
                    if (rule.condition?.StartsWith(RuleCondition.Not, StringComparison.OrdinalIgnoreCase) == true)
                    {
                        rule.condition = rule.condition.Substring(3);
                    }
                    else
                    {
                        rule.condition = RuleCondition.Not;
                    }
                    return rule;
                case ExpressionType.Quote:
                    var exp = unary.Operand;
                    return ConvertToFilterRule(queryAction, exp);
            }
            throw new NotSupportedException($"Unsupported binary operator: {unary.NodeType}");
        }

    }
}
