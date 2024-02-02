using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
