using Microsoft.VisualStudio.TestTools.UnitTesting;

using Vit.Core.Module.Serialization;
using Vit.Linq.QueryBuilder;
using Vit.Linq.QueryBuilder.NewtonsoftJson;

namespace Vit.Linq.MsTest.QueryBuilder.IQueryableTest
{
    [TestClass]
    public class Filter_Test_Newtonsoft : Filter_Test_FilterRule
    {

        [TestMethod]
        public void Test_FilterRule()
        {
            base.TestFilterRule();
        }

        public override IFilterRule GetRule(string filterRule)
        {
            return Json.Deserialize<FilterRule_Newtonsoft>(filterRule);
        }

        public override QueryBuilderService GetService()
        {
            QueryBuilderService service = new QueryBuilderService();
            return service;
        }


    }
}
