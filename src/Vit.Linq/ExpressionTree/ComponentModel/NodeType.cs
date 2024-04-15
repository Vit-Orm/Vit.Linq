using System;
using System.Collections.Generic;
using System.Text;

namespace Vit.Linq.ExpressionTree.ComponentModel
{
    public class NodeType
    {
        public const string Member=nameof(Member);
        public const string Constant = nameof(Constant);
        public const string Convert = nameof(Convert);

        public const string And = nameof(And);
        public const string Or = nameof(Or);
        public const string Not = nameof(Not);


        public const string Equal = nameof(Equal);
        public const string NotEqual = nameof(NotEqual);
        public const string LessThan = nameof(LessThan);
        public const string LessThanOrEqual = nameof(LessThanOrEqual);
        public const string GreaterThan = nameof(GreaterThan);
        public const string GreaterThanOrEqual = nameof(GreaterThanOrEqual);


        public const string Lambda = nameof(Lambda);


        public const string Call = nameof(Call);
        public const string ArrayIndex = nameof(ArrayIndex);
    }
}
