using System;
using System.Collections.Generic;
using System.Text;

namespace Vit.Linq.ComponentModel
{
    public class QueryParam
    {
        public IFilterRule filter { get; set; }

        public List<OrderParam> orders { get; set; }

        public int? skip { get; set; }
        public int? take { get; set; }
    }
}
