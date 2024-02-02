using System.Collections.Generic;
using System.Linq;

using Vit.Linq.ComponentModel;


namespace Vit.Extensions.Linq_Extensions
{

    public static partial class IQueryable_ToRangeData_Extensions
    {

        public static RangeData<T> IQueryable_ToRangeData<T>(this IQueryable query, RangeInfo range, IEnumerable<OrderField> orders = null)
        {
            if (query == null) return null;

            return new RangeData<T>(range) { totalCount = query.IQueryable_Count(), items = query.IQueryable_OrderBy(orders).IQueryable_Range(range).IQueryable_ToList<T>() };
        }

        public static RangeData<T> IQueryable_ToRangeData<T>(this IQueryable query, PageInfo page, IEnumerable<OrderField> orders = null)
        {
            return IQueryable_ToRangeData<T>(query, page.ToRange(), orders);
        }


    }
}