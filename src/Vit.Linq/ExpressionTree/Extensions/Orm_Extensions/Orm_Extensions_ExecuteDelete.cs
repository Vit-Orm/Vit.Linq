using System;
using System.Linq;
using System.Linq.Expressions;

namespace Vit.Extensions.Linq_Extensions
{

    public static partial class Queryable_Extensions
    {
        public static int ExecuteDelete(this IQueryable source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return source.Provider.Execute<int>(
                Expression.Call(
                    null,
                    new Func<IQueryable, int>(ExecuteDelete).Method
                    , source.Expression));
        }
    }
}