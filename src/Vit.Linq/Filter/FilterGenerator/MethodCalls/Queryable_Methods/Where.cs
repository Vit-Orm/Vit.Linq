using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Vit.Linq.Filter.ComponentModel;
using Vit.Linq.Filter.MethodCalls;

namespace Vit.Linq.Filter.FilterGenerator.MethodCalls.Queryable_Methods
{

    /// <summary>
    /// Queryable.Where
    /// </summary>
    public class Where : MethodConvertor_Common
    {
        public override Type methodType { get; } = typeof(Queryable);
        public override FilterRule ToData(FilterGenerateArgument arg, MethodCallExpression call)
        {
            // method.Arguments   [0]: expression       [1]: Where
            var expression = call.Arguments[0];
            var where = call.Arguments[1];

            if (expression is ConstantExpression)
            {
                return arg.convertService.ConvertToData(arg, where);
            }

            return new FilterRule
            {
                condition = "and",
                rules = new List<FilterRule> {
                    arg.convertService.ConvertToData(arg, expression),
                    arg.convertService.ConvertToData(arg, where)
                }
            };
        }
    }

}
