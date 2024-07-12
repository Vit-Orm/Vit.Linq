using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Vit.Linq.ExpressionTree;
using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq
{
    public static partial class Queryable_Extensions
    {

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, IEnumerable<ExpressionNodeOrderField> orders, ExpressionConvertService Instance = null)
        {
            if (query == null || orders?.Any() != true) return query;

            IOrderedQueryable<T> orderedQuery = null;
            Instance ??= ExpressionConvertService.Instance;

            foreach (var item in orders)
            {
                LambdaExpression exp = Instance.ConvertToCode_LambdaExpression(item.member, typeof(T));

                if (orderedQuery == null)
                {
                    orderedQuery = Extensions_OrderByExpressionNode.OrderBy(query, exp, item.asc);
                }
                else
                {
                    orderedQuery = Extensions_OrderByExpressionNode.ThenBy(orderedQuery, exp, item.asc);
                }
            }
            return orderedQuery;
        }

        internal static class Extensions_OrderByExpressionNode
        {

            #region OrderBy
            internal static IOrderedQueryable<T> OrderBy<T>(IQueryable<T> query, LambdaExpression exp, bool asc = true)
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

            internal static IOrderedQueryable<T> ThenBy<T>(IOrderedQueryable<T> query, LambdaExpression exp, bool asc = true)
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

            private static MethodInfo MethodInfo_OrderBy_;
            static MethodInfo MethodInfo_OrderBy(Type sourceType, Type returnType) =>
                (MethodInfo_OrderBy_ ??=
                    new Func<IQueryable<object>, Expression<Func<object, string>>, IOrderedQueryable<object>>(System.Linq.Queryable.OrderBy<object, string>)
                    .GetMethodInfo().GetGenericMethodDefinition())
                .MakeGenericMethod(sourceType, returnType);


            private static MethodInfo MethodInfo_OrderByDescending_;
            static MethodInfo MethodInfo_OrderByDescending(Type sourceType, Type returnType) =>
                (MethodInfo_OrderByDescending_ ??=
                    new Func<IQueryable<object>, Expression<Func<object, string>>, IOrderedQueryable<object>>(System.Linq.Queryable.OrderByDescending<object, string>)
                    .GetMethodInfo().GetGenericMethodDefinition())
                .MakeGenericMethod(sourceType, returnType);


            private static MethodInfo MethodInfo_ThenBy_;
            static MethodInfo MethodInfo_ThenBy(Type sourceType, Type returnType) =>
                (MethodInfo_ThenBy_ ??=
                    new Func<IOrderedQueryable<object>, Expression<Func<object, string>>, IOrderedQueryable<object>>(System.Linq.Queryable.ThenBy<object, string>)
                    .GetMethodInfo().GetGenericMethodDefinition())
                .MakeGenericMethod(sourceType, returnType);

            private static MethodInfo MethodInfo_ThenByDescending_;
            static MethodInfo MethodInfo_ThenByDescending(Type sourceType, Type returnType) =>
                (MethodInfo_ThenByDescending_ ??=
                    new Func<IOrderedQueryable<object>, Expression<Func<object, string>>, IOrderedQueryable<object>>(System.Linq.Queryable.ThenByDescending<object, string>)
                    .GetMethodInfo().GetGenericMethodDefinition())
                .MakeGenericMethod(sourceType, returnType);

            #endregion

        }
    }
}