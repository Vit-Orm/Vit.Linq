using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Vit.Core.Module.Serialization;
using Vit.Linq;
using Vit.Linq.ExpressionTree.ComponentModel;
using Vit.Linq.ExpressionTree.Query;

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
                ExpressionNode_Lambda node;

                // #1 Code to Data
                // query => query.Where().OrderBy().Skip().Take().Select().ToList();
                var isArgument = QueryableBuilder.CompareQueryByName(queryTypeName);
                node = convertService.ConvertToData_LambdaNode(expression, autoReduce: true, isArgument: isArgument);
                var strNode = Json.Serialize(node);

                // #2 Filter by CollectionQuery
                var collectionQuery = new CollectionQuery(node);
                var strQuery = Json.Serialize(collectionQuery);
                var predicate = convertService.ConvertToCode_PredicateExpression<Person>(collectionQuery.filter);
                //var lambdaExp = (Expression<Func<Person, bool>>)convertService.ToLambdaExpression(queryAction.filter, typeof(Person));

                var query = sourceData.Where(predicate);

                query = query.OrderBy(collectionQuery.orders);

                var rangedQuery = query;

                if (collectionQuery.skip.HasValue)
                    rangedQuery = rangedQuery.Skip(collectionQuery.skip.Value);
                if (collectionQuery.take.HasValue)
                    rangedQuery = rangedQuery.Take(collectionQuery.take.Value);

                switch (collectionQuery.method)
                {
                    case nameof(Queryable.First): return rangedQuery.First();
                    case nameof(Queryable.FirstOrDefault): return rangedQuery.FirstOrDefault();
                    case nameof(Queryable.Last): return rangedQuery.Last();
                    case nameof(Queryable.LastOrDefault): return rangedQuery.LastOrDefault();
                    case nameof(Queryable.Count): return rangedQuery.Count();
                    case nameof(Queryable_Extensions.TotalCount): return query.Count();
                    case nameof(Queryable_Extensions.ToListAndTotalCount): return (rangedQuery.ToList(), query.Count());
                    case "ToList":
                    case "":
                    case null:
                        //default: 
                        return rangedQuery;
                }

                throw new NotSupportedException("Method not support:" + collectionQuery.method);
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

            #region Count
            {
                var count = query.Count();
                Assert.AreEqual(5, count);
            }
            #endregion

            #region TotalCount
            {
                var totalCount = query.TotalCount();
                Assert.AreEqual(999, totalCount);
            }
            #endregion

            #region ToListAndTotalCount
            {
                var (list, totalCount) = query.ToListAndTotalCount();

                Assert.AreEqual(999, totalCount);

                Assert.AreEqual(5, list.Count);
                Assert.AreEqual(998, list.First().id);
                Assert.AreEqual(994, list.Last().id);
            }
            #endregion


            #region ToList
            {
                var list = query.ToList();
                Assert.AreEqual(5, list.Count);
                Assert.AreEqual(998, list.First().id);
                Assert.AreEqual(994, list.Last().id);
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
