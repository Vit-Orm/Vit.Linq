using System;
using System.Linq;
using System.Linq.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Vit.Core.Module.Serialization;
using Vit.Linq.Filter;
using Vit.Linq.Filter.ComponentModel;
using Vit.Linq.Filter.FilterConvertor;

namespace Vit.Linq.MsTest.Filter
{
    [TestClass]
    public class FilterService_CustomCondition_Test
    {
        [TestMethod]
        public void Test_CustomCondition()
        {

            // #1 ignore empty filter
            {
                #region Get FilterService
                var service = new FilterService();
                Func<FilterConvertArgument, IFilterRule, string, (bool success, Expression expression)> convertor;
                convertor = (arg, filter, condition) =>
                {
                    if (string.IsNullOrWhiteSpace(condition) && string.IsNullOrWhiteSpace(filter.@operator)) return (true, Expression.Constant(true));

                    return default;
                };
                service.RegisterConditionConvertor(null, convertor);
                #endregion


                //
                {
                    var query = DataSource.GetQueryable();

                    var strRule = "{  }".Replace("'", "\"");
                    var rule = Json.Deserialize<FilterRule>(strRule);
                    var result = query.Where(rule, service).ToList();
                    Assert.AreEqual(1000, result.Count);
                }

                //
                {
                    var query = DataSource.GetQueryable();

                    var strRule = "{ 'condition':'not', 'rules':[ { } ]  }".Replace("'", "\"");
                    var rule = Json.Deserialize<FilterRule>(strRule);
                    var result = query.Where(rule, service).ToList();
                    Assert.AreEqual(0, result.Count);
                }



            }



        }
    }



}
