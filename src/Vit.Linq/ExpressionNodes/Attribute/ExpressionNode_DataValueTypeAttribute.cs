using System;

namespace Vit.Linq.ExpressionNodes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ExpressionNode_DataValueTypeAttribute : Attribute
    {
        public EDataValueType dataValueType { get; set; }
        public ExpressionNode_DataValueTypeAttribute(EDataValueType dataValueType)
        {
            this.dataValueType = dataValueType;
        }
    }

}
