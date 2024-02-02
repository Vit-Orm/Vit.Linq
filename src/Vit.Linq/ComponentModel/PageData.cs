using System.Collections.Generic;


namespace Vit.Linq.ComponentModel
{
    public class PageData : PageData<object>
    {
        public PageData() { }
        public PageData(int pageSize, int pageIndex = 1) : base(pageSize, pageIndex) { }

        public PageData(PageInfo pageInfo = null) : base(pageInfo) { }
    }

    public class PageData<T> : PageInfo
    {

        public PageData() { }

        public PageData(int pageSize, int pageIndex = 1) : base(pageSize, pageIndex) { }


        public PageData(PageInfo pageInfo) : base(pageInfo.pageSize, pageInfo.pageIndex) { }


        /// <summary>
        /// data items for the current page
        /// </summary>
        public List<T> items;


        /// <summary>
        /// total count of records
        /// </summary>
        public int totalCount;
    }
}
