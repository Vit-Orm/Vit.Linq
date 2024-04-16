using System;
using System.Collections.Generic;
using System.Text;

namespace Vit.Linq.ExpressionTree
{
    public partial class ExpressionConvertService
    {
        public interface IMethodConvertor 
        {
            int priority { get; }

        }
    }
}
