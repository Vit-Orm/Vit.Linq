using System.Linq.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vit.Core.Module.Serialization;
using Vit.Extensions.Linq_Extensions;
using System.Data;
using Vit.Linq.ExpressionTree;
using Vit.Linq.ExpressionTree.ComponentModel;
using System;
using Vit.Linq.ExpressionTree.ComponentModel.CollectionQuery;
using System.Linq;
using System.Collections.Generic;

namespace Vit.Linq.MsTest.Converter
{
    // rootExpression?
    [TestClass]
    public class Queryable_Test
    {
        [TestMethod]
        public void Test()
        {
            {
                Func<Expression,  Type, object> QueryExecutor = (expression,type) =>
                {
                    var convertService = ExpressionConvertService.Instance;

                    ExpressionNode node;
                    

                    #region ExpressinNode
                    {
                        // #1 Code to Data
                        // (query) => query.Where().OrderBy().Skip().Take().Select().ToList();
                        node = convertService.ConvertToData(expression);

                        // {"nodeType":"Lambda","parameterNames":["Param_1"],"body":{"nodeType":"Call","methodName":"Where","arguments":[{"parameterName":"Param_1","nodeType":"Member"},{"nodeType":"Lambda","parameterNames":["Param_0"],"body":{"left":{"nodeType":"Member","parameterName":"Param_0","memberName":"id"},"right":{"nodeType":"Constant","valueType":{"typeName":"Int32"},"value":10},"nodeType":"GreaterThanOrEqual"}}]}}
                        var strNode = Json.Serialize(node);

                        // #2 Data to Code
                        // Param_1 => Param_1.Where(Param_0 => (Param_0.id >= 10))
                        var lambdaExp = convertService.ToLambdaExpression(node, typeof(IQueryable<Person>));
                        //var exp3 = (Expression<Func<IQueryable<Person>, IQueryable<Person>>>)lambdaExp;
                        var predicate_ = lambdaExp.Compile();
                    }
                    #endregion

                    // As Filter
                    var queryAction = new QueryAction(node);
                    var strQuery = Json.Serialize(queryAction);
                    var predicate = convertService.ToPredicateExpression<Person>(queryAction.filter);
                    //var lambdaExp = (Expression<Func<Person, bool>>)convertService.ToLambdaExpression(queryAction.filter, typeof(Person));


                    var list = DataSource.GetQueryable().Where(predicate);


                    list = list.OrderBy(queryAction.orders);

                    var methodName = queryAction.method;

                    if (methodName == "TotalCount") return list.Count();

                    if (queryAction.skip.HasValue)
                        list = list.Skip(queryAction.skip.Value);
                    if (queryAction.take.HasValue)
                        list = list.Take(queryAction.take.Value);

                    switch (methodName)
                    {
                        case "First": return list.First();
                        case "FirstOrDefault": return list.FirstOrDefault();
                        case "Last": return list.Last();
                        case "LastOrDefault": return list.LastOrDefault();
                        case "Count": return list.Count();
                        case "ToList":
                        case "":
                        case null:
                            return list;
                    }

                    throw new NotSupportedException("Method not support:" + methodName);
                };

                var query = QueryableBuilder.Build<Person>(QueryExecutor);

                var persons = new List<Person> { new Person { id = 2 }, new Person { id = 3 } }.AsQueryable();
                var pids = persons.Select(m => m.id).ToList();


                var jobs = new List<Job> { new Job { departmentId = 1 }, new Job { departmentId = 2 } };

                query = query
                #region work
                        //.Where(m => m.job.name != "test")     // Member Access ： Cascade
                        //.Where(m => !pids.Contains(m.id))     // MethodCall ： Contains
                        .Where(m => (float)m.id >= 10.0)      // Convert
                        //.Where(m => new List<int?> { 10 }.Contains(m.departmentId))        //  Nullable
                        //.Where(m => new int?[] { 10 }.Contains(m.departmentId))            //  Array

                        .Where(Param_0 => Param_0.id >= 10)
                        //.Where(m => m.id < 20)
                        //.Where(m => !m.name.Contains("8"))
                        //.Where(m => !m.job.name.Contains("6"))
                #endregion

                        //.Where(m => persons.Any(p => p.id == m.id)) // MethodCall Any
                        //.Where(m => persons.Where(p=>p.id>0).Any(p => p.id == m.id)) // MethodCall Any

                        //.Where((m, index) => index > 10) //  MethodCall Where   not suppoerted in FilterAction

                        //.OrderBy(m => m.job.name)
                        //.OrderByDescending(m => m.id)
                        //.ThenBy(m => m.job.departmentId)
                        //.ThenByDescending(m => m.name)
                        .Skip(1)
                        .Take(5)
                        ;


                #region List
                {
                    var list = query.ToList();
                    //Assert.AreEqual(5, list.Count);
                    //Assert.AreEqual(17, list[0].id);
                    //Assert.AreEqual(15, list[1].id);
                }
                #endregion

                var count = query.Count();
                var totalCount = query.TotalCount();

                var First = query.First();
                var FirstOrDefault = query.FirstOrDefault();
                var Last = query.Last();
                var LastOrDefault = query.LastOrDefault();

            }


        }




    }
}
