using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Vit.Core.Module.Serialization;
using Vit.Linq.FilterRules.ComponentModel;

namespace Vit.Linq.MsTest.Filter
{

    [TestClass]
    public class FilterService_Test
    {
        [TestMethod]
        public void Test()
        {
            // empty rules
            {
                var query = DataSource.GetQueryable();

                var strRule = @"{'condition':'and', 'rules':[]}".Replace("'", "\"");
                var rule = Json.Deserialize<FilterRule>(strRule);
                var result = query.Where(rule).ToList();

                Assert.AreEqual(1000, result.Count);
            }

            {
                var query = DataSource.GetQueryable();

                var strRule = @"{'condition':'and'}".Replace("'", "\"");
                var rule = Json.Deserialize<FilterRule>(strRule);
                var result = query.Where(rule).ToList();

                Assert.AreEqual(1000, result.Count);
            }



        }


        [TestMethod]
        public void Test_Example()
        {
            {
                var users = new[] { new { id = 1, name = "name1" }, new { id = 2, name = "name2" } };

                var strRule = "{'field':'id',  'operator': '=',  'value': 1 }".Replace("'", "\"");
                var rule = Json.Deserialize<FilterRule>(strRule);
                var result = users.AsQueryable().Where(rule).ToList();

                Assert.AreEqual(1, result[0].id);
            }
        }




    }




}
