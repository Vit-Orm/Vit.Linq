using Microsoft.VisualStudio.TestTools.UnitTesting;

using Vit.Core.Module.Serialization;
using Vit.Linq.Filter;
using Vit.Linq.NewtonsoftJson;

namespace Vit.Linq.MsTest.Filter.IQueryableTest
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

        public override FilterService GetService()
        {
            FilterService service = new FilterService();
            return service;
        }


    }
}
