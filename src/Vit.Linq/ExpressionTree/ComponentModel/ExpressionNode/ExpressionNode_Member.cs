using System;

namespace Vit.Linq.ExpressionTree.ComponentModel
{

    public interface ExpressionNode_Member : IExpressionNode
    {
        public string parameterName { get;   }
        public ExpressionNode objectValue { get;  }

        public string memberName { get; }

        ExpressionNode Member_SetType(Type type);
        Type Member_GetType();
    }

    public partial class ExpressionNode : ExpressionNode_Member
    {
        public virtual string parameterName { get; set; }

        public ExpressionNode objectValue { get; set; }

        public string memberName { get; set; }


        public static ExpressionNode Member(ExpressionNode objectValue, string memberName, string parameterName)
        {
            return new ExpressionNode
            {
                nodeType = NodeType.Member,
                objectValue = objectValue,
                memberName = memberName,
                parameterName = parameterName,
            };
        }

        public static ExpressionNode Member(ExpressionNode objectValue, string memberName)
        {
            return new ExpressionNode
            {
                nodeType = NodeType.Member,
                objectValue = objectValue,
                memberName = memberName,
            };
        }
        public static ExpressionNode Member(string parameterName, string memberName)
        {
            return new ExpressionNode
            {
                nodeType = NodeType.Member,
                parameterName = parameterName,
                memberName = memberName,
            };
        }

        public ExpressionNode Member_SetType(Type type)
        {
            SetCodeArg("Member_Type", type);
            return this;
        }
        public Type Member_GetType()
        {
            return GetCodeArg("Member_Type") as Type;
        }
    }

    public class ParamterInfo
    {
        internal ParamterInfo(object value, Type type)
        {
            this.value = value;
            this.type = type;
        }

        public object value { get; private set; }
        public Type type { get; private set; }
        //public int hashCode => value?.GetHashCode() ?? 0;
        public String parameterName { get; private set; }

        public void Rename(String parameterName) => this.parameterName = parameterName;
    }
    public class ExpressionNode_FreeParameter : ExpressionNode
    {
        protected ParamterInfo parameter;

        public override string parameterName
        {
            set => throw new NotSupportedException();
            get => parameter?.parameterName;
        }

        public static ExpressionNode Member(ParamterInfo parameter)
        {
            var node = new ExpressionNode_FreeParameter
            {
                nodeType = NodeType.Member,
                parameter = parameter,
            };
            node.Member_SetOriValue(parameter?.value);
            return node;
        }
    }

    public static partial class ExpressionNode_Extensions
    {
        public static object Member_GetOriValue(this ExpressionNode_Member member)
        {
            return member.GetCodeArg("Member_OriValue");
        }
        public static void Member_SetOriValue(this ExpressionNode_Member member, object oriGalue)
        {
            member.SetCodeArg("Member_OriValue", oriGalue);
        }
    }

}
