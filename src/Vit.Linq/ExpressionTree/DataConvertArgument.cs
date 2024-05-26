using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Vit.Linq.ExpressionTree.ComponentModel;

namespace Vit.Linq.ExpressionTree
{

    public class DataConvertArgument
    {
        public bool autoReduce { get; set; } = false;


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

        public static bool CanCalculateToConstant(Expression expression) 
        {
            switch (expression)
            {
                case null: return true;
                case MemberExpression member:
                    {
                        return CanCalculateToConstant(member.Expression);
                    }
                case UnaryExpression unary:
                    {
                        //switch (unary.NodeType)
                        //{
                        //    case ExpressionType.Convert:
                        //    case ExpressionType.Quote:
                        //        return CouldExecuteToConst(unary.Operand);
                        //}
                        return CanCalculateToConstant(unary.Operand);
                    }
                case BinaryExpression binary:
                    {
                        return CanCalculateToConstant(binary.Left)&& CanCalculateToConstant(binary.Right);
                    }
                case ConstantExpression constant:
                    {
                        var type = expression.Type;
                        var value = constant.Value;
                        if (value == null) return true;
                        if (IsQueryableArgument(type)) return false;
                        return true;
                    }
                case NewArrayExpression newArray:
                    {
                        return newArray.Expressions?.All(exp => CanCalculateToConstant(exp)) != false;
                    }
                case ListInitExpression listInit:
                    {
                        return listInit.Initializers?.All(exp => CanCalculateToConstant(exp.Arguments[0])) != false;
                    }
            }
            return false;
        }

        #region Type

        public static bool IsQueryableArgument(Type type)
        {
            if (!type.IsArray && type.IsGenericType && typeof(IQueryable).IsAssignableFrom(type))
            {
                //if (typeof(List<>) == type.GetGenericTypeDefinition())
                //    return false;

                return true;
            }

            return false;

        }
        public static bool IsTransportableType(Type type)
        {
            if (IsBasicType(type)) return true;

            if (type.IsArray && IsTransportableType(type.GetElementType()))
            {
                return true;
            }

            if (type.IsGenericType)
            {
                if (type.GetGenericArguments().Any(t => !IsTransportableType(t))) return false;

                if (typeof(IList).IsAssignableFrom(type)
                    || typeof(ICollection).IsAssignableFrom(type)
                    )
                    return true;
            }

            return false;
        }


        // is valueType of Nullable 
        public static bool IsBasicType(Type type)
        {
            return
                type.IsEnum || // enum
                type == typeof(string) || // string
                type.IsValueType ||  //int
                (type.IsGenericType && typeof(Nullable<>) == type.GetGenericTypeDefinition()); // int?
        }


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
