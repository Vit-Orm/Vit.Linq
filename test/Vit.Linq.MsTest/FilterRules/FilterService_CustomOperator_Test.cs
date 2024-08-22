using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Vit.Core.Module.Serialization;
using Vit.Linq.FilterRules;
using Vit.Linq.FilterRules.ComponentModel;
using Vit.Linq.FilterRules.FilterConvertor;

namespace Vit.Linq.MsTest.FilterRules
{

    [TestClass]
    public class FilterService_CustomOperator_Test
    {
        [TestMethod]
        public void Test_CustomOperator()
        {

            // #1 no arguments
            {
                #region Get FilterService
                var service = new FilterService();
                Func<OperatorConvertArgument, Expression> convertor;
                convertor = (arg) =>
                {
                    var operatorExpression = Expression.Call(typeof(TestExtensions), "IsEven", null, arg.leftValueExpression);
                    return operatorExpression;
                };
                service.RegisterOperatorConvertor("IsEven", convertor);
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
                Func<OperatorConvertArgument, Expression> convertor;
                convertor = (arg) =>
                {
                    var method = typeof(TestExtensions).GetMethod("IsEvenOrOdd");

                    var rightValueType = typeof(bool);
                    var rightValueExpression = arg.filterService.GetRightValueExpression(arg.filter, arg.parameter, rightValueType);

                    var operatorExpression = Expression.Call(null, method, new[] { arg.leftValueExpression, rightValueExpression });
                    return operatorExpression;
                };
                service.RegisterOperatorConvertor("IsEvenOrOdd", convertor);
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
                Func<OperatorConvertArgument, Expression> convertor;

                convertor = (arg) =>
                {
                    var rightValueType = typeof(int);
                    var rightValueExpression = arg.filterService.GetRightValueExpression(arg.filter, arg.parameter, rightValueType);

                    var operatorExpression = Expression.Call(typeof(TestExtensions), "IsDivisibleBy", null, arg.leftValueExpression, rightValueExpression);
                    return operatorExpression;
                };
                service.RegisterOperatorConvertor("IsDivisibleBy", convertor);

                convertor = (arg) =>
                {
                    var rightValueType = typeof(int);
                    var rightValueExpression = arg.filterService.GetRightValueExpression(arg.filter, arg.parameter, rightValueType);

                    var method = typeof(TestExtensions).GetMethod("IsDivisibleBy");
                    var operatorExpression = Expression.Call(null, method, new[] { arg.leftValueExpression, rightValueExpression });
                    return operatorExpression;
                };
                service.RegisterOperatorConvertor("IsDivisibleBy2", convertor);
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
                Func<OperatorConvertArgument, Expression> convertor;
                convertor = (arg) =>
                {
                    string memberPath = (string)arg.filter.value;
                    var rightValueExpression = LinqHelp.GetFieldMemberExpression(arg.parameter, memberPath);
                    return Expression.Equal(arg.leftValueExpression, rightValueExpression);
                };
                service.RegisterOperatorConvertor("EqualTo", convertor);
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
                Func<OperatorConvertArgument, Expression> convertor;
                convertor = (arg) =>
                {
                    //"string".Contains("Str",StringComparison.OrdinalIgnoreCase)

                    var leftValueExpression = arg.leftValueExpression;
                    var rightValueType = typeof(string);
                    var rightValueExpression = arg.filterService.GetRightValueExpression(arg.filter, arg.parameter, rightValueType);
                    var comparison = Expression.Constant(StringComparison.OrdinalIgnoreCase);

                    var nullCheck = Expression.Call(typeof(string), "IsNullOrEmpty", null, leftValueExpression);
                    var contains = Expression.Call(leftValueExpression, "Contains", null, rightValueExpression, comparison);

                    return Expression.AndAlso(Expression.Not(nullCheck), contains);
                };
                service.RegisterOperatorConvertor("Contains", convertor);
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




            // #6 In OrdinalIgnoreCase
            {
                #region Get FilterService
                var service = new FilterService();

                var stringListContainsMethod = new Func<IEnumerable<string>, string, IEqualityComparer<string>, bool>(Enumerable.Contains<string>).GetMethodInfo();
                Expression GetInExp(OperatorConvertArgument arg)
                {
                    Type leftValueType = arg.leftValueExpression.Type;

                    Type rightValueType = typeof(List<>).MakeGenericType(leftValueType);
                    var rightValueExpression = arg.filterService.GetRightValueExpression(arg.filter, arg.parameter, rightValueType);

                    if (leftValueType == typeof(string))
                    {
                        // #1 List<string>.Contains
                        var comparison = Expression.Constant(StringComparer.OrdinalIgnoreCase);
                        return Expression.Call(stringListContainsMethod, rightValueExpression, arg.leftValueExpression, comparison);
                    }
                    else
                    {
                        // #2 List<>.Contains
                        return Expression.Call(rightValueExpression, "Contains", null, arg.leftValueExpression);
                    }
                }

                Func<OperatorConvertArgument, Expression> convertor;
                convertor = GetInExp;
                service.RegisterOperatorConvertor("In", convertor);
                convertor = (arg) => Expression.Not(GetInExp(arg));
                service.RegisterOperatorConvertor("NotIn", convertor);
                #endregion


                {
                    var query = DataSource.GetQueryable();

                    var strRule = "{'field':'name',  'operator': 'In',  'value': ['Name987','nAme988'] }".Replace("'", "\"");
                    var rule = Json.Deserialize<FilterRule>(strRule);
                    var result = query.Where(rule, service).ToList();

                    Assert.AreEqual(2, result.Count);
                    Assert.AreEqual(987, result.First().id);
                }


                {
                    var query = DataSource.GetQueryable();

                    var strRule = "{'field':'name',  'operator': 'NotIn',  'value': ['Name987','nAme988'] }".Replace("'", "\"");
                    var rule = Json.Deserialize<FilterRule>(strRule);
                    var result = query.Where(rule, service).ToList();

                    Assert.AreEqual(998, result.Count);
                }

                {
                    var query = DataSource.GetQueryable();

                    var strRule = "{'field':'id',  'operator': 'In',  'value': [3,4,5] }".Replace("'", "\"");
                    var rule = Json.Deserialize<FilterRule>(strRule);
                    var result = query.Where(rule, service).ToList();
                    Assert.AreEqual(3, result.Count);
                    Assert.AreEqual(5, result[2].id);
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
