using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vit.Core.Module.Serialization;
using Vit.Linq.Filter;
using Vit.Extensions.Linq_Extensions;
using Newtonsoft.Json.Linq;

namespace Vit.Linq.MsTest.QueryBuilder
{

    [TestClass]
    public class CustomeValue_Test
    {
        [TestMethod]
        public void Test_CustomeValue()
        {
            {
                var service = new FilterService();
                service.GetRuleValue = (object? value, IFilterRule rule, Type fieldType) =>
                {
                    // to deal with null value
                    if (value is JValue jv) value = jv.Value;
                    if (value is string str && rule.@operator?.Contains("in", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        return str.Split(',');
                    }
                    return value;
                };

                var query = DataSource.GetQueryable();

                var strRule = "{'field':'name',  'operator': 'in',  'value':'name3,name4' }".Replace("'", "\"");
                var rule = Json.Deserialize<FilterRule>(strRule);
                var result = query.Where(rule, service).ToList();
                Assert.AreEqual(2, result.Count);
                Assert.AreEqual("name3", result[0].name);
                Assert.AreEqual("name4", result[1].name);
            }
        }
    }
}
