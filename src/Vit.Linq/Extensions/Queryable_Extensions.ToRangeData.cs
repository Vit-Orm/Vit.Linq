using System.Collections.Generic;
using System.Linq;

using Vit.Linq.ComponentModel;
using Vit.Linq.FilterRules;
using Vit.Linq.FilterRules.ComponentModel;


namespace Vit.Linq
{

    public static partial class Queryable_Extensions
    {
        public static RangeData<T> ToRangeData<T>(this IQueryable<T> query, RangeInfo range, FilterService filterService = null)
        {
            if (query == null) return null;
            return new RangeData<T>(range) { totalCount = query.Count(), items = query.Range(range).ToList() };
        }

        public static RangeData<T> ToRangeData<T>(this IQueryable<T> query, IEnumerable<OrderField> orders, RangeInfo range)
        {
            return ToRangeData(query?.OrderBy(orders), range);
        }

        public static RangeData<T> ToRangeData<T>(this IQueryable<T> query, FilterRule filter, IEnumerable<OrderField> orders, RangeInfo range, FilterService filterService = null)
        {
            return ToRangeData(query?.Where(filter, filterService)?.OrderBy(orders), range);
        }


        public static RangeData<T> ToRangeData<T>(this IQueryable<T> query, FilterRule filter, RangeInfo range, FilterService filterService = null)
        {
            return ToRangeData(query?.Where(filter, filterService), range);
        }

        public static RangeData<T> ToRangeData<T>(this IQueryable<T> query, RangedQuery rangedQuery, FilterService filterService = null)
        {
            return ToRangeData(query, rangedQuery?.filter, rangedQuery?.orders, rangedQuery?.range, filterService);
        }

    }
}