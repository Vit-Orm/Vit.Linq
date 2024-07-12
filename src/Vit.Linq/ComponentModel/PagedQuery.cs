using System.Collections.Generic;

using Vit.Linq.Filter.ComponentModel;

namespace Vit.Linq.ComponentModel
{
    public class PagedQuery
    {
        public FilterRule filter { get; set; }
        public IEnumerable<OrderField> orders { get; set; }
        public PageInfo page { get; set; }
    }
}
