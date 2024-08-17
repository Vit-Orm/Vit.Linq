using System.Collections.Generic;

using Vit.Linq.ComponentModel;

namespace Vit.Linq.FilterRules.ComponentModel
{
    public class QueryAction
    {
        public FilterRule filter { get; set; }
        public IEnumerable<OrderField> orders { get; set; }


        public int? skip { get; set; }
        public int? take { get; set; }


        /// <summary>
        /// default is ToList, could be :  Count | First | FirstOrDefault | Last | LastOrDefault | TotalCount
        /// </summary>
        public string method { get; set; }

    }
}
