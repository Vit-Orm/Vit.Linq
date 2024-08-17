using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Vit.Linq.FilterRules.ComponentModel;
using Vit.Linq.FilterRules.MethodCalls;

namespace Vit.Linq.FilterRules.FilterGenerator
{

    public class Binary : IFilterGenerator
    {
        public virtual int priority { get; set; } = 200;

        public FilterRule ConvertToData(FilterGenerateArgument arg, Expression expression)
        {
            if (expression is not BinaryExpression binary) return null;

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
                    var left = arg.convertService.GetMemberName(binary.Left);
                    var right = arg.convertService.GetValue(binary.Right);
                    return new FilterRule { field = left?.ToString(), @operator = opt, value = right };
                case ExpressionType.AndAlso:
                    return new FilterRule
                    {
                        condition = "and",
                        rules = new List<FilterRule> { arg.convertService.ConvertToData(arg, binary.Left), arg.convertService.ConvertToData(arg, binary.Right) }
                    };

                case ExpressionType.OrElse:
                    return new FilterRule
                    {
                        condition = "or",
                        rules = new List<FilterRule> { arg.convertService.ConvertToData(arg, binary.Left), arg.convertService.ConvertToData(arg, binary.Right) }
                    };
            }

            throw new NotSupportedException($"Unsupported binary operator: {binary.NodeType}");

        }


    }
}
