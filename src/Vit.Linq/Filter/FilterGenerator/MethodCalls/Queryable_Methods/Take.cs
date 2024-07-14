using System;
using System.Linq;
using System.Linq.Expressions;

using Vit.Linq.Filter.ComponentModel;
using Vit.Linq.Filter.MethodCalls;

namespace Vit.Linq.Filter.FilterGenerator.MethodCalls.Queryable_Methods
{
    /// <summary>
    /// Queryable.Take
    /// </summary>
    public class Take : MethodConvertor_Common
    {
        public override Type methodType { get; } = typeof(Queryable);
        public override FilterRule ToData(FilterGenerateArgument arg, MethodCallExpression call)
        {
            // method.Arguments   [0]: expression       [1]: takeCount
            var expression = call.Arguments[0];
            var take = (int)arg.convertService.GetValue(call.Arguments[1]);
            arg.queryAction.take = take;

            return arg.convertService.ConvertToData(arg, expression);
        }
    }

}
