using System.Collections.Generic;
using System.Linq;

using Vit.Linq.ComponentModel;

namespace Vit.Extensions.Linq_Extensions
{

    public static partial class IQueryable_ToPageData_Extensions
    {

        public static PageData<T> IQueryable_ToPageData<T>(this IQueryable query, PageInfo page, IEnumerable<OrderField> orders = null)
        {
            if (query == null) return null;

            return new PageData<T>(page) { totalCount = query.IQueryable_Count(), items = query.IQueryable_OrderBy(orders).IQueryable_Page(page).IQueryable_ToList<T>() };
        }
    }
}