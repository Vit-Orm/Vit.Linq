using System;
using System.Collections.Generic;

namespace Vit.Linq.ExpressionTree.ComponentModel
{

    public class MemberBind
    {
        public string name { get; set; }
        public ExpressionNode value { get; set; }
    }

    /// <summary>
    ///  NewType could be :  Object   Array    List
    /// </summary>
    public interface ExpressionNode_New : IExpressionNode
    {
        ValueType valueType { get; set; }
        List<MemberBind> constructorArgs { get; set; }
        List<MemberBind> memberArgs { get; set; }

        /// <summary>
        /// elements that are used to initialize collection
        /// </summary>
        List<ExpressionNode> initializers { get; set; }

        Type New_GetType();
        ExpressionNode New_SetType(Type type);

        ExpressionNode New_SetConstructorArgTypes(Type[] types);
        Type[] New_GetConstructorArgTypes();

    }

    public partial class ExpressionNode : ExpressionNode_New
    {
        //public ValueType valueType { get; set; }

        public List<MemberBind> constructorArgs { get; set; }
        public List<MemberBind> memberArgs { get; set; }


        /// <summary>
        /// elements that are used to initialize collection
        /// </summary>
        public List<ExpressionNode> initializers { get; set; }

        public static ExpressionNode New(Type type, List<MemberBind> constructorArgs = null, List<MemberBind> memberArgs = null)
        {
            return new ExpressionNode
            {
                nodeType = NodeType.New,
                valueType = ValueType.FromType(type),
                constructorArgs = constructorArgs,
                memberArgs = memberArgs,
            }.New_SetType(type);
        }



        public ExpressionNode New_SetType(Type type) => type == null ? this : SetCodeArg("New_Type", type);
        public Type New_GetType() => GetCodeArg("New_Type") as Type;


        public ExpressionNode New_SetConstructorArgTypes(Type[] types) => SetCodeArg("New_ConstructorArgTypes", types);
        public Type[] New_GetConstructorArgTypes() => GetCodeArg("New_ConstructorArgTypes") as Type[];

    }

}
