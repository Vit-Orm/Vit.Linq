using System.Collections.Generic;

using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq.ExpressionTree.CollectionQuery
{
    public partial class QueryAction
    {
        public QueryAction() { }


        public QueryAction(ExpressionNode node)
        {
            LoadFromNode(this, node);
        }


        public ExpressionNode filter { get; set; }

        public List<ExpressionNodeOrderField> orders { get; set; }

        public int? skip { get; set; }
        public int? take { get; set; }


        /// <summary>
        /// default is ToList, could be :  Count | First | FirstOrDefault | Last | LastOrDefault | TotalCount
        /// </summary>
        public string method { get; set; }


    }
}
