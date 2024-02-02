using System;
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
                return (IOrderedQueryable<T>)MethodInfo_OrderBy(typeof(T), exp.ReturnType).Invoke(null, new object[] { query, exp });
            }
            else
            {
                return (IOrderedQueryable<T>)MethodInfo_OrderByDescending(typeof(T), exp.ReturnType).Invoke(null, new object[] { query, exp });
            }
        }

        internal static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> query, LambdaExpression exp, bool asc = true)
        {
            if (asc)
            {
                return (IOrderedQueryable<T>)MethodInfo_ThenBy(typeof(T), exp.ReturnType).Invoke(null, new object[] { query, exp });
            }
            else
            {
                return (IOrderedQueryable<T>)MethodInfo_ThenByDescending(typeof(T), exp.ReturnType).Invoke(null, new object[] { query, exp });
            }
        }
        #endregion


        #region  CachedMethodInfo

        private static   MethodInfo MethodInfo_OrderBy_;
        static MethodInfo MethodInfo_OrderBy(Type sourceType, Type returnType) =>
            (MethodInfo_OrderBy_ ??=
                new Func<IQueryable<object>, Expression<Func<object, object>>, IOrderedQueryable<object>>(Queryable.OrderBy<object, object>)
                .GetMethodInfo().GetGenericMethodDefinition())
            .MakeGenericMethod(sourceType, returnType);


        private static MethodInfo MethodInfo_OrderByDescending_;
        static MethodInfo MethodInfo_OrderByDescending(Type sourceType, Type returnType) =>
            (MethodInfo_OrderByDescending_ ??=
                new Func<IQueryable<object>, Expression<Func<object, object>>, IOrderedQueryable<object>>(Queryable.OrderByDescending<object, object>)
                .GetMethodInfo().GetGenericMethodDefinition())
            .MakeGenericMethod(sourceType, returnType);


        private static MethodInfo MethodInfo_ThenBy_;
        static MethodInfo MethodInfo_ThenBy(Type sourceType, Type returnType) =>
            (MethodInfo_ThenBy_ ??=
                new Func<IOrderedQueryable<object>, Expression<Func<object, object>>, IOrderedQueryable<object>>(Queryable.ThenBy<object, object>)
                .GetMethodInfo().GetGenericMethodDefinition())
            .MakeGenericMethod(sourceType, returnType);

        private static MethodInfo MethodInfo_ThenByDescending_;
        static MethodInfo MethodInfo_ThenByDescending(Type sourceType, Type returnType) =>
            (MethodInfo_ThenByDescending_ ??=
                new Func<IOrderedQueryable<object>, Expression<Func<object, object>>, IOrderedQueryable<object>>(Queryable.ThenByDescending<object, object>)
                .GetMethodInfo().GetGenericMethodDefinition())
            .MakeGenericMethod(sourceType, returnType); 

        #endregion

    }
}