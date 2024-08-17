using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Vit.Linq.ExpressionNodes;
using Vit.Linq.ExpressionNodes.ComponentModel;

namespace Vit.Linq
{
    public static partial class IQueryable_Extensions
    {

        public static IQueryable OrderBy(this IQueryable query, IEnumerable<ExpressionNodeOrderField> orders, ExpressionConvertService expressionConvertService = null)
        {
            return Extensions_OrderByExpressionNode.MethodInfo_OrderBy(query.ElementType).Invoke(null, new object[] { query, orders, expressionConvertService }) as IOrderedQueryable;
        }

        static class Extensions_OrderByExpressionNode
        {
            private static MethodInfo MethodInfo_OrderBy_;
            public static MethodInfo MethodInfo_OrderBy(Type elementType) =>
                 (MethodInfo_OrderBy_ ??=
                     new Func<IQueryable<object>, IEnumerable<ExpressionNodeOrderField>, ExpressionConvertService, IQueryable<object>>(Queryable_Extensions.OrderBy<object>)
                     .GetMethodInfo().GetGenericMethodDefinition())
                 .MakeGenericMethod(elementType);
        }


    }
}