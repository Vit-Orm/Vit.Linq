using System.Collections.Generic;


namespace Vit.Linq.ExpressionTree.ComponentModel.CollectionQuery
{
    public partial class QueryAction
    {
        public QueryAction() { }


        public QueryAction(ExpressionNode node)
        {
            LoadFromNode(this, node);
        }


        public ExpressionNode filter { get; set; }

        public List<OrderField> orders { get; set; }

        public int? skip { get; set; }
        public int? take { get; set; }


        /// <summary>
        /// default is ToList, could be :  Count | First | FirstOrDefault | Last | LastOrDefault | TotalCount
        /// </summary>
        public string method { get; set; }


    }
}
