using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Vit.Linq.QueryBuilder
{
    public partial class LinqHelp
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="propertyOrFieldName"> could be array index, example:  "2"  "name"</param>
        /// <returns></returns>
        public static Expression GetFieldMemberExpression_ByName(Expression parameter, string propertyOrFieldName)
        {
            var valueType = parameter.Type;
            int index;
            if (valueType.IsArray)
            {
                // Array
                if (int.TryParse(propertyOrFieldName, out index))
                    return Expression.ArrayAccess(parameter, Expression.Constant(index));
            }
            else
            if (valueType.IsGenericType && typeof(IEnumerable).IsAssignableFrom(valueType))
            {
                // IEnumerable<>    List<>
                if (int.TryParse(propertyOrFieldName, out index))
                    return Expression.Call(typeof(Enumerable), "ElementAt", valueType.GetGenericArguments(), parameter, Expression.Constant(index));
            }

            return Expression.PropertyOrField(parameter, propertyOrFieldName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="fieldPath"> could be nasted , example: "name"  "depart.name"  "departs[1].name" "departs.1.name"</param>
        /// <returns></returns>
        public static Expression GetFieldMemberExpression(Expression parameter, string fieldPath)
        {
            fieldPath = fieldPath?.Replace("]", "").Replace("[", ".");
            if (!string.IsNullOrWhiteSpace(fieldPath))
            {
                foreach (var fieldName in fieldPath.Split('.'))
                {
                    parameter = GetFieldMemberExpression_ByName(parameter, fieldName);
                }
            }
            return parameter;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fieldPath"> could be nasted , example: "name"  "depart.name"</param>
        /// <returns></returns>
        public static Expression GetFieldMemberExpression(Type type, string fieldPath)
        {
            return GetFieldMemberExpression(Expression.Parameter(type), fieldPath);
        }



        public static Expression<Func<T, object>> GetFieldExpression<T>(string fieldPath)
        {
            var parammeter = Expression.Parameter(typeof(T));
            Expression memberExp = GetFieldMemberExpression(parammeter, fieldPath);
            var lambda = Expression.Lambda(memberExp, parammeter).Compile();
            return t => lambda.DynamicInvoke(t);
        }




    }
}
