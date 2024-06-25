using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Vit.Linq;
using Vit.Linq.ComponentModel;

namespace Vit.Extensions.Linq_Extensions
{

    public static partial class IQueryable_OrderBy_Extensions
    {


        public static IQueryable IQueryable_OrderBy(this IQueryable source, IEnumerable<OrderField> orders)
        {
            if (source == null || orders?.Any() != true) return source;

            #region GetOrderByMethodName
            bool isFirst = true;
            string GetOrderByMethodName(bool asc)
            {
                if (isFirst)
                {
                    isFirst = false;
                    return asc ? "OrderBy" : "OrderByDescending";
                }
                return asc ? "ThenBy" : "ThenByDescending";
            }
            #endregion


            var targetType = source.ElementType;
            ParameterExpression parameter = Expression.Parameter(targetType);

            Expression queryExpr = source.Expression;

            foreach (var item in orders)
            {
                // #1 get memberExp
                Expression memberExp = LinqHelp.GetFieldMemberExpression(parameter, item.field);


                #region #2 call
                queryExpr = Expression.Call(
                  typeof(Queryable), GetOrderByMethodName(item.asc),
                  new Type[] { source.ElementType, memberExp.Type },
                  queryExpr, Expression.Quote(Expression.Lambda(memberExp, parameter)));
                #endregion
            }

            return source.Provider.CreateQuery(queryExpr);
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="field"></param>
        /// <param name="asc"> whether sort by asc</param>
        /// <returns></returns>
        public static IQueryable IQueryable_OrderBy(this IQueryable query, string field, bool asc = true)
        {
            return query.IQueryable_OrderBy(new[] { new OrderField { field = field, asc = asc } });
        }

    }
}