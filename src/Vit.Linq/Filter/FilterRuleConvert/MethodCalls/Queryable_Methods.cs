using System;
using System.Linq.Expressions;
using System.Linq;
using System.Collections.Generic;
using Vit.Linq.Filter.ComponentModel;
using Vit.Linq.ComponentModel;

namespace Vit.Linq.Filter.MethodCalls.Queryable_Methods
{



    #region Take Skip
    /// <summary>
    /// Queryable.Take
    /// </summary>
    public class Take : MethodConvertor_Common
    {
        public override Type methodType { get; } = typeof(Queryable);
        public override FilterRule ToData(DataConvertArgument arg, MethodCallExpression call)
        {
            // method.Arguments   [0]: expression       [1]: takeCount
            var expression = call.Arguments[0];
            var take = (int)arg.convertService.GetValue(call.Arguments[1]);
            arg.queryAction.take = take;

            return arg.convertService.ConvertToFilterRule(arg.queryAction, expression);
        }
    }


    /// <summary>
    /// Queryable.Skip
    /// </summary>
    public class Skip : MethodConvertor_Common
    {
        public override Type methodType { get; } = typeof(Queryable);
        public override FilterRule ToData(DataConvertArgument arg, MethodCallExpression call)
        {
            // method.Arguments   [0]: expression       [1]: skipCount
            var expression = call.Arguments[0];
            var skip = (int)arg.convertService.GetValue(call.Arguments[1]);
            arg.queryAction.skip = skip;

            return arg.convertService.ConvertToFilterRule(arg.queryAction, expression);
        }
    }
    #endregion


    #region OrderBy
    /// <summary>
    /// Queryable.OrderBy
    /// </summary>
    public class OrderBy : IMethodConvertor
    {
        public virtual int priority { get; set; } = 100;

        public Type methodType { get; } = typeof(Queryable);

        static readonly List<string> methodNames = new List<string> { "OrderBy", "OrderByDescending", "ThenBy", "ThenByDescending" };
        public virtual bool PredicateToData(DataConvertArgument arg, MethodCallExpression call)
        {
            return methodType == call.Method.DeclaringType && methodNames.Contains(call.Method.Name);
        }

        public FilterRule ToData(DataConvertArgument arg, MethodCallExpression call)
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

            return arg.convertService.ConvertToFilterRule(queryAction, expression);
        }
    }

    #endregion

    #region Where

    /// <summary>
    /// Queryable.Where
    /// </summary>
    public class Where : MethodConvertor_Common
    {
        public override Type methodType { get; } = typeof(Queryable);
        public override FilterRule ToData(DataConvertArgument arg, MethodCallExpression call)
        {
            // method.Arguments   [0]: expression       [1]: Where
            var expression = call.Arguments[0];
            var where = call.Arguments[1];
            if (expression is ConstantExpression)
            {
                return arg.convertService.ConvertToFilterRule(arg.queryAction, where);
            }

            return new FilterRule
            {
                condition = "and",
                rules = new List<FilterRule> {
                    arg.convertService.ConvertToFilterRule(arg.queryAction, expression),
                    arg.convertService.ConvertToFilterRule(arg.queryAction, where)
                }
            };
        }
    }
    #endregion


    #region Count / First / FirstOrDefault / Last / LastOrDefault
    /// <summary>
    /// Queryable  Count / First / FirstOrDefault / Last / LastOrDefault
    /// </summary>
    public class Others : IMethodConvertor
    {
        public virtual int priority { get; set; } = 100;

        public Type methodType { get; } = typeof(Queryable);

        static readonly List<string> methodNames = new List<string> { "Count", "First", "FirstOrDefault", "Last", "LastOrDefault" };
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
    #endregion

}
