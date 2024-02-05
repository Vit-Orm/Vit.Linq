using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vit.Core.Module.Serialization;
using Vit.Linq.Filter;
using Vit.Extensions.Linq_Extensions;
using System.Linq.Expressions;

namespace Vit.Linq.MsTest.Filter
{

    [TestClass]
    public class CustomOperator_Test
    {
        [TestMethod]
        public void Test_CustomOperator()
        {

            // #1 no arguments
            {
                #region Get FilterService
                var service = new FilterService();
                Func<OperatorBuilderArgs, Expression> operatorBuilder;
                operatorBuilder = (OperatorBuilderArgs args) =>
                {
                    var operatorExpression = Expression.Call(typeof(TestExtensions), "IsEven", null, args.leftValue);
                    return operatorExpression;
                };
                service.CustomOperator_Add("IsEven", operatorBuilder);
                #endregion


                // IsEven
                {
                    var query = DataSource.GetQueryable();

                    var strRule = "{'field':'id',  'operator': 'IsEven'  }".Replace("'", "\"");
                    var rule = Json.Deserialize<FilterRule>(strRule);
                    var result = query.Where(rule, service).ToList();
                    Assert.AreEqual(500, result.Count);
                }
            }



            // #2 1 argument
            {
                #region Get FilterService
                var service = new FilterService();
                Func<OperatorBuilderArgs, Expression> operatorBuilder;
                operatorBuilder = (OperatorBuilderArgs args) =>
                {
                    var method = typeof(TestExtensions).GetMethod("IsEvenOrOdd");

                    var valueType = typeof(bool);
                    var rightValueExpression = args.GetRightValueExpression(valueType);

                    var operatorExpression = Expression.Call(null, method, new[] { args.leftValue, rightValueExpression });
                    return operatorExpression;
                };
                service.CustomOperator_Add("IsEvenOrOdd", operatorBuilder);
                #endregion


                // IsEvenOrOdd
                {
                    var query = DataSource.GetQueryable();

                    var strRule = "{'field':'id',  'operator': 'IsEvenOrOdd',  'value':false  }".Replace("'", "\"");
                    var rule = Json.Deserialize<FilterRule>(strRule);
                    var result = query.Where(rule, service).ToList();
                    Assert.AreEqual(500, result.Count);
                }
            }


            // #3   by methodName or Method
            {
                #region Get FilterService
                var service = new FilterService();
                Func<OperatorBuilderArgs, Expression> operatorBuilder;

                operatorBuilder = (OperatorBuilderArgs args) =>
                {
                    var operatorExpression = Expression.Call(typeof(TestExtensions), "IsDivisibleBy", null, args.leftValue, args.GetRightValueExpression(typeof(int)));
                    return operatorExpression;
                };
                service.CustomOperator_Add("IsDivisibleBy", operatorBuilder);

                operatorBuilder = (OperatorBuilderArgs args) =>
                {
                    var method = typeof(TestExtensions).GetMethod("IsDivisibleBy");
                    var operatorExpression = Expression.Call(null, method, new[] { args.leftValue, args.GetRightValueExpression(typeof(int)) });
                    return operatorExpression;
                };
                service.CustomOperator_Add("IsDivisibleBy2", operatorBuilder);
                #endregion


                // IsDivisibleBy
                {
                    var query = DataSource.GetQueryable();

                    var strRule = "{'field':'id',  'operator': 'IsDivisibleBy',  'value':10 }".Replace("'", "\"");
                    var rule = Json.Deserialize<FilterRule>(strRule);
                    var result = query.Where(rule, service).ToList();
                    Assert.AreEqual(100, result.Count);
                    Assert.AreEqual(0, result[0].id);
                    Assert.AreEqual(10, result[1].id);
                }

                // IsDivisibleBy2
                {
                    var query = DataSource.GetQueryable();

                    var strRule = "{'field':'id',  'operator': 'IsDivisibleBy2',  'value':10 }".Replace("'", "\"");
                    var rule = Json.Deserialize<FilterRule>(strRule);
                    var result = query.Where(rule, service).ToList();
                    Assert.AreEqual(100, result.Count);
                    Assert.AreEqual(0, result[0].id);
                    Assert.AreEqual(10, result[1].id);
                }
            }



            // #4 right value is the value of another field
            {
                #region Get FilterService
                var service = new FilterService();
                Func<OperatorBuilderArgs, Expression> operatorBuilder;
                operatorBuilder = (OperatorBuilderArgs args) =>
                {
                    var rightValueExpression = LinqHelp.GetFieldMemberExpression(args.parameter, (string)args.rule.value);
                    return Expression.Equal(args.leftValue, rightValueExpression);
                };
                service.CustomOperator_Add("EqualTo", operatorBuilder);
                #endregion

                // EqualTo another field
                {
                    var query = DataSource.GetQueryable();

                    var strRule = "{'field':'id',  'operator': 'EqualTo',  'value':'jobList.Count' }".Replace("'", "\"");
                    var rule = Json.Deserialize<FilterRule>(strRule);
                    var result = query.Where(rule, service).ToList();
                    Assert.AreEqual(2, result.Count);
                    Assert.AreEqual(1, result[0].id);
                    Assert.AreEqual(2, result[1].id);
                }
            }




            // #5 String.Contains OrdinalIgnoreCase
            {
                #region Get FilterService
                var service = new FilterService();
                Func<OperatorBuilderArgs, Expression> operatorBuilder;
                operatorBuilder = (OperatorBuilderArgs args) =>
                {
                    //"string".Contains("Str",StringComparison.OrdinalIgnoreCase)

                    var leftValueExpression = args.leftValue;
                    var valueType = typeof(string);
                    var rightValueExpression = args.GetRightValueExpression(valueType);
                    var comparison = Expression.Constant(StringComparison.OrdinalIgnoreCase);

                    var nullCheck = Expression.Call(typeof(string), "IsNullOrEmpty", null, leftValueExpression);
                    var contains = Expression.Call(leftValueExpression, "Contains", null, rightValueExpression, comparison);

                    return Expression.AndAlso(Expression.Not(nullCheck), contains);
                };
                service.CustomOperator_Add("Contains", operatorBuilder);
                #endregion


                {
                    var query = DataSource.GetQueryable();

                    var strRule = "{'field':'name',  'operator': 'Contains',  'value': 'Name987' }".Replace("'", "\"");
                    var rule = Json.Deserialize<FilterRule>(strRule);
                    var result = query.Where(rule, service).ToList();

                    Assert.AreEqual(1, result.Count);
                    Assert.AreEqual(987, result.First().id);
                }
            }

        }
    }




    public static class TestExtensions
    {
        public static bool IsEven(this int number)
        {
            return number % 2 == 0;
        }

        public static bool IsOdd(this int number)
        {
            return number % 2 != 0;
        }

        public static bool IsEvenOrOdd(this int number, bool isEven)
        {
            return (number % 2 == 0) == isEven;
        }

        public static bool IsDivisibleBy(this int number, int divisor)
        {
            return number % divisor == 0;
        }

    }

}
