using System;
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
                if (autoReduce)
                {
                    var del = Expression.Lambda(expression).Compile();
                    value = (T)del.DynamicInvoke();
                    return true;
                }
            }
            catch (Exception ex)
            {
            }
            value = default;
            return false;
        }

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


        public ExpressionNode GetParameter(object value, Type type)
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
