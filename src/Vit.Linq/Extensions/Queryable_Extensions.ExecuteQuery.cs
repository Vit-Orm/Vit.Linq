using System.Linq;

using Vit.Linq.ComponentModel;
using Vit.Linq.FilterRules;


namespace Vit.Linq
{

    public static partial class Queryable_Extensions
    {
        /// <summary>
        /// using ToListAndTotalCount to query data and totalCount
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="pagedQuery"></param>
        /// <param name="filterService"></param>
        /// <returns></returns>
        public static PageData<T> ExecuteQuery<T>(this IQueryable<T> query, PagedQuery pagedQuery, FilterService filterService = null)
        {
            if (query == null) return null;

            query = query.Where(pagedQuery?.filter, filterService).OrderBy(pagedQuery?.orders).Page(pagedQuery?.page);

            var data = new PageData<T>(pagedQuery?.page);

            (data.items, data.totalCount) = query.ToListAndTotalCount();

            return data;
        }

        /// <summary>
        /// using ToListAndTotalCount to query data and totalCount
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="rangedQuery"></param>
        /// <param name="filterService"></param>
        /// <returns></returns>
        public static RangeData<T> ExecuteQuery<T>(this IQueryable<T> query, RangedQuery rangedQuery, FilterService filterService = null)
        {
            if (query == null) return null;

            query = query.Where(rangedQuery?.filter, filterService).OrderBy(rangedQuery?.orders).Range(rangedQuery?.range);

            var data = new RangeData<T>(rangedQuery?.range);

            (data.items, data.totalCount) = query.ToListAndTotalCount();

            return data;
        }

    }
}