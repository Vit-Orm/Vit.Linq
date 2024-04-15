using System;
using System.Collections.Generic;
using System.Text;

namespace Vit.Linq.ExpressionTree.ComponentModel
{
    public interface IExpressionNode
    {
        string nodeType { get; }
    }
    public partial class ExpressionNode : IExpressionNode
    {
        public string nodeType { get; set; }
    }
}
