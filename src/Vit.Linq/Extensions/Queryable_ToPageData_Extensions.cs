using System.Collections.Generic;
using System.Linq;

using Vit.Linq.ComponentModel;


namespace Vit.Extensions.Linq_Extensions
{

    public static partial class Queryable_ToPageData_Extensions
    {

        public static PageData<T> ToPageData<T>(this IQueryable<T> query, PageInfo page, IEnumerable<OrderField> orders = null)
        {
            if (query == null) return null;

            return new PageData<T>(page) { totalCount = query.Count(), items = query.OrderBy(orders).Page(page).ToList() };
        }

    }
}