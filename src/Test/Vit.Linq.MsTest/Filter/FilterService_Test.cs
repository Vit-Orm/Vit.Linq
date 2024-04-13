using System.Data;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Vit.Core.Module.Serialization;
using Vit.Extensions.Linq_Extensions;
using Vit.Linq.Filter;

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
    }




}
