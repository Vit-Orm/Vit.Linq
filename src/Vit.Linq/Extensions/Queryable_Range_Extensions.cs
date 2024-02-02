using System.Linq;
using System.Runtime.CompilerServices;

using Vit.Linq.ComponentModel;

namespace Vit.Extensions.Linq_Extensions
{

    public static partial class Queryable_Range_Extensions
    {
        public static IQueryable<T> Range<T>(this IQueryable<T> query, RangeInfo range)
        {
            if (query == null || range == null) return query;

            return query.Range(range.skip, range.take);
        }



        public static IQueryable<T> Range<T>(this IQueryable<T> query, int skip, int take)
        {
            return query?.Skip(skip).Take(take);
        }
    }
}