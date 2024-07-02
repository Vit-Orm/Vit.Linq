using System.Collections.Generic;
using System.Linq;

using Vit.Linq.ComponentModel;
using Vit.Linq.Filter.ComponentModel;


namespace Vit.Linq
{

    public static partial class IQueryable_Extensions
    {

        public static RangeData<T> IQueryable_ToRangeData<T>(this IQueryable query, RangeInfo range)
        {
            if (query == null) return null;
            return new RangeData<T>(range) { totalCount = query.IQueryable_Count(), items = query.IQueryable_Range(range).IQueryable_ToList<T>() };
        }

        public static RangeData<T> IQueryable_ToRangeData<T>(this IQueryable query, IEnumerable<OrderField> orders, RangeInfo range)
        {
            return IQueryable_ToRangeData<T>(query?.IQueryable_OrderByMemberExpression(orders), range);
        }

        public static RangeData<T> IQueryable_ToRangeData<T>(this IQueryable query, FilterRule filter, IEnumerable<OrderField> orders, RangeInfo range)
        {
            return IQueryable_ToRangeData<T>(query?.IQueryable_Where(filter)?.IQueryable_OrderByMemberExpression(orders), range);
        }


        public static RangeData<T> IQueryable_ToRangeData<T>(this IQueryable query, FilterRule filter, RangeInfo range)
        {
            return IQueryable_ToRangeData<T>(query?.IQueryable_Where(filter), range);
        }

    }
}