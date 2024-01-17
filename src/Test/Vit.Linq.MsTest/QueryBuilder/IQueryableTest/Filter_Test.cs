using Vit.Extensions.Linq_Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Vit.Core.Module.Serialization;
using Vit.Linq.QueryBuilder;
using System;
using Newtonsoft.Json.Linq;
using Queryable = System.Linq.IQueryable;

namespace Vit.Linq.MsTest.QueryBuilder.IQueryableTest
{
    [TestClass]
    public class Filter_Test : Filter_TestBase<Queryable>
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

        public override Queryable ToQuery(IQueryable<ModelA> query) => query;

        public override List<ModelA> Filter(Queryable query, IFilterRule rule)
        {
            return query.IQueryable_Where(rule, GetService()).IQueryable_ToList<ModelA>();
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


