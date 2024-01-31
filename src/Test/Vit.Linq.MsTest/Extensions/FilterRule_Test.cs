using Vit.Extensions.Linq_Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Vit.Core.Module.Serialization;
using Vit.Linq.Filter;

namespace Vit.Linq.MsTest.Extensions
{
    [TestClass]
    public class FilterRule_Test
    {


        [TestMethod]
        public void Test_ToList()
        {
            {
                var query = DataSource.GetQueryable();

                var strRule = @"{'condition':'and', 'rules':[  
                                        {'field':'id',  'operator': '>',  'value':10  },
                                        {'field':'id',  'operator': '<',  'value': '20' } 
                                    ]}".Replace("'", "\"");
                var rule = Json.Deserialize<FilterRule>(strRule);
                var result = query.Where(rule).ToList();
                Assert.AreEqual(9, result.Count);
                Assert.AreEqual(11, result[0].id);
            }

            {
                var query = DataSource.GetQueryable();

                var rule = new FilterRule { field = "id", @operator = ">", value = 10 }.And(new FilterRule { field = "id", @operator = "<", value = 20 });
                var result = query.Where(rule).ToList();
                Assert.AreEqual(9, result.Count);
                Assert.AreEqual(11, result[0].id);
            }

            {
                var query = DataSource.GetQueryable();

                var rule = new FilterRule { field = "id", @operator = ">", value = 10 }.Not();
                var result = query.Where(rule).ToList();
                Assert.AreEqual(11, result.Count);
            }
        }


    }
}
