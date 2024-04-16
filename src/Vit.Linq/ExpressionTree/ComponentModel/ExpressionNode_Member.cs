namespace Vit.Linq.ExpressionTree.ComponentModel
{

    public interface ExpressionNode_Member : IExpressionNode
    {
        public string memberName { get; set; }
        public string parameterName { get; set; }
        public ExpressionNode objectValue { get; set; }
    }


    public partial class ExpressionNode : ExpressionNode_Member
    {
        public string memberName { get; set; }
        public string parameterName { get; set; }
        public ExpressionNode objectValue { get; set; }

        public static ExpressionNode Member(string memberName = null, string parameterName = null, ExpressionNode objectValue = null)
            => new ExpressionNode { nodeType = NodeType.Member, memberName = memberName, parameterName = parameterName, objectValue = objectValue };
    }
}
