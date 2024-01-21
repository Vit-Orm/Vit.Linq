using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using Vit.Linq.ComponentModel;


namespace Vit.Extensions.Linq_Extensions
{

    public static partial class Queryable_ToPageData_Extensions
    {

        #region ToPageData
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PageData<T> ToPageData<T>(this IQueryable<T> query, PageInfo page)
            where T : class
        {
            if (query == null) return null;

            var queryPaged = query;
            if (page != null)
                queryPaged = queryPaged.Page(page);

            return new PageData<T>(page) { totalCount = query.Count(), rows = queryPaged.ToList() };
        }

        public static PageData<T> ToPageData<T>(this IQueryable<T> query, IEnumerable<SortItem> sort, PageInfo page
        ) where T : class
        {
            return query?.Sort(sort).ToPageData(page);
        }
        #endregion


        #region ToPageData with selector
        /// <summary>
        ///  filter first, then ToList, then invoke selector
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <param name="page"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PageData<TResult> ToPageData<T, TResult>(this IQueryable<T> query, PageInfo page, Func<T, TResult> selector)
            where T : class
        {
            if (query == null) return null;

            var queryPaged = query;
            if (page != null)
                queryPaged = queryPaged.Page(page);

            return new PageData<TResult>(page) { totalCount = query.Count(), rows = queryPaged.ToList().Select(selector).ToList() };
        }

        /// <summary>
        ///  filter first, then ToList, then invoke selector
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <param name="sort"></param>
        /// <param name="page"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PageData<TResult> ToPageData<T, TResult>(this IQueryable<T> query, IEnumerable<SortItem> sort, PageInfo page, Func<T, TResult> selector
        ) where T : class
        {
            return query?.Sort(sort).ToPageData(page, selector);
        }
        #endregion



    }
}