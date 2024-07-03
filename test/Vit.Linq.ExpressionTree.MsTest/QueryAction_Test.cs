using System.Linq.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vit.Core.Module.Serialization;
using Vit.Linq;
using System.Data;
using Vit.Linq.ExpressionTree.ComponentModel;
using System;
using System.Linq;
using System.Collections.Generic;
using Vit.Linq.ExpressionTree.CollectionQuery;

namespace Vit.Linq.ExpressionTree.MsTest
{
    [TestClass]
    public class QueryAction_Test
    {


        IQueryable<Person> GetQuery()
        {
            var convertService = ExpressionConvertService.Instance;
            var queryTypeName = "TestQuery";
            var sourceData = DataSource.GetQueryable();

            Func<Expression, Type, object> QueryExecutor = (expression, type) =>
            {
                ExpressionNode node;

                // #1 Code to Data
                // query => query.Where().OrderBy().Skip().Take().Select().ToList();
                var isArgument = QueryableBuilder.CompareQueryByName(queryTypeName);
                node = convertService.ConvertToData(expression, autoReduce: true, isArgument: isArgument);
                var strNode = Json.Serialize(node);

                // #2 Filter by QueryAction
                var queryAction = new QueryAction(node);
                var strQuery = Json.Serialize(queryAction);
                var predicate = convertService.ToPredicateExpression<Person>(queryAction.filter);
                //var lambdaExp = (Expression<Func<Person, bool>>)convertService.ToLambdaExpression(queryAction.filter, typeof(Person));

                var query = sourceData.Where(predicate);

                query = query.OrderBy(queryAction.orders);

                var methodName = queryAction.method;

                if (methodName == "TotalCount") return query.Count();

                if (queryAction.skip.HasValue)
                    query = query.Skip(queryAction.skip.Value);
                if (queryAction.take.HasValue)
                    query = query.Take(queryAction.take.Value);

                switch (methodName)
                {
                    case "First": return query.First();
                    case "FirstOrDefault": return query.FirstOrDefault();
                    case "Last": return query.Last();
                    case "LastOrDefault": return query.LastOrDefault();
                    case "Count": return query.Count();
                    case "ToList":
                    case "":
                    case null:
                        return query;
                }

                throw new NotSupportedException("Method not support:" + methodName);
            };

            return QueryableBuilder.Build<Person>(QueryExecutor, queryTypeName);
        }


        [TestMethod]
        public void Test_Query()
        {
            var query = GetQuery();

            query = query
                .Where(m => !m.name.Contains("111"))
                .OrderBy(m => m.job.name)
                .OrderByDescending(m => m.id)
                .ThenBy(m => m.job.departmentId)
                .ThenByDescending(m => m.name)
                .Skip(1)
                .Take(5);

            #region ToList
            {
                var list = query.ToList();
                Assert.AreEqual(5, list.Count);
                Assert.AreEqual(998, list.First().id);
                Assert.AreEqual(994, list.Last().id);
            }
            #endregion

            #region Count
            {
                var count = query.Count();
                Assert.AreEqual(5, count);
            }
            #endregion

            #region First FirstOrDefault
            {
                var person = query.First();
                Assert.AreEqual(998, person.id);
            }
            {
                var person = query.FirstOrDefault();
                Assert.AreEqual(998, person.id);
            }
            #endregion

            #region First LastOrDefault
            {
                var person = query.Last();
                Assert.AreEqual(994, person.id);
            }
            {
                var person = query.LastOrDefault();
                Assert.AreEqual(994, person.id);
            }
            #endregion
        }


        [TestMethod]
        public void Test_Where()
        {
            #region Convert
            {
                var query = GetQuery();

                query = query
                    .Where(m => m.id <= 10.0)      // Convert
                    ;

                var list = query.ToList();
                Assert.AreEqual(11, list.Count);
                Assert.AreEqual(0, list.First().id);
                Assert.AreEqual(10, list.Last().id);
            }
            #endregion

            #region Member Access :  Cascade
            {
                var query = GetQuery();

                query = query
                    .Where(m => m.job.name == "name10_job1")     // Member Access :  Cascade
                    ;

                var list = query.ToList();
                Assert.AreEqual(1, list.Count);
                Assert.AreEqual(10, list.First().id);
            }
            #endregion


            #region MethodCall :  String.Contains
            {
                var query = GetQuery();

                query = query
                  .Where(m => m.job.name.Contains("me10_job")) // String.Contains
                    ;

                var list = query.ToList();
                Assert.AreEqual(1, list.Count);
                Assert.AreEqual(10, list.First().id);
            }
            #endregion

            #region MethodCall :  List.Contains
            {
                var query = GetQuery();
                var ids = new List<int> { 3, 4 };
                query = query
                      .Where(m => ids.Contains(m.id))     // MethodCall :  List.Contains
                      .Where(m => new List<int> { 3, 4 }.Contains(m.id))     // MethodCall :  List.Contains
                    ;

                var list = query.ToList();
                Assert.AreEqual(2, list.Count);
                Assert.AreEqual(3, list.First().id);
                Assert.AreEqual(4, list.Last().id);
            }
            #endregion

            #region MethodCall :  Array.Contains
            {
                var query = GetQuery();
                var ids = new int[] { 3, 4 };
                query = query
                    .Where(m => ids.Contains(m.id))     // MethodCall :  Array.Contains
                    .Where(m => new int[] { 3, 4 }.Contains(m.id))     // MethodCall :  Array.Contains
                    ;

                var list = query.ToList();
                Assert.AreEqual(2, list.Count);
                Assert.AreEqual(3, list.First().id);
                Assert.AreEqual(4, list.Last().id);
            }
            #endregion




            #region Nullable
            {
                var query = GetQuery();

                query = query
                  .Where(m => new List<int?> { 3, 4 }.Contains(m.id)) // Nullable
                    ;

                var list = query.ToList();
                Assert.AreEqual(2, list.Count);
                Assert.AreEqual(3, list.First().id);
                Assert.AreEqual(4, list.Last().id);
            }
            #endregion

            #region Where
            {
                var query = GetQuery();

                query = query
                    .Where(Param_0 => Param_0.id >= 10)
                    .Where(m => m.id < 20)
                    ;

                var list = query.ToList();
                Assert.AreEqual(10, list.Count);
                Assert.AreEqual(10, list.First().id);
                Assert.AreEqual(19, list.Last().id);
            }
            #endregion
        }


        [TestMethod]
        public void Test_MethodCall_Enumerable_Contains()
        {
            #region Enumerable.Contains
            {
                var query = GetQuery();

                var persons = new List<Person> { new Person { id = 2 }, new Person { id = 3 } };
                var ids = persons.Where(p => p.id > 0).Select(p => p.id);

                query = query
                    .Where(m => ids.Contains(m.id)) // MethodCall Enumerable.Contains
                    ;

                var list = query.ToList();
                Assert.AreEqual(2, list.Count);
                Assert.AreEqual(2, list.First().id);
                Assert.AreEqual(3, list.Last().id);
            }
            #endregion

        }



        [TestMethod]
        public void Test_MethodCall_Enumerable_ToArray()
        {
            #region Enumerable.ToArray
            {
                var query = GetQuery().Where(m => m.id >= 2 && m.id < 4);

                var list = query.ToArray();
                Assert.AreEqual(2, list.Length);
                Assert.AreEqual(2, list.First().id);
                Assert.AreEqual(3, list.Last().id);
            }
            #endregion

        }




    }
}
