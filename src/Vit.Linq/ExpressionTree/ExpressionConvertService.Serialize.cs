 using System;
using System.Collections.Generic;
using System.Text;

namespace Vit.Linq.ExpressionTree
{
    public partial class ExpressionConvertService
    {

        public Func<object, Type, Object> convertToType { get; set; }

        public object ConvertToType(Object value, Type valueType)
        {
            return convertToType.Invoke(value, valueType);
        }

    }
}
