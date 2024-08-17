using System.Linq.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Vit.Core.Module.Serialization;
using Vit.Linq.ExpressionNodes.ComponentModel;
using Vit.Linq.ExpressionNodes.ExpressionConvertor;
using Vit.Linq.ExpressionNodes.ExpressionConvertor.MethodCalls;

namespace Vit.Linq.ExpressionNodes.MsTest.CustomMethod
{


    [TestClass]
    public class CustomMethod_Test
    {

        [TestMethod]
        public void Test_CustomMethod()
        {
            {
                var convertService = ExpressionConvertService.Instance;


                Expression<Func<int, int>> addMethod = x => x.Add(5);

                var node = convertService.ConvertToData_LambdaNode(addMethod);
                //{"nodeType":"Lambda","body":{"nodeType":"Lambda","parameterNames":["x"],"body":{"nodeType":"MethodCall","methodCall_typeName":"CustomMethod_Test_Extensions","methodName":"Add","arguments":[{"nodeType":"Member","parameterName":"x"},{"nodeType":"Constant","valueType":{"typeName":"Int32"},"value":5}]}}}
                var str = Json.Serialize(node);

                ExpressionNode_MethodCall addLambda = node.body.body;
                Assert.AreEqual(NodeType.MethodCall, addLambda.nodeType);
                Assert.AreEqual(nameof(CustomMethod_Test_Extensions.Add), addLambda.methodName);
                Assert.AreEqual(5, addLambda.arguments[1].value);
            }

        }


        [TestMethod]
        public void Test_MyCustomMethod()
        {
            {
                var convertService = ExpressionConvertService.Instance;

                Expression<Func<int, int>> addMethod = x => x.MyAdd(5);

                var node = convertService.ConvertToData_LambdaNode(addMethod);
                var str = Json.Serialize(node);

                ExpressionNode_MethodCall addMethodCall = node.body.body;
                Assert.AreEqual(NodeType.MethodCall, addMethodCall.nodeType);
                Assert.AreEqual(nameof(CustomMethod_Test_Extensions.MyAdd), addMethodCall.methodName);
                Assert.AreEqual(5 + 10, addMethodCall.arguments[1].value);
            }

        }

    }


    public static partial class CustomMethod_Test_Extensions
    {
        [ExpressionNode_CustomMethod]
        public static int Add(this int data, int value) => throw new NotImplementedException();

        [MyCustomMethod(offset = 10)]
        public static int MyAdd(this int data, int value) => throw new NotImplementedException();
    }



    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class MyCustomMethodAttribute : ExpressionNode_CustomMethodAttribute
    {
        public int offset { get; set; }

        public override ExpressionNode ToData(ToDataArgument arg, MethodCallExpression call)
        {
            var node = MethodCall.ConvertToData(arg, call);
            ExpressionNode_MethodCall nodeMethodCall = node;

            nodeMethodCall.arguments[1].value = (int)nodeMethodCall.arguments[1].value + offset;

            return node;
        }
    }


}
