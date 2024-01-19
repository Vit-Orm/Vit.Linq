using System.Text.Json;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Vit.Linq.Filter;
using Vit.Linq.SystemTextJson;

namespace Vit.Linq.MsTest.QueryBuilder.IQueryableTest
{
    [TestClass]
    public class Filter_Test_SystemTextJson : Filter_Test_FilterRule
    {
        [TestMethod]
        public void Test_FilterRule()
        {
            base.TestFilterRule();
        }

        public override IFilterRule GetRule(string filterRule)
        {
            return JsonSerializer.Deserialize<FilterRule_SystemTextJson>(filterRule);
        }

        public override FilterService GetService()
        {
            FilterService service = new FilterService();
            return service;
        }



    }
}
