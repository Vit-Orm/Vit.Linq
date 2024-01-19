using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vit.Core.Module.Serialization;
using Vit.Linq.Filter;
using Vit.Extensions.Linq_Extensions;

namespace Vit.Linq.MsTest.QueryBuilder
{

    [TestClass]
    public class OperatorMap_Test
    {
        [TestMethod]
        public void Test_OperatorMap()
        {
            {
                var service = new FilterService();
                var query = DataSource.GetQueryable();
                service.AddOperatorMap("Equal", FilterRuleOperator.Equal);


                var strRule = "{'field':'isEven',  'operator': 'eQual',  'value':true }".Replace("'", "\"");
                var rule = Json.Deserialize<FilterRule>(strRule);
                var result = query.Where(rule, service).ToList();
                Assert.AreEqual(result.Count, 500);
                Assert.AreEqual(0, result[0].id);
            }
        }
    }
}
