using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Vit.Linq
{

    public static partial class Queryable_Extensions
    {
        public static Task<List<T>> ToListAsync<T>(this IQueryable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return source.Provider.Execute<Task<List<T>>>(
                Expression.Call(
                    null,
                    new Func<IQueryable<T>, Task<List<T>>>(ToListAsync<T>).Method
                    , source.Expression));
        }
    }
}