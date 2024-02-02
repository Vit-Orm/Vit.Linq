using System;
using System.Collections.Generic;
using System.Text;

namespace Vit.Linq.ComponentModel
{
    public class RangeData : RangeData<object>
    {
        public RangeData() { }
        public RangeData(int skip, int take) : base(skip, take) { }

        public RangeData(RangeInfo rangeInfo) : base(rangeInfo) { }
    }



    public class RangeData<T> : RangeInfo
    {

        public RangeData() { }

        public RangeData(int skip, int take) : base(skip, take) { }


        public RangeData(RangeInfo rangeInfo) : base(rangeInfo.skip, rangeInfo.take) { }

        /// <summary>
        /// Gets or sets the data items within the specified range.
        /// </summary>
        public List<T> items { get; set; }

        /// <summary>
        /// Gets or sets the total count of records.
        /// </summary>
        public int totalCount { get; set; }
    }
}
