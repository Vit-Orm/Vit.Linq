using System.Collections.Generic;


namespace Vit.Linq.ComponentModel
{
    public class PageData : PageData<object>
    {
        public PageData(PageInfo pageInfo = null) : base(pageInfo)
        {
        }
    }

    public class PageData<T> : PageInfo
    {

        public PageData(PageInfo pageInfo = null)
        {
            if (null != pageInfo)
            {
                pageSize = pageInfo.pageSize;
                pageIndex = pageInfo.pageIndex;
            }
        }

        /// <summary>
        /// rows
        /// </summary>
        public List<T> rows;


        /// <summary>
        /// totalCount
        /// </summary>
        public int totalCount;
    }
}
