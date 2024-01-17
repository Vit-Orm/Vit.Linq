using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Vit.Linq.QueryBuilder;

namespace Vit.Linq.MsTest.QueryBuilder
{

    public abstract class Filter_TestBase<Query>
    {
        IQueryable<ModelA> GetQueryable() => DataSource.GetQueryable();


        public abstract IFilterRule GetRule(string filterRule);
        public abstract Query ToQuery(IQueryable<ModelA> query);

        public abstract List<ModelA> Filter(Query query, IFilterRule rule);



        protected void TestFilterRule()
        {

            #region #1 [object] IsNull | IsNotNull

            #region ##1 IsNull
            {
                var query = GetQueryable();

                var item = query.Skip(10).FirstOrDefault();
                item.name = null;

                var strRule = "{'field':'name',  'operator': 'IsNull'  }".Replace("'", "\"");
                var rule = GetRule(strRule);
                var result = Filter(ToQuery(query), rule);
                Assert.AreEqual(1, result.Count);
                Assert.AreEqual(10, result[0].id);
            }

            {
                var query = GetQueryable();

                var item = query.Skip(10).FirstOrDefault();
                item.name = null;

                var strRule = "{'field':'name',  'operator': '=',  'value': null  }".Replace("'", "\"");
                var rule = GetRule(strRule);
                var result = Filter(ToQuery(query), rule);
                Assert.AreEqual(1, result.Count);
                Assert.AreEqual(10, result[0].id);
            }
            #endregion

            #region ##2 IsNotNull
            {
                var query = GetQueryable();

                var item = query.Skip(10).FirstOrDefault();
                item.name = null;

                var strRule = "{'field':'name',  'operator': 'IsNotNull'  }".Replace("'", "\"");
                var rule = GetRule(strRule);
                var result = Filter(ToQuery(query), rule);
                Assert.AreEqual(999, result.Count);
            }

            {
                var query = GetQueryable();

                var item = query.Skip(10).FirstOrDefault();
                item.name = null;

                var strRule = "{'field':'name',  'operator': '!=',  'value': null  }".Replace("'", "\"");
                var rule = GetRule(strRule);
                var result = Filter(ToQuery(query), rule);
                Assert.AreEqual(999, result.Count);
            }
            #endregion

            #endregion


            #region #2 [number | string | bool] compare

            #region ##1.1 [number] =
            {
                // ###1
                {
                    var query = GetQueryable();

                    var strRule = "{'field':'id',  'operator': '=',  'value':10 }".Replace("'", "\"");
                    var rule = GetRule(strRule);
                    var result = Filter(ToQuery(query), rule);
                    Assert.AreEqual(1, result.Count);
                    Assert.AreEqual(10, result[0].id);
                }

                // ###2
                {
                    var query = GetQueryable();

                    var strRule = "{'field':'id',  'operator': '=',  'value': '10' }".Replace("'", "\"");
                    var rule = GetRule(strRule);
                    var result = Filter(ToQuery(query), rule);
                    Assert.AreEqual(1, result.Count);
                    Assert.AreEqual(10, result[0].id);
                }


                // ###3  = null
                {
                    var query = GetQueryable();

                    var item = query.Skip(10).FirstOrDefault();
                    item.pid = null;

                    var strRule = "{'field':'pid',  'operator': '=',  'value': null }".Replace("'", "\"");
                    var rule = GetRule(strRule);
                    var result = Filter(ToQuery(query), rule);
                    Assert.AreEqual(10, result.FirstOrDefault().id);
                }


            }
            #endregion

            #region ##1.2 [bool] =
            {
                // ###1
                {
                    var query = GetQueryable();

                    var strRule = "{'field':'isEven',  'operator': '=',  'value':true }".Replace("'", "\"");
                    var rule = GetRule(strRule);
                    var result = Filter(ToQuery(query), rule);
                    Assert.AreEqual(result.Count, 500);
                    Assert.AreEqual(0, result[0].id);
                }

                // ###2
                {
                    var query = GetQueryable();

                    var strRule = "{'field':'isEven',  'operator': '=',  'value': 'false' }".Replace("'", "\"");
                    var rule = GetRule(strRule);
                    var result = Filter(ToQuery(query), rule);
                    Assert.AreEqual(result.Count, 500);
                    Assert.AreEqual(1, result[0].id);
                }
            }
            #endregion

            #region ##2.1 [number] !=
            {
                // ###1
                {
                    var query = GetQueryable();

                    var strRule = "{'field':'id',  'operator': '!=',  'value':10 }".Replace("'", "\"");
                    var rule = GetRule(strRule);
                    var result = Filter(ToQuery(query), rule);
                    Assert.AreEqual(999, result.Count);
                }

                // ###2 != null
                {
                    var query = GetQueryable();

                    var item = query.Skip(10).FirstOrDefault();
                    item.pid = null;

                    var strRule = "{'field':'pid',  'operator': '!=',  'value': null }".Replace("'", "\"");
                    var rule = GetRule(strRule);
                    var result = Filter(ToQuery(query), rule);
                    Assert.AreEqual(999, result.Count);
                }
            }
            #endregion

            #region ##2.2 [bool] !=
            {
                // ###1
                {
                    var query = GetQueryable();

                    var strRule = "{'field':'isEven',  'operator': '!=',  'value':true }".Replace("'", "\"");
                    var rule = GetRule(strRule);
                    var result = Filter(ToQuery(query), rule);
                    Assert.AreEqual(result.Count, 500);
                    Assert.AreEqual(1, result[0].id);
                }

                // ###2
                {
                    var query = GetQueryable();

                    var strRule = "{'field':'isEven',  'operator': '!=',  'value': 'false' }".Replace("'", "\"");
                    var rule = GetRule(strRule);
                    var result = Filter(ToQuery(query), rule);
                    Assert.AreEqual(result.Count, 500);
                    Assert.AreEqual(0, result[0].id);
                }
            }
            #endregion


            #region ##3 [number] > <
            {
                {
                    var query = GetQueryable();

                    var strRule = @"{'condition':'and', 'rules':[   
                                        {'field':'id',  'operator': '>',  'value':10  },
                                        {'field':'id',  'operator': '<',  'value': '20' } 
                                    ]}".Replace("'", "\"");
                    var rule = GetRule(strRule);
                    var result = Filter(ToQuery(query), rule);
                    Assert.AreEqual(9, result.Count);
                    Assert.AreEqual(11, result[0].id);
                }
            }
            #endregion


            #region ##4 [number] >= <=
            {
                {
                    var query = GetQueryable();

                    var strRule = @"{'condition':'and', 'rules':[   
                                        {'field':'id',  'operator': '>=',  'value':10  },
                                        {'field':'id',  'operator': '<=',  'value': '20' } 
                                    ]}".Replace("'", "\"");
                    var rule = GetRule(strRule);
                    var result = Filter(ToQuery(query), rule);
                    Assert.AreEqual(11, result.Count);
                    Assert.AreEqual(10, result[0].id);
                }
            }
            #endregion

            #endregion


            #region #3  In | NotIn

            #region ##1 In
            {
                {
                    var query = GetQueryable();

                    var strRule = "{'field':'id',  'operator': 'In',  'value': [3,4,5] }".Replace("'", "\"");
                    var rule = GetRule(strRule);
                    var result = Filter(ToQuery(query), rule);
                    Assert.AreEqual(3, result.Count);
                    Assert.AreEqual(5, result[2].id);
                }

                {
                    var query = GetQueryable();

                    var strRule = "{'field':'name',  'operator': 'In',  'value': [ 'name3', 'name4'] }".Replace("'", "\"");
                    var rule = GetRule(strRule);
                    var result = Filter(ToQuery(query), rule);
                    Assert.AreEqual(2, result.Count);
                    Assert.AreEqual("name3", result[0].name);
                }

                {
                    var query = GetQueryable();
                    query.First().name = null;

                    var strRule = @"{'condition':'or', 'rules':[
                                        {'field':'name',  'operator': 'IsNull' },
                                        {'field':'name',  'operator': 'In',  'value': [ 'name3', 'name4'] } 
                                    ]}".Replace("'", "\"");
                    var rule = GetRule(strRule);
                    var result = Filter(ToQuery(query), rule);
                    Assert.AreEqual(3, result.Count);
                    Assert.AreEqual(null, result[0].name);
                    Assert.AreEqual("name4", result[2].name);
                }

                {
                    var query = GetQueryable();
                    query.First().name = null;

                    var strRule = @"{'field':'name',  'operator': 'In',  'value': [ 'name3', 'name4', null ] }".Replace("'", "\"");
                    var rule = GetRule(strRule);
                    var result = Filter(ToQuery(query), rule);
                    Assert.AreEqual(3, result.Count);
                    Assert.AreEqual(null, result[0].name);
                    Assert.AreEqual("name4", result[2].name);
                }
            }
            #endregion

            #region ##2  NotIn
            {
                var query = GetQueryable();
                query.FirstOrDefault().name = null;

                var strRule = @"{'condition':'and', 'rules':[   
                                        {'field':'name',  'operator': 'IsNotNull' },
                                        {'field':'name',  'operator': 'NotIn',  'value': [ 'name3', 'name4'] } 
                                    ]}".Replace("'", "\"");
                var rule = GetRule(strRule);
                var result = Filter(ToQuery(query), rule);
                Assert.AreEqual(997, result.Count);
            }
            #endregion

            #endregion


            #region #4 [string] operate

            #region ##1  Contains
            {
                var query = GetQueryable();

                var strRule = "{'field':'name',  'operator': 'Contains',  'value': '987' }".Replace("'", "\"");
                var rule = GetRule(strRule);
                var result = Filter(ToQuery(query), rule);

                Assert.AreEqual(1, result.Count);
                Assert.AreEqual(987, result.First().id);
            }
            #endregion

            #region ##2  NotContains
            {
                //###1
                {
                    var query = GetQueryable();

                    var strRule = "{'field':'name',  'operator': 'NotContains',  'value': '987' }".Replace("'", "\"");
                    var rule = GetRule(strRule);
                    var result = Filter(ToQuery(query), rule);
                    Assert.AreEqual(999, result.Count);
                }


                //###2
                {
                    var query = GetQueryable();
                    query.Skip(987).FirstOrDefault().name = null;

                    var strRule = "{'field':'name',  'operator': 'NotContains',  'value': '987' }".Replace("'", "\"");
                    var rule = GetRule(strRule);
                    var result = Filter(ToQuery(query), rule);
                    Assert.AreEqual(1000, result.Count);
                }

                //###3
                {
                    var query = GetQueryable();
                    query.Skip(987).FirstOrDefault().name = "";

                    var strRule = "{'field':'name',  'operator': 'NotContains',  'value': '987' }".Replace("'", "\"");
                    var rule = GetRule(strRule);
                    var result = Filter(ToQuery(query), rule);
                    Assert.AreEqual(1000, result.Count);
                }
            }
            #endregion

            #region ##3  StartsWith
            {
                var query = GetQueryable();

                var strRule = "{'field':'name',  'operator': 'StartsWith',  'value': 'name98' }".Replace("'", "\"");
                var rule = GetRule(strRule);
                var result = Filter(ToQuery(query), rule);

                Assert.AreEqual(11, result.Count);
            }
            #endregion

            #region ##4  EndsWith
            {
                var query = GetQueryable();

                var strRule = "{'field':'name',  'operator': 'EndsWith',  'value': '987' }".Replace("'", "\"");
                var rule = GetRule(strRule);
                var result = Filter(ToQuery(query), rule);

                Assert.AreEqual(1, result.Count);
            }
            #endregion

            #region ##5 IsNullOrEmpty
            {
                //###1
                {
                    var query = GetQueryable();
                    query.Skip(10).FirstOrDefault().name = null;

                    var strRule = "{'field':'name',  'operator': 'IsNullOrEmpty' }".Replace("'", "\"");
                    var rule = GetRule(strRule);
                    var result = Filter(ToQuery(query), rule);

                    Assert.AreEqual(1, result.Count);

                }

                //###2
                {
                    var query = GetQueryable();
                    query.Skip(10).FirstOrDefault().name = "";

                    var strRule = "{'field':'name',  'operator': 'IsNullOrEmpty' }".Replace("'", "\"");
                    var rule = GetRule(strRule);
                    var result = Filter(ToQuery(query), rule);

                    Assert.AreEqual(1, result.Count);
                }
            }
            #endregion

            #region ##6  IsNotNullOrEmpty
            {
                //###1
                {
                    var query = GetQueryable();
                    query.Skip(10).FirstOrDefault().name = null;

                    var strRule = "{'field':'name',  'operator': 'IsNotNullOrEmpty' }".Replace("'", "\"");
                    var rule = GetRule(strRule);
                    var result = Filter(ToQuery(query), rule);

                    Assert.AreEqual(999, result.Count);
                }
                //###2
                {
                    var query = GetQueryable();
                    query.Skip(10).FirstOrDefault().name = "";

                    var strRule = "{'field':'name',  'operator': 'IsNotNullOrEmpty' }".Replace("'", "\"");
                    var rule = GetRule(strRule);
                    var result = Filter(ToQuery(query), rule);

                    Assert.AreEqual(999, result.Count);
                }
            }
            #endregion


            #endregion


            #region #5  DateTime
            {
                // ##1 string =
                {
                    var query = GetQueryable();
                    var model = query.Skip(100).First();
                    var strTime = model.addTime.ToString("yyyy-MM-dd HH:mm:ss");
                    model.addTime = DateTime.Parse(strTime);
                    var strRule = $"{{'field':'addTime',  'operator': '=',  'value': '{strTime}' }}".Replace("'", "\"");
                    var rule = GetRule(strRule);

                    var result = Filter(ToQuery(query), rule);

                    var model2 = result.First();
                    Assert.AreEqual(1, result.Count);
                    Assert.AreEqual(model.id, model2.id);
                }

                // ##2 string <
                {
                    var query = GetQueryable();
                    var model = query.Skip(100).First();
                    var strTime = model.addTime.ToString("yyyy-MM-dd HH:mm:ss");
                    model.addTime = DateTime.Parse(strTime);
                    var strRule = $"{{'field':'addTime',  'operator': '<',  'value': '{strTime}' }}".Replace("'", "\"");
                    var rule = GetRule(strRule);

                    var result = Filter(ToQuery(query), rule);

                    Assert.AreEqual(100, result.Count);
                }


                // ##3 DateTime =
                {
                    var query = GetQueryable();
                    var model = query.Skip(100).First(); 
                    var strRule = $"{{'field':'addTime',  'operator': '=' }}".Replace("'", "\"");
                    var rule = GetRule(strRule);
                    rule.value = model.addTime;

                    var result = Filter(ToQuery(query), rule);

                    var model2 = result.First();
                    Assert.AreEqual(1, result.Count);
                    Assert.AreEqual(model.id, model2.id);
                }

                // ##4 DateTime <
                {
                    var query = GetQueryable();
                    var model = query.Skip(100).First();
                    var strRule = $"{{'field':'addTime',  'operator': '<' }}".Replace("'", "\"");
                    var rule = GetRule(strRule);
                    rule.value = model.addTime;

                    var result = Filter(ToQuery(query), rule);

                    Assert.AreEqual(100, result.Count);
                }
            }
            #endregion



            #region #6  nested field
            {
                var query = GetQueryable();
                var strRule = "{'field':'b1.name',  'operator': '=',  'value': 'name987_b1' }".Replace("'", "\"");
                var rule = GetRule(strRule);

                var result = Filter(ToQuery(query), rule);

                Assert.AreEqual(1, result.Count);
                Assert.AreEqual(987, result[0].id);
            }
            #endregion

        }



    }
}

