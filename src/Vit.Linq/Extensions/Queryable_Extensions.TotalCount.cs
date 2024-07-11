using System;
using System.Linq;
using System.Linq.Expressions;

namespace Vit.Linq
{

    public static partial class Queryable_Extensions
    {
        /// <summary>
        /// Collection Total Count without Take and Skip
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static int TotalCount(this IQueryable source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return source.Provider.Execute<int>(
                Expression.Call(
                    null,
                    new Func<IQueryable, int>(TotalCount).Method
                    , source.Expression));
        }

        /// <summary>
        /// Collection Total Count without Take and Skip
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static int TotalCount<T>(this IQueryable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return source.Provider.Execute<int>(
                Expression.Call(
                    null,
                    new Func<IQueryable<T>, int>(TotalCount).Method
                    , source.Expression));
        }
    }
}