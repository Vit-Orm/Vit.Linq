using System.Collections.Generic;
using System.Data;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Vit.Linq.FilterRules;
using Vit.Linq.FilterRules.ComponentModel;

using Queryable = System.Linq.IQueryable;

namespace Vit.Linq.MsTest.Filter.IQueryableTest
{
    [TestClass]
    public class Filter_Test_Newtonsoft : Filter_TestBase<Queryable>
    {

        [TestMethod]
        public void Test_FilterRule()
        {
            base.TestFilterRule();
        }

        public override FilterRule GetRule(string filterRule)
        {
            return JsonConvert.DeserializeObject<FilterRule>(filterRule);
        }

        public override Queryable ToQuery(IQueryable<Person> query) => query;

        public override List<Person> Filter(Queryable query, IFilterRule rule)
        {
            return query.IQueryable_Where(rule, GetService()).IQueryable_ToList<Person>();
        }

        public virtual FilterService GetService()
        {
            FilterService service = new FilterService();
            service.getPrimitiveValue = GetPrimitiveValue_Newtonsoft;
            return service;
        }



        static object GetPrimitiveValue_Newtonsoft(object value)
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
        }



    }
}
