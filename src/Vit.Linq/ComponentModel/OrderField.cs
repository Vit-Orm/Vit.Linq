
namespace Vit.Linq.ComponentModel
{
    public class OrderField
    {
        /// <summary>
        /// field name(can be cascaded). demo: "parent.id"
        /// </summary>
        public string field;

        /// <summary>
        ///  Gets or sets a value indicating whether the order is ascending (true) or descending (false).
        /// </summary>
        public bool asc;

        public OrderField() { }
        public OrderField(string field, bool asc = true)
        {
            this.field = field;
            this.asc = asc;
        }
    }
}
