using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text;

namespace Vit.Extensions.Linq_Extensions
{
    public static partial class IQueryable_TotalCount_Extensions
    {

        #region Count 
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
        #endregion
    }
}
