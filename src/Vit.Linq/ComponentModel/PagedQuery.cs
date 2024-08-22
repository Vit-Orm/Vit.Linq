using System.Collections.Generic;

using Vit.Linq.FilterRules.ComponentModel;

namespace Vit.Linq.ComponentModel
{
    public class PagedQuery
    {
        public FilterRule filter { get; set; }
        public List<OrderField> orders { get; set; }
        public PageInfo page { get; set; }

        public RangedQuery ToRangedQuery() => new RangedQuery { filter = filter, orders = orders, range = page?.ToRange() };

        public static implicit operator RangedQuery(PagedQuery query) => query?.ToRangedQuery();

    }
    public class PagedQuery<T> : PagedQuery
    {
        public T extra { get; set; }
    }
}
