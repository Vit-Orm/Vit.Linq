namespace Vit.Linq.ExpressionTree.ComponentModel
{
    public static class NodeType
    {
        public const string Member = nameof(Member);
        public const string Constant = nameof(Constant);
        public const string Convert = nameof(Convert);

        public const string AndAlso = nameof(AndAlso);
        public const string OrElse = nameof(OrElse);
        public const string Not = nameof(Not);

        public const string Equal = nameof(Equal);
        public const string NotEqual = nameof(NotEqual);
        public const string LessThan = nameof(LessThan);
        public const string LessThanOrEqual = nameof(LessThanOrEqual);
        public const string GreaterThan = nameof(GreaterThan);
        public const string GreaterThanOrEqual = nameof(GreaterThanOrEqual);

        public const string Lambda = nameof(Lambda);

        public const string MethodCall = nameof(MethodCall);
        public const string ArrayIndex = nameof(ArrayIndex);

        public const string New = nameof(New);
    }

}
