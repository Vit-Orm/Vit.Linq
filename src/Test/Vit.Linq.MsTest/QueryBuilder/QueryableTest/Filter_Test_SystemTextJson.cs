using System.Text.Json;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Vit.Linq.QueryBuilder;
using Vit.Linq.QueryBuilder.SystemTextJson;

namespace Vit.Linq.MsTest.QueryBuilder.QueryableTest
{
    [TestClass]
    public class Filter_Test_SystemTextJson : Filter_Test
    {

        public override IFilterRule GetRule(string filterRule)
        {
            return JsonSerializer.Deserialize<FilterRule_SystemTextJson>(filterRule);
        }

        public override QueryBuilderService GetService()
        {
            QueryBuilderService service = new QueryBuilderService();
            return service;
        }


        [TestMethod]
        public void Test_FilterRule()
        {
            base.TestFilterRule();
        }
    }
}
