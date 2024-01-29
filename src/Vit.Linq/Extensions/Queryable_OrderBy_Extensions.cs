using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

using Vit.Linq;
using Vit.Linq.ComponentModel;

namespace Vit.Extensions.Linq_Extensions
{
    /// <summary>
    /// ref https://www.cnblogs.com/Extnet/p/9848272.html
    /// </summary>
    public static partial class Queryable_OrderBy_Extensions
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, IEnumerable<OrderField> orders)
        {
            if (query == null || orders?.Any() != true) return query;

            var paramExp = Expression.Parameter(typeof(T));
            IOrderedQueryable<T> orderedQuery = null;

            foreach (var item in orders)
            {
                var memberExp = LinqHelp.GetFieldMemberExpression(paramExp, item.field);
                LambdaExpression exp = Expression.Lambda(memberExp, paramExp);

                if (orderedQuery == null)
                {
                    orderedQuery = OrderBy(query, exp, item.asc);
                }
                else
                {
                    orderedQuery = ThenBy(orderedQuery, exp, item.asc);
                }
            }
            return orderedQuery;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="field"></param>
        /// <param name="asc"> whether sort by asc</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, string field, bool asc = true)
        {
            if (query == null || string.IsNullOrEmpty(field)) return query;

            return OrderBy(query, new[] { new OrderField(field, asc) });
        }





        #region OrderBy
        internal static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> query, LambdaExpression exp, bool asc = true)
        {
            if (asc)
            {
                var genericMethod = MethodInfo_OrderBy.MakeGenericMethod(typeof(T), exp.ReturnType);
                return (IOrderedQueryable<T>)genericMethod.Invoke(null, new object[] { query, exp });
            }
            else
            {
                var genericMethod = MethodInfo_OrderByDescending.MakeGenericMethod(typeof(T), exp.ReturnType);
                return (IOrderedQueryable<T>)genericMethod.Invoke(null, new object[] { query, exp });
            }
        }

        internal static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> query, LambdaExpression exp, bool asc = true)
        {
            if (asc)
            {
                var genericMethod = MethodInfo_ThenBy.MakeGenericMethod(typeof(T), exp.ReturnType);
                return (IOrderedQueryable<T>)genericMethod.Invoke(null, new object[] { query, exp });
            }
            else
            {
                var genericMethod = MethodInfo_ThenByDescending.MakeGenericMethod(typeof(T), exp.ReturnType);
                return (IOrderedQueryable<T>)genericMethod.Invoke(null, new object[] { query, exp });
            }
        }

        static readonly MethodInfo MethodInfo_OrderBy = typeof(Queryable).GetMethods().FirstOrDefault(m => m.Name == "OrderBy" && m.GetParameters().Length == 2);
        static readonly MethodInfo MethodInfo_OrderByDescending = typeof(Queryable).GetMethods().FirstOrDefault(m => m.Name == "OrderByDescending" && m.GetParameters().Length == 2);
        static readonly MethodInfo MethodInfo_ThenBy = typeof(Queryable).GetMethods().FirstOrDefault(m => m.Name == "ThenBy" && m.GetParameters().Length == 2);
        static readonly MethodInfo MethodInfo_ThenByDescending = typeof(Queryable).GetMethods().FirstOrDefault(m => m.Name == "ThenByDescending" && m.GetParameters().Length == 2);

        #endregion

    }
}