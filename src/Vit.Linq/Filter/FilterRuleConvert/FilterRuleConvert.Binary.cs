using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Vit.Linq.Filter.ComponentModel;

namespace Vit.Linq.Filter
{
    public partial class FilterRuleConvert
    {

        protected virtual FilterRule ConvertToData_Binary(QueryAction queryAction, BinaryExpression binary)
        {
            switch (binary.NodeType)
            {
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                    string opt = null;
                    switch (binary.NodeType)
                    {
                        case ExpressionType.Equal: opt = "="; break;
                        case ExpressionType.NotEqual: opt = "!="; break;
                        case ExpressionType.GreaterThan: opt = ">"; break;
                        case ExpressionType.GreaterThanOrEqual: opt = ">="; break;
                        case ExpressionType.LessThan: opt = "<"; break;
                        case ExpressionType.LessThanOrEqual: opt = "<="; break;
                    }
                    var left = GetMemberName(binary.Left);
                    var right = GetValue(binary.Right);
                    return new FilterRule { field = left?.ToString(), @operator = opt, value = right };

                case ExpressionType.AndAlso:
                    return new FilterRule { condition = "and", rules = new List<FilterRule> { ConvertToFilterRule(queryAction, binary.Left), ConvertToFilterRule(queryAction, binary.Right) } };

                case ExpressionType.OrElse:
                    return new FilterRule { condition = "or", rules = new List<FilterRule> { ConvertToFilterRule(queryAction, binary.Left), ConvertToFilterRule(queryAction, binary.Right) } };
            }
            throw new NotSupportedException($"Unsupported binary operator: {binary.NodeType}");
        }


    }
}
