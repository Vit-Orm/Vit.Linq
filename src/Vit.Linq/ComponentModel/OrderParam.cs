
namespace Vit.Linq.ComponentModel
{
    public class OrderParam
    {
        /// <summary>
        /// field name(can be cascaded). demo "parent.id"
        /// </summary>
        public string field;

        /// <summary>
        /// whether is order by ascending
        /// </summary>
        public bool asc;

        public OrderParam() { }
        public OrderParam(string field, bool asc)
        {
            this.field = field;
            this.asc = asc;
        }
    }
}
