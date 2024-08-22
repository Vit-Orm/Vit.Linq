namespace Vit.Linq.ExpressionNodes
{
    public enum EDataValueType
    {
        /// <summary>
        /// constant value like 1.5 or int[]{1,2,3}
        /// </summary>
        constant,
        /// <summary>
        /// QueryableArgument
        /// </summary>
        argument,
        /// <summary>
        /// like parameter from lambda argument  or combined value
        /// </summary>
        other,
    }



}
