using System.Linq;

using Vit.Linq.ComponentModel;

namespace Vit.Linq
{

    public static partial class IQueryable_Extensions
    {


        public static IQueryable IQueryable_Page(this IQueryable query, PageInfo page)
        {
            return query.IQueryable_Range(page.ToRange());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"> the index of the page (starting from 1) </param>
        /// <returns></returns>
        public static IQueryable IQueryable_Page(this IQueryable query, int pageSize, int pageIndex = 1)
        {
            return query.IQueryable_Page(new PageInfo(pageSize: pageSize, pageIndex: pageIndex));
        }
    }
}