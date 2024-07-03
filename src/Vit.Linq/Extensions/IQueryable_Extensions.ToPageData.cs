using System.Collections.Generic;
using System.Linq;

using Vit.Linq.ComponentModel;
using Vit.Linq.Filter.ComponentModel;

namespace Vit.Linq
{

    public static partial class IQueryable_Extensions
    {

        public static PageData<T> IQueryable_ToPageData<T>(this IQueryable query, PageInfo page)
        {
            if (query == null) return null;

            return new PageData<T>(page) { totalCount = query.IQueryable_Count(), items = query.IQueryable_Page(page).IQueryable_ToList<T>() };
        }

        public static PageData<T> IQueryable_ToPageData<T>(this IQueryable query, IEnumerable<OrderField> orders, PageInfo page)
        {
            return IQueryable_ToPageData<T>(query?.IQueryable_OrderBy(orders), page);
        }



        public static PageData<T> IQueryable_ToPageData<T>(this IQueryable query, FilterRule filter, IEnumerable<OrderField> orders, PageInfo page)
        {
            return IQueryable_ToPageData<T>(query?.IQueryable_Where(filter)?.IQueryable_OrderBy(orders), page);
        }


        public static PageData<T> IQueryable_ToPageData<T>(this IQueryable query, FilterRule filter, PageInfo page)
        {
            return IQueryable_ToPageData<T>(query?.IQueryable_Where(filter), page);
        }
    }
}