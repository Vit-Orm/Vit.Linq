using System.Collections.Generic;
using System.Linq;

using Vit.Linq.ComponentModel;
using Vit.Linq.Filter;
using Vit.Linq.Filter.ComponentModel;


namespace Vit.Linq
{

    public static partial class Queryable_Extensions
    {
        public static PageData<T> ToPageData<T>(this IQueryable<T> query, PageInfo page)
        {
            if (query == null) return null;

            return new PageData<T>(page) { totalCount = query.Count(), items = query.Page(page).ToList() };
        }

        public static PageData<T> ToPageData<T>(this IQueryable<T> query, IEnumerable<OrderField> orders, PageInfo page)
        {
            return ToPageData(query?.OrderBy(orders), page);
        }



        public static PageData<T> ToPageData<T>(this IQueryable<T> query, FilterRule filter, IEnumerable<OrderField> orders, PageInfo page, FilterService filterService = null)
        {
            return ToPageData(query?.Where(filter, filterService)?.OrderBy(orders), page);
        }


        public static PageData<T> ToPageData<T>(this IQueryable<T> query, FilterRule filter, PageInfo page, FilterService filterService = null)
        {
            return ToPageData(query?.Where(filter, filterService), page);
        }

        public static PageData<T> ToPageData<T>(this IQueryable<T> query, PagedQuery pagedQuery, FilterService filterService = null)
        {
            return ToPageData(query, pagedQuery?.filter, pagedQuery?.orders, pagedQuery.page, filterService);
        }
    }
}