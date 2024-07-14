using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Vit.Linq.ComponentModel;
using Vit.Linq.Filter.ComponentModel;
using Vit.Linq.Filter.MethodCalls;

namespace Vit.Linq.Filter.FilterGenerator.MethodCalls.Queryable_Methods
{
    /// <summary>
    /// Queryable.OrderBy
    /// </summary>
    public class OrderBy : IMethodConvertor
    {
        public virtual int priority { get; set; } = 100;

        public Type methodType { get; } = typeof(Queryable);

        static readonly List<string> methodNames = new List<string> { nameof(Queryable.OrderBy), nameof(Queryable.OrderByDescending), nameof(Queryable.ThenBy), nameof(Queryable.ThenByDescending) };
        public virtual bool PredicateToData(FilterGenerateArgument arg, MethodCallExpression call)
        {
            return methodType == call.Method.DeclaringType && methodNames.Contains(call.Method.Name);
        }

        public FilterRule ToData(FilterGenerateArgument arg, MethodCallExpression call)
        {
            var queryAction = arg.queryAction;

            // method.Arguments   [0]: expression       [1]: takeCount
            var expression = call.Arguments[0];
            var memberName = arg.convertService.GetMemberName(call.Arguments[1]);

            if (queryAction.orders is not OrderField[])
            {
                List<OrderField> orders = queryAction.orders as List<OrderField>;
                if (orders == null)
                {
                    queryAction.orders = orders = new List<OrderField>();
                }

                var methodName = call.Method.Name;
                var orderParam = new OrderField(memberName, !methodName.EndsWith("Descending"));

                orders.Insert(0, orderParam);

                if (methodName.StartsWith("Order"))
                {
                    queryAction.orders = orders.ToArray();
                }

            }

            return arg.convertService.ConvertToData(arg, expression);
        }
    }

}
