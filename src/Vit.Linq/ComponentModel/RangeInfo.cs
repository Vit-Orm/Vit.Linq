namespace Vit.Linq.ComponentModel
{
    public class RangeInfo
    {

        /// <summary>
        /// the number of records to skip
        /// </summary>
        public int skip;

        /// <summary>
        /// the number of records to take
        /// </summary>
        public int take;

        public RangeInfo() { }

        public RangeInfo(int skip, int take = 10)
        {
            this.skip = skip;
            this.take = take;
        }

        public static RangeInfo FromPage(int pageSize, int pageIndex = 1)
        {
            return new RangeInfo { skip = pageSize * (pageIndex - 1), take = pageSize };
        }

        public static RangeInfo FromPage(PageInfo page)
        {
            return FromPage(page.pageSize, page.pageIndex);
        }
    }
}
