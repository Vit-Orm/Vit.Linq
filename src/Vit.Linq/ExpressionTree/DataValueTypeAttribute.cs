using System;

namespace Vit.Linq.ExpressionTree
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
