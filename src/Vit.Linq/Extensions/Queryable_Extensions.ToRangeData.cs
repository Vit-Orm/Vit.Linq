using System.Collections.Generic;
using System.Linq;

using Vit.Linq.ComponentModel;
using Vit.Linq.Filter.ComponentModel;


namespace Vit.Linq
{

    public static partial class Queryable_Extensions
    {
        public static RangeData<T> ToRangeData<T>(this IQueryable<T> query, RangeInfo range)
        {
            if (query == null) return null;
            return new RangeData<T>(range) { totalCount = query.Count(), items = query.Range(range).ToList() };
        }

        public static RangeData<T> ToRangeData<T>(this IQueryable<T> query, IEnumerable<OrderField> orders, RangeInfo range)
        {
            return ToRangeData(query?.OrderBy(orders), range);
        }

        public static RangeData<T> ToRangeData<T>(this IQueryable<T> query, FilterRule filter, IEnumerable<OrderField> orders, RangeInfo range)
        {
            return ToRangeData(query?.Where(filter)?.OrderBy(orders), range);
        }


        public static RangeData<T> ToRangeData<T>(this IQueryable<T> query, FilterRule filter, RangeInfo range)
        {
            return ToRangeData(query?.Where(filter), range);
        }

    }
}