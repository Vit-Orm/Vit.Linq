using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Vit.Linq
{

    public static partial class Queryable_Extensions
    {
        public static (List<T> list, int totalCount) ToListAndTotalCount<T>(this IQueryable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return source.Provider.Execute<(List<T> list, int totalCount)>(
                Expression.Call(
                    null,
                    new Func<IQueryable<T>, (List<T> list, int totalCount)>(ToListAndTotalCount<T>).Method
                    , source.Expression));
        }
    }
}