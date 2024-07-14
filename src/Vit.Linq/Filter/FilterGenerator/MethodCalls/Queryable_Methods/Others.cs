using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Vit.Linq.Filter.ComponentModel;
using Vit.Linq.Filter.MethodCalls;

namespace Vit.Linq.Filter.FilterGenerator.MethodCalls.Queryable_Methods
{




    #region Count / First / FirstOrDefault / Last / LastOrDefault
    /// <summary>
    /// Queryable  Count / First / FirstOrDefault / Last / LastOrDefault
    /// </summary>
    public class Others : IMethodConvertor
    {
        public virtual int priority { get; set; } = 100;

        public Type methodType { get; } = typeof(Queryable);

        static readonly List<string> methodNames = new List<string> { nameof(Queryable.Count), nameof(Queryable.First), nameof(Queryable.FirstOrDefault), nameof(Queryable.Last), nameof(Queryable.LastOrDefault) };
        public virtual bool PredicateToData(FilterGenerateArgument arg, MethodCallExpression call)
        {
            return methodType == call.Method.DeclaringType && methodNames.Contains(call.Method.Name);
        }

        public FilterRule ToData(FilterGenerateArgument arg, MethodCallExpression call)
        {
            // method.Arguments   [0]: expression
            var expression = call.Arguments[0];
            arg.queryAction.method = call.Method.Name;

            return arg.convertService.ConvertToData(arg, expression);
        }
    }
    #endregion

}
