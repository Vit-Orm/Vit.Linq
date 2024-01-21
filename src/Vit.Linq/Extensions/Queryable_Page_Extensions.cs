using System.Linq;
using System.Runtime.CompilerServices;

using Vit.Linq.ComponentModel;

namespace Vit.Extensions.Linq_Extensions
{

    public static partial class Queryable_Page_Extensions
    {


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IQueryable<T> Page<T>(this IQueryable<T> query, PageInfo page)
            where T : class
        {
            if (query == null || page == null) return query;

            return query.Page(page.pageIndex, page.pageSize);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="pageIndex">start from 1</param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IQueryable<T> Page<T>(this IQueryable<T> query, int pageIndex, int pageSize)
          where T : class
        {
            return query.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }
    }
}