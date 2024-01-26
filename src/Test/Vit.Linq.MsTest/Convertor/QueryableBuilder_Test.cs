using System.Linq.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vit.Core.Module.Serialization;
using Vit.Linq.Convertor;
using Vit.Linq.Filter;
using Vit.Extensions.Linq_Extensions;
using System.Data;
using System.Linq;
using System;

namespace Vit.Linq.MsTest.Convertor
{
    [TestClass]
    public class QueryableBuilder_Test
    {
        [TestMethod]
        public void Test()
        {
            {
                Func<Expression, IQueryable<ModelA>> QueryExecutor = (exp) =>
                {
                    var query = new ExpressionConvertor().ConvertToQueryParam(exp);
                    var strQuery = Json.Serialize(query);

                    var exp2 = new FilterService().ToExpression<ModelA>(query.filter);
                    var query2 = new ExpressionConvertor().ConvertToQueryParam(exp2);
                    var strQuery2 = Json.Serialize(query2);


                    var list = DataSource.GetQueryable().Where(query.filter);
                    list = list.OrderBy(query.orders);
                    if (query.skip.HasValue)
                        list = list.Skip(query.skip.Value);
                    if (query.take.HasValue)
                        list = list.Take(query.take.Value);
                    return list;
                };

                var query = QueryableBuilder.Build(QueryExecutor);

                query = query
                    .Where(m => m.id >= 10)
                    .Where(m => m.id < 20)
                    .Where(m => !m.name.Contains("8"))
                    .Where(m => !m.b1.name.Contains("6"))
                    .OrderBy(m => m.b1.name)
                    .OrderByDescending(m => m.id)
                    .ThenBy(m => m.b1.pid)
                    .ThenByDescending(m => m.name)
                    .Skip(1)
                    .Take(5);

                var list = query.ToList();

                Assert.AreEqual(5, list.Count);
                Assert.AreEqual(17, list[0].id);
                Assert.AreEqual(15, list[1].id);
            }


        }




    }
}
