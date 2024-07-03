using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vit.Core.Module.Serialization;
using Vit.Linq.Filter;
using Vit.Linq;
using Vit.Linq.Filter.ComponentModel;

namespace Vit.Linq.MsTest.Filter
{

    [TestClass]
    public class FilterService_OperatorMap_Test
    {
        [TestMethod]
        public void Test_OperatorMap()
        {
            {
                var service = new FilterService();
                var query = DataSource.GetQueryable();
                service.AddOperatorMap("Equal", RuleOperator.Equal);


                var strRule = "{'field':'isEven',  'operator': 'eQual',  'value':true }".Replace("'", "\"");
                var rule = Json.Deserialize<FilterRule>(strRule);
                var result = query.Where(rule, service).ToList();
                Assert.AreEqual(result.Count, 500);
                Assert.AreEqual(0, result[0].id);
            }
        }
    }
}
