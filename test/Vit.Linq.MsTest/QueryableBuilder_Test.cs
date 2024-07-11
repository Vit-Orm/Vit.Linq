using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Vit.Core.Module.Serialization;
using Vit.Linq;
using Vit.Linq.Filter;

namespace Vit.Linq.MsTest
{
    [TestClass]
    public class QueryableBuilder_Test
    {
        [TestMethod]
        public void Test()
        {
            {
                Func<Expression, Type, object> QueryExecutor = (exp, type) =>
                {
                    var queryAction = new FilterRuleConvert().ConvertToQueryAction(exp);
                    var strQuery = Json.Serialize(queryAction);

                    var exp2 = new FilterService().ToExpression<Person>(queryAction.filter);
                    var query2 = new FilterRuleConvert().ConvertToQueryAction(exp2);
                    var strQuery2 = Json.Serialize(query2);


                    var query = DataSource.GetQueryable().Where(queryAction.filter);
                    query = query.OrderBy(queryAction.orders);

                    var rangedQuery = query;

                    if (queryAction.skip.HasValue)
                        rangedQuery = rangedQuery.Skip(queryAction.skip.Value);
                    if (queryAction.take.HasValue)
                        rangedQuery = rangedQuery.Take(queryAction.take.Value);

                    switch (queryAction.method)
                    {
                        case nameof(Queryable.First): return rangedQuery.First();
                        case nameof(Queryable.FirstOrDefault): return rangedQuery.FirstOrDefault();
                        case nameof(Queryable.Last): return rangedQuery.Last();
                        case nameof(Queryable.LastOrDefault): return rangedQuery.LastOrDefault();
                        case nameof(Queryable.Count): return rangedQuery.Count();
                        case nameof(Queryable_Extensions.TotalCount): return query.Count();
                        case nameof(Queryable_Extensions.ToListAndTotalCount): return (rangedQuery.ToList(), query.Count());
                        default:
                            // ToList
                            return rangedQuery;
                    }


                };

                var query = QueryableBuilder.Build<Person>(QueryExecutor);

                query = query
                    .Where(m => m.id >= 10)
                    .Where(m => m.id < 20)
                    .Where(m => !m.name.Contains("8"))
                    .Where(m => !m.job.name.Contains("6"))
                    .OrderBy(m => m.job.name)
                    .OrderByDescending(m => m.id)
                    .ThenBy(m => m.job.departmentId)
                    .ThenByDescending(m => m.name)
                    .Skip(1)
                    .Take(5);

                #region Count
                {
                    var count = query.Count();
                    Assert.AreEqual(5, count);
                }
                #endregion

                #region TotalCount
                {
                    var totalCount = query.TotalCount();
                    Assert.AreEqual(8, totalCount);
                }
                #endregion

                #region ToListAndTotalCount
                {
                    var (list, totalCount) = query.ToListAndTotalCount();

                    Assert.AreEqual(8, totalCount);

                    Assert.AreEqual(5, list.Count);
                    Assert.AreEqual(17, list[0].id);
                    Assert.AreEqual(15, list[1].id);
                }
                #endregion

                #region ToList
                {
                    var list = query.ToList();
                    Assert.AreEqual(5, list.Count);
                    Assert.AreEqual(17, list[0].id);
                    Assert.AreEqual(15, list[1].id);
                }
                #endregion

            }


        }




    }
}
