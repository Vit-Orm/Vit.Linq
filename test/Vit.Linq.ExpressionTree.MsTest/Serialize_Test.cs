using System;
using System.Linq.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq.ExpressionTree.MsTest
{
    [TestClass]
    public class Serialize_Test
    {
        [TestMethod]
        public void Test()
        {
            try
            {
                var Json = Sers.Core.Module.Serialization.Text.Serialization_Text.Instance;

                var service = ExpressionConvertService.Instance;

                Expression<Func<Person, bool>> predicate = person => person.id > 0;

                Expression expression = predicate;
                ExpressionNode_Lambda node = service.ConvertToData_LambdaNode(expression);

                var str = Json.Serialize(node);

                var node2 = Json.Deserialize<ExpressionNode>(str);
                var str2 = Json.Serialize(node2);

            }
            catch (Exception ex)
            {
                throw;
            }

            try
            {
                var Json = Vit.Core.Module.Serialization.Serialization_Newtonsoft.Instance;

                var service = ExpressionConvertService.Instance;

                Expression<Func<Person, bool>> predicate = person => person.id > 0;

                Expression expression = predicate;
                ExpressionNode_Lambda node = service.ConvertToData_LambdaNode(expression);

                var str = Json.Serialize(node);

                var node2 = Json.Deserialize<ExpressionNode>(str);
                var str2 = Json.Serialize(node2);

            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }
}
