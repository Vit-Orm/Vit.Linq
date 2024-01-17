using Microsoft.VisualStudio.TestTools.UnitTesting;

using Vit.Linq.QueryBuilder;
using Vit.Linq.QueryBuilder.SystemTextJson;

namespace Vit.Linq.MsTest.QueryBuilder.IQueryableTest
{
    [TestClass]
    public class Filter_Test_SystemTextJson2 : Filter_Test
    {

        public override IFilterRule GetRule(string filterRule)
        {
            return FilterRule_SystemTextJson.FromString(filterRule);
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
