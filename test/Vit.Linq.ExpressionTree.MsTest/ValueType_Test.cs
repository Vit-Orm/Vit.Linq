
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ValueType = Vit.Linq.ExpressionTree.ComponentModel.ValueType;

namespace Vit.Linq.ExpressionTree.MsTest
{
    [TestClass]
    public class ValueType_Test
    {
        [TestMethod]
        public void ConvertToPrimitiveType_Test()
        {

            #region ValueType
            // int
            {
                var type = typeof(int);
                int expectedValue = 12;
                object value;

                value = ValueType.ConvertToPrimitiveType(12.0, type);
                Assert.AreEqual(expectedValue, value);

                value = ValueType.ConvertToPrimitiveType(12.1, type);
                Assert.AreEqual(expectedValue, value);

                value = ValueType.ConvertToPrimitiveType(12, type);
                Assert.AreEqual(expectedValue, value);


                value = ValueType.ConvertToPrimitiveType("12", type);
                Assert.AreEqual(expectedValue, value);

                value = ValueType.ConvertToPrimitiveType("12.0", type);
                Assert.AreEqual(expectedValue, value);

                value = ValueType.ConvertToPrimitiveType("12.1", type);
                Assert.AreEqual(expectedValue, value);

                // nullable
                value = ValueType.ConvertToPrimitiveType(null, type);
                Assert.AreEqual(0, value);

                value = ValueType.ConvertToPrimitiveType((int?)12, type);
                Assert.AreEqual(expectedValue, value);

                value = ValueType.ConvertToPrimitiveType((double?)12.0, type);
                Assert.AreEqual(expectedValue, value);

                value = ValueType.ConvertToPrimitiveType((double?)12.1, type);
                Assert.AreEqual(expectedValue, value);

            }
            // int2
            {
                var type = typeof(int);
                int expectedValue = -12;

                var value = ValueType.ConvertToPrimitiveType(-12.1, type);
                Assert.AreEqual(expectedValue, value);
            }
            // byte
            {
                var type = typeof(byte);
                byte expectedValue = 12;
                object value;


                value = ValueType.ConvertToPrimitiveType(12.0, type);
                Assert.AreEqual(expectedValue, value);

                value = ValueType.ConvertToPrimitiveType(12.1, type);
                Assert.AreEqual(expectedValue, value);

                value = ValueType.ConvertToPrimitiveType(12, type);
                Assert.AreEqual(expectedValue, value);


                value = ValueType.ConvertToPrimitiveType("12", type);
                Assert.AreEqual(expectedValue, value);

                value = ValueType.ConvertToPrimitiveType("12.0", type);
                Assert.AreEqual(expectedValue, value);

                value = ValueType.ConvertToPrimitiveType("12.1", type);
                Assert.AreEqual(expectedValue, value);
            }
            // float
            {
                var type = typeof(float);
                float expectedValue = 12.123f;
                object value;

                value = ValueType.ConvertToPrimitiveType(12.123, type);
                Assert.AreEqual(expectedValue, value);

                value = ValueType.ConvertToPrimitiveType("12.123", type);
                Assert.AreEqual(expectedValue, value);



                expectedValue = 12;
                value = ValueType.ConvertToPrimitiveType(12, type);
                Assert.AreEqual(expectedValue, value);

                value = ValueType.ConvertToPrimitiveType("12", type);
                Assert.AreEqual(expectedValue, value);
            }


            // bool
            {
                var type = typeof(bool);
                bool expectedValue = true;
                object value;

                value = ValueType.ConvertToPrimitiveType(true, type);
                Assert.AreEqual(expectedValue, value);

                value = ValueType.ConvertToPrimitiveType("true", type);
                Assert.AreEqual(expectedValue, value);


                expectedValue = false;
                value = ValueType.ConvertToPrimitiveType("false", type);
                Assert.AreEqual(expectedValue, value);
            }

            // DateTime
            {
                var type = typeof(DateTime);
                object value;

                var strDate = "2001-02-03 04:05:06";
                var date = DateTime.Parse(strDate);

                value = ValueType.ConvertToPrimitiveType(strDate, type);
                Assert.AreEqual(date, value);

                value = ValueType.ConvertToPrimitiveType((DateTime?)date, type);
                Assert.AreEqual(date, value);

                value = ValueType.ConvertToPrimitiveType(null, type);
                Assert.AreEqual(DateTime.MinValue, value);

                value = ValueType.ConvertToPrimitiveType("2001-02-03T04:05:06Z", type);
                value = ((DateTime)value).ToUniversalTime();
                Assert.AreEqual(date, value);
            }


            // string
            {
                var type = typeof(string);
                object value;

                // ##1 string -> string
                value = ValueType.ConvertToPrimitiveType("trueasdge", type);
                Assert.AreEqual("trueasdge", value);


                // ##2 bool -> string
                value = ValueType.ConvertToPrimitiveType(true, type);
                Assert.AreEqual("true", value);

                value = ValueType.ConvertToPrimitiveType(false, type);
                Assert.AreEqual("false", value);

                value = ValueType.ConvertToPrimitiveType((bool?)true, type);
                Assert.AreEqual("true", value);

                value = ValueType.ConvertToPrimitiveType(null, type);
                Assert.AreEqual(null, value);


                // ##3 double -> string
                value = ValueType.ConvertToPrimitiveType(12.1, type);
                Assert.AreEqual("12.1", value);

                value = ValueType.ConvertToPrimitiveType(12, type);
                Assert.AreEqual("12", value);


                value = ValueType.ConvertToPrimitiveType(null, type);
                Assert.AreEqual(null, value);
            }
            #endregion

        }


        [TestMethod]
        public void ConvertToType_Nullable_Test()
        {
            Type type;
            object value;
            object expectedValue;

            #region Nullable
            // int
            {
                type = typeof(int?);
                expectedValue = (int?)12;
                int? v;

                value = ValueType.ConvertValueToType(12.0, type);
                v = value as int?;
                Assert.AreEqual(expectedValue, v);

                value = ValueType.ConvertValueToType(12.1, type);
                v = value as int?;
                Assert.AreEqual(expectedValue, v);

                value = ValueType.ConvertValueToType(12, type);
                v = value as int?;
                Assert.AreEqual(expectedValue, v);


                value = ValueType.ConvertValueToType("12", type);
                v = value as int?;
                Assert.AreEqual(expectedValue, v);

                value = ValueType.ConvertValueToType("12.0", type);
                v = value as int?;
                Assert.AreEqual(expectedValue, v);

                value = ValueType.ConvertValueToType("12.1", type);
                v = value as int?;
                Assert.AreEqual(expectedValue, v);

                // nullable
                value = ValueType.ConvertValueToType(null, type);
                v = value as int?;
                Assert.AreEqual(null, v);

                value = ValueType.ConvertValueToType((int?)12, type);
                v = value as int?;
                Assert.AreEqual(expectedValue, v);

                value = ValueType.ConvertValueToType((double?)12.0, type);
                v = value as int?;
                Assert.AreEqual(expectedValue, v);

                value = ValueType.ConvertValueToType((double?)12.1, type);
                v = value as int?;
                Assert.AreEqual(expectedValue, v);

            }

            // bool
            {
                type = typeof(bool?);
                expectedValue = (bool?)true;

                value = ValueType.ConvertValueToType(true, type);
                Assert.AreEqual(expectedValue, value);

                value = ValueType.ConvertValueToType("true", type);
                Assert.AreEqual(expectedValue, value);

                value = ValueType.ConvertValueToType(null, type);
                Assert.AreEqual(null, value);

                value = ValueType.ConvertValueToType("false", type);
                Assert.AreEqual((bool?)false, value);
            }
            #endregion
        }


        [TestMethod]
        public void ConvertToType_Array_Test()
        {

            #region One-dimensional Array
            // int []
            {
                var type = typeof(int[]);
                int expectedValue = 12;

                var value = ValueType.ConvertValueToType(new[] { 12.1 }, type) as int[];
                Assert.AreEqual(expectedValue, value[0]);
            }

            // int? []
            {
                var type = typeof(int?[]);
                int? expectedValue = 12;

                var value = ValueType.ConvertValueToType(new[] { 12.1 }, type) as int?[];
                Assert.AreEqual(expectedValue, value[0]);
            }
            #endregion


            #region One-dimensional Collection
            // double[]  ->  List<int>
            {
                var type = typeof(List<int>);
                int expectedValue = 12;

                var value = ValueType.ConvertValueToType(new[] { 12.1 }, type) as List<int>;
                Assert.AreEqual(expectedValue, value[0]);
            }
            // IEnumerable<double>  ->  IEnumerable<int>
            {
                var type = typeof(IEnumerable<int>);
                int expectedValue = 12;
                IEnumerable<double> oriValue = new[] { 11.1 }.Select(m => m + 1);

                var value = ValueType.ConvertValueToType(oriValue, type) as IEnumerable<int>;
                Assert.AreEqual(expectedValue, value.First());
            }
            // IEnumerable<double>  ->  IQueryable<int>
            {
                var type = typeof(IQueryable<int>);
                int expectedValue = 12;
                IEnumerable<double> oriValue = new[] { 11.1 }.Select(m => m + 1);

                var value = ValueType.ConvertValueToType(oriValue, type) as IQueryable<int>;
                Assert.AreEqual(expectedValue, value.First());
            }
            // IQueryable<double>  ->  ICollection<string>
            {
                var type = typeof(ICollection<string>);
                var expectedValue = "12.1";
                IQueryable<double> oriValue = new[] { 11.1 }.AsQueryable().Select(m => m + 1);

                var value = ValueType.ConvertValueToType(oriValue, type) as ICollection<string>;
                Assert.AreEqual(expectedValue, value.First());
            }
            #endregion


            #region Two-dimensional
            // double [][] -> int [][]
            {
                var type = typeof(int[][]);
                int expectedValue = 12;

                var value = ValueType.ConvertValueToType(new[] { new[] { 12.1 } }, type) as int[][];
                Assert.AreEqual(expectedValue, value[0][0]);
            }
            #endregion
        }




    }
}
