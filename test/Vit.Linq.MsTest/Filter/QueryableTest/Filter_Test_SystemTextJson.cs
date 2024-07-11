using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Vit.Linq.Filter;
using Vit.Linq.Filter.ComponentModel;

using Queryable = System.Linq.IQueryable<Vit.Linq.MsTest.Person>;

namespace Vit.Linq.MsTest.Filter.QueryableTest
{
    [TestClass]
    public class Filter_Test_SystemTextJson : Filter_TestBase<Queryable>
    {

        [TestMethod]
        public void Test_FilterRule()
        {
            base.TestFilterRule();
        }

        public override FilterRule GetRule(string filterRule)
        {
            return JsonSerializer.Deserialize<FilterRule>(filterRule);
        }

        public override Queryable ToQuery(IQueryable<Person> query) => query;

        public override List<Person> Filter(Queryable query, IFilterRule rule)
        {
            return query.Where(rule, GetService()).ToList<Person>();
        }

        public virtual FilterService GetService()
        {
            FilterService service = new FilterService();
            service.getPrimitiveValue = GetPrimitiveValue_Text;
            return service;
        }



        static object GetPrimitiveValue_Text(object value)
        {
            if (value is JsonElement elem)
            {
                switch (elem.ValueKind)
                {
                    case JsonValueKind.Null:
                    case JsonValueKind.Undefined: return null;
                    case JsonValueKind.True: return true;
                    case JsonValueKind.False: return false;
                    case JsonValueKind.Number: return elem.GetDecimal();
                    case JsonValueKind.String: return elem.GetString();
                    case JsonValueKind.Array:
                        return elem.EnumerateArray().Select(item => GetPrimitiveValue_Text(item)).ToList();
                }
            }
            return value;
        }



    }
}
