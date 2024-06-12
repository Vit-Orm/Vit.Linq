using System;
using System.Collections.Generic;
using System.Text;

namespace Vit.Linq.ExpressionTree.ComponentModel
{
    public static class NodeType
    {
        public const string Member = "Member";
        public const string Constant = "Constant";
        public const string Convert = "Convert";

        public const string And = "And";
        public const string Or = "Or";
        public const string Not = "Not";

        public const string Equal = "Equal";
        public const string NotEqual = "NotEqual";
        public const string LessThan = "LessThan";
        public const string LessThanOrEqual = "LessThanOrEqual";
        public const string GreaterThan = "GreaterThan";
        public const string GreaterThanOrEqual = "GreaterThanOrEqual";

        public const string Lambda = "Lambda";

        public const string MethodCall = nameof(MethodCall);
        public const string ArrayIndex = "ArrayIndex";

        public const string New = nameof(New);

    }

}
