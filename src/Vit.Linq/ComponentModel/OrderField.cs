
namespace Vit.Linq.ComponentModel
{
    public class OrderField
    {
        /// <summary>
        /// field name(can be cascaded). demo "parent.id"
        /// </summary>
        public string field;

        /// <summary>
        /// whether is order by ascending
        /// </summary>
        public bool asc;

        public OrderField() { }
        public OrderField(string field, bool asc)
        {
            this.field = field;
            this.asc = asc;
        }
    }
}
