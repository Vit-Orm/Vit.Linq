using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vit.Linq.ComponentModel;
using Vit.Linq.Filter;
using Vit.Linq.NewtonsoftJson;

namespace Vit.Linq.MsTest.Filter.QueryableTest
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
        public override FilterService GetService()
        {
            FilterService service = new FilterService();
            return service;
        }



    }
}
