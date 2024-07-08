using System;
using System.Collections.Generic;

namespace Vit.Linq.ExpressionTree.ComponentModel
{

    public class MemberBind
    {
        public string name { get; set; }
        public ExpressionNode value { get; set; }
    }


    public interface ExpressionNode_New : IExpressionNode
    {
        List<MemberBind> constructorArgs { get; set; }
        List<MemberBind> memberArgs { get; set; }

        Type New_GetType();
        ExpressionNode New_SetType(Type type);

        ExpressionNode New_SetConstructorArgTypes(Type[] types);
        Type[] New_GetConstructorArgTypes();

    }

    public partial class ExpressionNode : ExpressionNode_New
    {
        public List<MemberBind> constructorArgs { get; set; }
        public List<MemberBind> memberArgs { get; set; }

        public static ExpressionNode New(List<MemberBind> constructorArgs = null, List<MemberBind> memberArgs = null)
        {
            return new ExpressionNode
            {
                nodeType = NodeType.New,
                constructorArgs = constructorArgs,
                memberArgs = memberArgs,
            };
        }



        public ExpressionNode New_SetType(Type type) => SetCodeArg("New_Type", type);
        public Type New_GetType() => GetCodeArg("New_Type") as Type;


        public ExpressionNode New_SetConstructorArgTypes(Type[] types) => SetCodeArg("New_ConstructorArgTypes", types);
        public Type[] New_GetConstructorArgTypes() => GetCodeArg("New_ConstructorArgTypes") as Type[];

    }

}
