using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vit.Core.Module.Serialization;
using Vit.Linq.Filter;
using Vit.Extensions.Linq_Extensions;
using Newtonsoft.Json.Linq;
using Vit.Linq.Filter.ComponentModel;

namespace Vit.Linq.MsTest.Filter
{

    [TestClass]
    public class PrimitiveValue_Test
    {
        [TestMethod]
        public void Test_PrimitiveValue()
        {
            // getPrimitiveValue
            {
                var service = new FilterService();
                service.getPrimitiveValue = (object value) =>
                {
                    if (value is JValue jv)
                    {
                        return jv.Value;
                    }
                    if (value is JArray ja)
                    {
                        return ja.Select(token => (token as JValue)?.Value).ToList();
                    }
                    return value;
                };

                var query = DataSource.GetQueryable();

                var strRule = "{'field':'name',  'operator': 'in',  'value':['name3','name4',12.5,true] }".Replace("'", "\"");
                var rule = Json.Deserialize<FilterRule>(strRule);
                var result = query.Where(rule, service).ToList();
                Assert.AreEqual(2, result.Count);
                Assert.AreEqual("name3", result[0].name);
                Assert.AreEqual("name4", result[1].name);
            }

            // getRightValue
            {
                var service = new FilterService();
                service.getRightValue = (IFilterRule rule, Type valueType) =>
                {
                    var valueInRule = rule.value;

                    // to deal with null value
                    if (valueInRule is JValue jv) return (true, jv.Value);
                    if (valueInRule is string str && rule.@operator?.Contains("in", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        return (true, str.Split(',').ToList());
                    }
                    return (false, null);
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
