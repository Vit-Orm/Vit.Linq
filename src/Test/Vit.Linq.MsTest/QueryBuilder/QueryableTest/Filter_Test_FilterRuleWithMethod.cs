using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vit.Core.Module.Serialization;
using Vit.Linq.QueryBuilder;
using Vit.Linq.MoreFilter;
using Newtonsoft.Json.Linq;
using System;

namespace Vit.Linq.MsTest.QueryBuilder.QueryableTest
{
    [TestClass]
    public class Filter_Test_FilterRuleWithMethod : Filter_Test_FilterRule
    {
        [TestMethod]
        public void Test_FilterRule()
        {
            base.TestFilterRule();
        }

        public override IFilterRule GetRule(string filterRule)
        {
            return Json.Deserialize<FilterRuleWithMethod>(filterRule);
        }

        public virtual QueryBuilderService GetService()
        {
            QueryBuilderService service = new QueryBuilderService();
            service.GetRuleValue = (object value, IFilterRule rule, Type fieldType) =>
            {
                // to deal with null value
                if (value is JValue jv) return jv.Value;
                return value;
            };
            return service;
        }

    }
}


