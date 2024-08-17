using System;
using System.Linq;
using System.Linq.Expressions;

using Vit.Linq.FilterRules.ComponentModel;
using Vit.Linq.FilterRules.MethodCalls;

namespace Vit.Linq.FilterRules.FilterGenerator.MethodCalls.Queryable_Methods
{

    /// <summary>
    /// Queryable.Skip
    /// </summary>
    public class Skip : MethodConvertor_Common
    {
        public override Type methodType { get; } = typeof(Queryable);
        public override FilterRule ToData(FilterGenerateArgument arg, MethodCallExpression call)
        {
            // method.Arguments   [0]: expression       [1]: skipCount
            var expression = call.Arguments[0];
            var skip = (int)arg.convertService.GetValue(call.Arguments[1]);
            arg.queryAction.skip = skip;

            return arg.convertService.ConvertToData(arg, expression);
        }
    }

}
