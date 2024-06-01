﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Vit.Linq.ExpressionTree.ComponentModel;
using Vit.Linq.ExpressionTree.ExpressionConvertor;

namespace Vit.Linq.ExpressionTree
{
    public enum EValueType
    {
        /// <summary>
        /// constant value like 1.5 or int[]{1,2,3}
        /// </summary>
        constant,
        /// <summary>
        /// QueryableArgument
        /// </summary>
        argument,
        /// <summary>
        /// like parameter from lambda argument  or combined value
        /// </summary>
        other,
    }
    public class DataConvertArgument
    {
        public bool autoReduce { get; set; } = false;

        public Func<ConstantExpression, bool> isArgument { get; set; }

        public virtual bool IsArgument(ConstantExpression constant)
        {
            if (isArgument != null)
                return isArgument(constant);

            var type = constant.Type;
            //var value = constant.Value;
            if (!type.IsArray && type.IsGenericType && typeof(IQueryable).IsAssignableFrom(type))
            {
                //if (typeof(List<>) == type.GetGenericTypeDefinition())
                //    return false;

                return true;
            }
            return false;
        }

        /// <summary>
        /// TODO: use object hashcode to cache EValueType
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected EValueType GetEValueType(Expression expression)
        {
            List<EValueType> childrenTypes;
            switch (expression)
            {
                case null: return EValueType.constant;

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


        public bool ReduceValue<T>(Expression expression, out T value)
        {
            try
            {
                if (autoReduce && CanCalculateToConstant(expression))
                {
                    value = (T)InvokeExpression(expression);
                    return true;
                }
            }
            catch (Exception ex)
            {
            }
            value = default;
            return false;
        }

        public static object InvokeExpression(Expression expression)
        {
            return Expression.Lambda(expression).Compile().DynamicInvoke();
        }

        public bool CanCalculateToConstant(Expression expression)
        {
            return GetEValueType(expression) == EValueType.constant;
        }

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

        internal List<ParamterInfo> globalParameters { get; private set; }


        public ExpressionNode CreateParameter(object value, Type type)
        {
            ParamterInfo parameter;

            parameter = globalParameters?.FirstOrDefault(p => p.value?.GetHashCode() == value.GetHashCode());

            if (parameter == null)
            {
                if (globalParameters == null) globalParameters = new List<ParamterInfo>();

                parameter = new ParamterInfo(value: value, type: type);
                globalParameters.Add(parameter);
            }
            return ExpressionNode_FreeParameter.Member(parameter);
        }




    }



}
