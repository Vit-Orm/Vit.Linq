namespace Vit.Linq.FilterRules.ComponentModel
{
    public class RuleOperator
    {
        public const string IsNull = nameof(IsNull);
        public const string IsNotNull = nameof(IsNotNull);


        public const string Equal = "=";
        public const string NotEqual = "!=";

        public const string LessThan = "<";
        public const string LessThanOrEqual = "<=";
        public const string GreaterThan = ">";
        public const string GreaterThanOrEqual = ">=";



        public const string In = nameof(In);
        public const string NotIn = nameof(NotIn);

        public const string Contains = nameof(Contains);
        public const string NotContain = nameof(NotContain);
        public const string StartsWith = nameof(StartsWith);
        public const string EndsWith = nameof(EndsWith);
        public const string IsNullOrEmpty = nameof(IsNullOrEmpty);
        public const string IsNotNullOrEmpty = nameof(IsNotNullOrEmpty);


    }
}
