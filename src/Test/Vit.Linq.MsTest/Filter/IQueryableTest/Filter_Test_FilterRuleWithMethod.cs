using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vit.Core.Module.Serialization;
using Vit.Linq.Filter;
using Vit.Linq.MoreFilter;
using Newtonsoft.Json.Linq;
using System;
using Vit.Linq.ComponentModel;

namespace Vit.Linq.MsTest.Filter.IQueryableTest
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

        public virtual FilterService GetService()
        {
            FilterService service = new FilterService();
            service.GetPrimitiveValue = (object value, IFilterRule rule, Type fieldType) =>
            {
                // to deal with null value
                if (value is JValue jv) return jv.Value;
                return value;
            };
            return service;
        }

    }
}


