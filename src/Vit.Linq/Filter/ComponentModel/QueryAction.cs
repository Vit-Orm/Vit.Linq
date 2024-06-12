using System;
using System.Collections.Generic;
using System.Text;
using Vit.Linq.ComponentModel;

namespace Vit.Linq.Filter.ComponentModel
{
    public class QueryAction
    {
        public IFilterRule filter { get; set; }

        public IEnumerable<OrderField> orders { get; set; }

        public int? skip { get; set; }
        public int? take { get; set; }


        /// <summary>
        /// default is ToList, could be :  Count | First | FirstOrDefault | Last | LastOrDefault | TotalCount
        /// </summary>
        public string method { get; set; }

    }
}
