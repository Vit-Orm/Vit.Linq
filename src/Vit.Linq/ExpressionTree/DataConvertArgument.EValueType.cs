using System;

namespace Vit.Linq.ExpressionTree
{
    public enum EValueType
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



    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ValueTypeAttribute : Attribute
    {
        public EValueType valueType { get; set; }
        public ValueTypeAttribute(EValueType valueType)
        {
            this.valueType = valueType;
        }
    }



}
