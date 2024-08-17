using System;

namespace Vit.Linq.ExpressionNodes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DataValueTypeAttribute : Attribute
    {
        public EDataValueType dataValueType { get; set; }
        public DataValueTypeAttribute(EDataValueType dataValueType)
        {
            this.dataValueType = dataValueType;
        }
    }



}
