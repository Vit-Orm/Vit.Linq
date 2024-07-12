using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Vit.Linq.Filter.ComponentModel;

namespace Vit.Linq.Filter.MethodCalls.Queryable_Extensions_Methods
{


    /// <summary>
    /// Queryable_Extensions  TotalCount / ToListAndTotalCount
    /// </summary>
    public class Queryable_Extensions : IMethodConvertor
    {
        public virtual int priority { get; set; } = 100;

        public Type methodType { get; } = typeof(Vit.Linq.Queryable_Extensions);

        static readonly List<string> methodNames = new List<string> { nameof(Vit.Linq.Queryable_Extensions.TotalCount), nameof(Vit.Linq.Queryable_Extensions.ToListAndTotalCount) };
        public virtual bool PredicateToData(DataConvertArgument arg, MethodCallExpression call)
        {
            return methodType == call.Method.DeclaringType && methodNames.Contains(call.Method.Name);
        }

        public FilterRule ToData(DataConvertArgument arg, MethodCallExpression call)
        {
            // method.Arguments   [0]: expression
            var expression = call.Arguments[0];
            arg.queryAction.method = call.Method.Name;

            return arg.convertService.ConvertToFilterRule(arg.queryAction, expression);
        }
    }


}
