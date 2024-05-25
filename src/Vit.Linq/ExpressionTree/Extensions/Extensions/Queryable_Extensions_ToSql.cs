using System;
using System.Linq;
using System.Linq.Expressions;

namespace Vit.Extensions.Linq_Extensions
{

    public static partial class Queryable_Extensions
    {
        public static string ToSql(this IQueryable source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return source.Provider.Execute<string>(
                Expression.Call(
                    null,
                    new Func<IQueryable, string>(ToSql).Method
                    , source.Expression));
        }
    }
}