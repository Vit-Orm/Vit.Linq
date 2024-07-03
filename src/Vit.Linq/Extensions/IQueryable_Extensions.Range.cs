using System.Linq;

using Vit.Linq.ComponentModel;

namespace Vit.Linq
{

    public static partial class IQueryable_Extensions
    {
        public static IQueryable IQueryable_Range(this IQueryable query, RangeInfo range)
        {
            if (range == null) return query;

            return IQueryable_Range(query, range.skip, range.take);
        }



        public static IQueryable IQueryable_Range(this IQueryable query, int skip, int take)
        {
            return query?.IQueryable_Skip(skip).IQueryable_Take(take);
        }
    }
}