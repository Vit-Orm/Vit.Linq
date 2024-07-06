using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Vit.Linq.ExpressionTree.ComponentModel;

using ParameterInfo = Vit.Linq.ExpressionTree.ComponentModel.ParameterInfo;

namespace Vit.Linq.ExpressionTree
{
    public partial class DataConvertArgument
    {


        public Func<object, Type, bool> isArgument { get; set; }


        public virtual bool IsArgument(ConstantExpression constant)
        {
            return IsArgument(constant.Value, constant.Type);
        }

        public virtual bool IsArgument(object value, Type type)
        {
            if (isArgument != null)
                return isArgument(value, type);


            if (!type.IsArray && type.IsGenericType && typeof(IQueryable).IsAssignableFrom(type))
            {
                return true;
            }
            return false;
        }

        #region GetEValueType

        private readonly Dictionary<int, EValueType> eValueTypeCache = new Dictionary<int, EValueType>();
        protected EValueType GetEValueType(Expression expression)
        {
            //if (expression == null) return EValueType.constant;

            var hashCode = expression.GetHashCode();
            if (eValueTypeCache.TryGetValue(hashCode, out var type)) return type;

            type = GetEValueType_Directly(expression);
            eValueTypeCache[hashCode] = type;

            return type;
        }

        protected EValueType GetEValueType_Directly(Expression expression)
        {

            List<EValueType> childrenTypes;
            switch (expression)
            {
                // case null: return EValueType.constant;

                case ConstantExpression constant:
                    {
                        if (IsArgument(constant)) return EValueType.argument;
                        return EValueType.constant;
                    }
                case MemberExpression member:
                    {
                        return GetEValueType(member.Expression);
                    }
                case UnaryExpression unary:
                    {
                        return GetEValueType(unary.Operand) == EValueType.constant ? EValueType.constant : EValueType.other;
                    }
                case BinaryExpression binary:
                    {
                        childrenTypes = new List<EValueType> { GetEValueType(binary.Left), GetEValueType(binary.Right) };
                        break;
                    }
                case NewExpression newExp:
                    {
                        childrenTypes = newExp.Arguments?.Select(GetEValueType).ToList();
                        break;
                    }
                case NewArrayExpression newArray:
                    {
                        childrenTypes = newArray.Expressions?.Select(GetEValueType).ToList();
                        break;
                    }
                case ListInitExpression listInit:
                    {
                        childrenTypes = listInit.Initializers?.Select(exp => GetEValueType(exp.Arguments[0])).ToList();
                        break;
                    }
                case MethodCallExpression call:
                    {
                        // get ValueType from ValueTypeAttribute
                        {
                            if (call.Method.GetCustomAttributes(typeof(ValueTypeAttribute), inherit: true).FirstOrDefault() is ValueTypeAttribute attribute) return attribute.valueType;

                            attribute = call.Method.DeclaringType.GetCustomAttributes(typeof(ValueTypeAttribute), inherit: true).FirstOrDefault() as ValueTypeAttribute;
                            if (attribute != null) return attribute.valueType;
                        }

                        childrenTypes = new();
                        if (call.Arguments?.Any() == true) childrenTypes.AddRange(call.Arguments.Select(GetEValueType));
                        if (call.Object != null) childrenTypes.Add(GetEValueType(call.Object));
                        break;
                    }
                default: return EValueType.other;
            }
            if (childrenTypes?.Any() != true) return EValueType.constant;

            if (childrenTypes.All(m => m == EValueType.constant)) return EValueType.constant;
            return EValueType.other;
        }

        #endregion



        #region Type


        //public static bool IsTransportableType(Type type)
        //{
        //    if (IsBasicType(type)) return true;

        //    if (type.IsArray && IsTransportableType(type.GetElementType()))
        //    {
        //        return true;
        //    }

        //    if (type.IsGenericType)
        //    {
        //        if (type.GetGenericArguments().Any(t => !IsTransportableType(t))) return false;

        //        if (typeof(IList).IsAssignableFrom(type)
        //            || typeof(ICollection).IsAssignableFrom(type)
        //            )
        //            return true;
        //    }

        //    return false;
        //}


        //// is valueType of Nullable 
        //public static bool IsBasicType(Type type)
        //{
        //    return
        //        type.IsEnum || // enum
        //        type == typeof(string) || // string
        //        type.IsValueType ||  //int
        //        (type.IsGenericType && typeof(Nullable<>) == type.GetGenericTypeDefinition()); // int?
        //}


        #endregion



        public ExpressionConvertService convertService { get; set; }

        private readonly List<string> usedParameterNames = new List<string>();

        public void RegisterParameterNames(IEnumerable<string> names)
        {
            usedParameterNames.AddRange(names);
        }

        public void GenerateGlobalParameterName()
        {
            #region GetUnusedParameterName
            int i = 0;
            string GetUnusedParameterName()
            {
                for (; ; i++)
                {
                    var parameterName = "Param_" + i;
                    if (!usedParameterNames.Contains(parameterName))
                    {
                        usedParameterNames.Add(parameterName);
                        return parameterName;
                    }
                }
            }
            #endregion

            globalParameters?.ForEach(p =>
            {
                if (string.IsNullOrWhiteSpace(p.parameterName))
                {
                    p.Rename(GetUnusedParameterName());
                }
            });

        }

        internal List<ParameterInfo> globalParameters { get; private set; }


        public ExpressionNode CreateParameter(object value, Type type)
        {
            ParameterInfo parameter;

            parameter = globalParameters?.FirstOrDefault(p => p.value?.GetHashCode() == value.GetHashCode());

            if (parameter == null)
            {
                globalParameters ??= new List<ParameterInfo>();

                parameter = new ParameterInfo(value: value, type: type);
                globalParameters.Add(parameter);
            }
            return ExpressionNode_FreeParameter.Member(parameter);
        }




    }



}
