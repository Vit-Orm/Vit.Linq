namespace Vit.Linq.ComponentModel
{
    public class PageInfo
    {
        /// <summary>
        /// the size of the page (number of records per page)
        /// </summary>
        public int pageSize { get; set; }

        /// <summary>
        /// the index of the page (starting from 1)
        /// </summary>
        public int pageIndex { get; set; }

        public PageInfo() { }


        public PageInfo(int pageSize, int pageIndex = 1)
        {
            this.pageSize = pageSize;
            this.pageIndex = pageIndex;
        }

        public RangeInfo ToRange()
        {
            return new RangeInfo { skip = pageSize * (pageIndex - 1), take = pageSize };
        }


        public static implicit operator RangeInfo(PageInfo page)
        {
            return page.ToRange();
        }
    }

}
