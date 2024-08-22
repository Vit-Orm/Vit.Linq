using System.Data;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using NodeValueType = Vit.Linq.ExpressionNodes.ComponentModel.NodeValueType;

namespace Vit.Linq.ExpressionNodes.MsTest
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

                value = NodeValueType.ConvertToPrimitiveType(12.0, type);
                Assert.AreEqual(expectedValue, value);

                value = NodeValueType.ConvertToPrimitiveType(12.1, type);
                Assert.AreEqual(expectedValue, value);

                value = NodeValueType.ConvertToPrimitiveType(12, type);
                Assert.AreEqual(expectedValue, value);


                value = NodeValueType.ConvertToPrimitiveType("12", type);
                Assert.AreEqual(expectedValue, value);

                value = NodeValueType.ConvertToPrimitiveType("12.0", type);
                Assert.AreEqual(expectedValue, value);

                value = NodeValueType.ConvertToPrimitiveType("12.1", type);
                Assert.AreEqual(expectedValue, value);

                // nullable
                value = NodeValueType.ConvertToPrimitiveType(null, type);
                Assert.AreEqual(0, value);

                value = NodeValueType.ConvertToPrimitiveType((int?)12, type);
                Assert.AreEqual(expectedValue, value);

                value = NodeValueType.ConvertToPrimitiveType((double?)12.0, type);
                Assert.AreEqual(expectedValue, value);

                value = NodeValueType.ConvertToPrimitiveType((double?)12.1, type);
                Assert.AreEqual(expectedValue, value);

            }
            // int2
            {
                var type = typeof(int);
                int expectedValue = -12;

                var value = NodeValueType.ConvertToPrimitiveType(-12.1, type);
                Assert.AreEqual(expectedValue, value);
            }
            // byte
            {
                var type = typeof(byte);
                byte expectedValue = 12;
                object value;


                value = NodeValueType.ConvertToPrimitiveType(12.0, type);
                Assert.AreEqual(expectedValue, value);

                value = NodeValueType.ConvertToPrimitiveType(12.1, type);
                Assert.AreEqual(expectedValue, value);

                value = NodeValueType.ConvertToPrimitiveType(12, type);
                Assert.AreEqual(expectedValue, value);


                value = NodeValueType.ConvertToPrimitiveType("12", type);
                Assert.AreEqual(expectedValue, value);

                value = NodeValueType.ConvertToPrimitiveType("12.0", type);
                Assert.AreEqual(expectedValue, value);

                value = NodeValueType.ConvertToPrimitiveType("12.1", type);
                Assert.AreEqual(expectedValue, value);
            }
            // float
            {
                var type = typeof(float);
                float expectedValue = 12.123f;
                object value;

                value = NodeValueType.ConvertToPrimitiveType(12.123, type);
                Assert.AreEqual(expectedValue, value);

                value = NodeValueType.ConvertToPrimitiveType("12.123", type);
                Assert.AreEqual(expectedValue, value);



                expectedValue = 12;
                value = NodeValueType.ConvertToPrimitiveType(12, type);
                Assert.AreEqual(expectedValue, value);

                value = NodeValueType.ConvertToPrimitiveType("12", type);
                Assert.AreEqual(expectedValue, value);
            }


            // bool
            {
                var type = typeof(bool);
                bool expectedValue = true;
                object value;

                value = NodeValueType.ConvertToPrimitiveType(true, type);
                Assert.AreEqual(expectedValue, value);

                value = NodeValueType.ConvertToPrimitiveType("true", type);
                Assert.AreEqual(expectedValue, value);


                expectedValue = false;
                value = NodeValueType.ConvertToPrimitiveType("false", type);
                Assert.AreEqual(expectedValue, value);
            }

            // DateTime
            {
                var type = typeof(DateTime);
                object value;

                var strDate = "2001-02-03 04:05:06";
                var date = DateTime.Parse(strDate);

                value = NodeValueType.ConvertToPrimitiveType(strDate, type);
                Assert.AreEqual(date, value);

                value = NodeValueType.ConvertToPrimitiveType((DateTime?)date, type);
                Assert.AreEqual(date, value);

                value = NodeValueType.ConvertToPrimitiveType(null, type);
                Assert.AreEqual(DateTime.MinValue, value);

                value = NodeValueType.ConvertToPrimitiveType("2001-02-03T04:05:06Z", type);
                value = ((DateTime)value).ToUniversalTime();
                Assert.AreEqual(date, value);
            }


            // string
            {
                var type = typeof(string);
                object value;

                // ##1 string -> string
                value = NodeValueType.ConvertToPrimitiveType("trueasdge", type);
                Assert.AreEqual("trueasdge", value);


                // ##2 bool -> string
                value = NodeValueType.ConvertToPrimitiveType(true, type);
                Assert.AreEqual("true", value);

                value = NodeValueType.ConvertToPrimitiveType(false, type);
                Assert.AreEqual("false", value);

                value = NodeValueType.ConvertToPrimitiveType((bool?)true, type);
                Assert.AreEqual("true", value);

                value = NodeValueType.ConvertToPrimitiveType(null, type);
                Assert.AreEqual(null, value);


                // ##3 double -> string
                value = NodeValueType.ConvertToPrimitiveType(12.1, type);
                Assert.AreEqual("12.1", value);

                value = NodeValueType.ConvertToPrimitiveType(12, type);
                Assert.AreEqual("12", value);


                value = NodeValueType.ConvertToPrimitiveType(null, type);
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

                value = NodeValueType.ConvertValueToType(12.0, type);
                v = value as int?;
                Assert.AreEqual(expectedValue, v);

                value = NodeValueType.ConvertValueToType(12.1, type);
                v = value as int?;
                Assert.AreEqual(expectedValue, v);

                value = NodeValueType.ConvertValueToType(12, type);
                v = value as int?;
                Assert.AreEqual(expectedValue, v);


                value = NodeValueType.ConvertValueToType("12", type);
                v = value as int?;
                Assert.AreEqual(expectedValue, v);

                value = NodeValueType.ConvertValueToType("12.0", type);
                v = value as int?;
                Assert.AreEqual(expectedValue, v);

                value = NodeValueType.ConvertValueToType("12.1", type);
                v = value as int?;
                Assert.AreEqual(expectedValue, v);

                // nullable
                value = NodeValueType.ConvertValueToType(null, type);
                v = value as int?;
                Assert.AreEqual(null, v);

                value = NodeValueType.ConvertValueToType((int?)12, type);
                v = value as int?;
                Assert.AreEqual(expectedValue, v);

                value = NodeValueType.ConvertValueToType((double?)12.0, type);
                v = value as int?;
                Assert.AreEqual(expectedValue, v);

                value = NodeValueType.ConvertValueToType((double?)12.1, type);
                v = value as int?;
                Assert.AreEqual(expectedValue, v);

            }

            // bool
            {
                type = typeof(bool?);
                expectedValue = (bool?)true;

                value = NodeValueType.ConvertValueToType(true, type);
                Assert.AreEqual(expectedValue, value);

                value = NodeValueType.ConvertValueToType("true", type);
                Assert.AreEqual(expectedValue, value);

                value = NodeValueType.ConvertValueToType(null, type);
                Assert.AreEqual(null, value);

                value = NodeValueType.ConvertValueToType("false", type);
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

                var value = NodeValueType.ConvertValueToType(new[] { 12.1 }, type) as int[];
                Assert.AreEqual(expectedValue, value[0]);
            }

            // int? []
            {
                var type = typeof(int?[]);
                int? expectedValue = 12;

                var value = NodeValueType.ConvertValueToType(new[] { 12.1 }, type) as int?[];
                Assert.AreEqual(expectedValue, value[0]);
            }
            #endregion


            #region One-dimensional Collection
            // double[]  ->  List<int>
            {
                var type = typeof(List<int>);
                int expectedValue = 12;

                var value = NodeValueType.ConvertValueToType(new[] { 12.1 }, type) as List<int>;
                Assert.AreEqual(expectedValue, value[0]);
            }
            // IEnumerable<double>  ->  IEnumerable<int>
            {
                var type = typeof(IEnumerable<int>);
                int expectedValue = 12;
                IEnumerable<double> oriValue = new[] { 11.1 }.Select(m => m + 1);

                var value = NodeValueType.ConvertValueToType(oriValue, type) as IEnumerable<int>;
                Assert.AreEqual(expectedValue, value.First());
            }
            // IEnumerable<double>  ->  IQueryable<int>
            {
                var type = typeof(IQueryable<int>);
                int expectedValue = 12;
                IEnumerable<double> oriValue = new[] { 11.1 }.Select(m => m + 1);

                var value = NodeValueType.ConvertValueToType(oriValue, type) as IQueryable<int>;
                Assert.AreEqual(expectedValue, value.First());
            }
            // IQueryable<double>  ->  ICollection<string>
            {
                var type = typeof(ICollection<string>);
                var expectedValue = "12.1";
                IQueryable<double> oriValue = new[] { 11.1 }.AsQueryable().Select(m => m + 1);

                var value = NodeValueType.ConvertValueToType(oriValue, type) as ICollection<string>;
                Assert.AreEqual(expectedValue, value.First());
            }
            #endregion


            #region Two-dimensional
            // double [][] -> int [][]
            {
                var type = typeof(int[][]);
                int expectedValue = 12;

                var value = NodeValueType.ConvertValueToType(new[] { new[] { 12.1 } }, type) as int[][];
                Assert.AreEqual(expectedValue, value[0][0]);
            }
            #endregion
        }




    }
}
