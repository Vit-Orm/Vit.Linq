using Microsoft.VisualStudio.TestTools.UnitTesting;

using Vit.Linq.QueryBuilder;
using Vit.Linq.QueryBuilder.NewtonsoftJson;

namespace Vit.Linq.MsTest.QueryBuilder.QueryableTest
{
    [TestClass]
    public class Filter_Test_Newtonsoft2 : Filter_Test_FilterRule
    {
        [TestMethod]
        public void Test_FilterRule()
        {
            base.TestFilterRule();
        }

        public override IFilterRule GetRule(string filterRule)
        {
            return FilterRule_Newtonsoft.FromString(filterRule);
        }
        public override QueryBuilderService GetService()
        {
            QueryBuilderService service = new QueryBuilderService();
            return service;
        }



    }
}
