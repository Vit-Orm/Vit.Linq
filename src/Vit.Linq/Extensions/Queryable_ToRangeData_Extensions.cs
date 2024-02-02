using System.Collections.Generic;
using System.Linq;

using Vit.Linq.ComponentModel;


namespace Vit.Extensions.Linq_Extensions
{

    public static partial class Queryable_ToRangeData_Extensions
    {

        public static RangeData<T> ToRangeData<T>(this IQueryable<T> query, RangeInfo range, IEnumerable<OrderField> orders = null)
        {
            if (query == null) return null;

            return new RangeData<T>(range) { totalCount = query.Count(), items = query.OrderBy(orders).Range(range).ToList() };
        }

        public static RangeData<T> ToRangeData<T>(this IQueryable<T> query, PageInfo page, IEnumerable<OrderField> orders = null)
        {
            return ToRangeData(query, page.ToRange(), orders);
        }


    }
}