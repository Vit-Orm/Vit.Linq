using Vit.Extensions.Linq_Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Vit.Core.Module.Serialization;
using Vit.Linq.Filter;
using System;
using Newtonsoft.Json.Linq;
using Queryable = System.Linq.IQueryable<Vit.Linq.MsTest.Person>;
using Vit.Linq.ComponentModel;

namespace Vit.Linq.MsTest.Filter.QueryableTest
{
    [TestClass]
    public class Filter_Test_FilterRule : Filter_TestBase<Queryable>
    {

        [TestMethod]
        public void Test_FilterRule_()
        {
            base.TestFilterRule();
        }


        public override IFilterRule GetRule(string filterRule)
        {
            return Json.Deserialize<FilterRule>(filterRule);
        }

        public override Queryable ToQuery(IQueryable<Person> query) => query;

        public override List<Person> Filter(Queryable query, IFilterRule rule)
        {
            return query.Where(rule, GetService()).ToList<Person>();
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


