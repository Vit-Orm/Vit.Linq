using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Vit.Linq
{
    public partial class LinqHelp
    {

        #region GetMemberType

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberPath"> could be nested , example: "name"  "depart.name"  "departs[1].name" "departs.1.name"</param>
        /// <returns></returns>
        public static Type GetNestedMemberType(Type type, string memberPath)
        {
            if (string.IsNullOrWhiteSpace(memberPath)) return type;

            memberPath = memberPath.Replace("]", "").Replace("[", ".");
            foreach (var memberName in memberPath.Split('.'))
            {
                if (type.IsArray && int.TryParse(memberName, out _))
                {
                    type = type.GetElementType();
                }
                else if (type.IsGenericType && typeof(IEnumerable).IsAssignableFrom(type) && int.TryParse(memberName, out _))
                {
                    type = type.GetGenericArguments()[0];
                }
                else
                    type = GetMemberType(type, memberName);
            }
            return type;
        }
        public static Type GetMemberType(Type type, string memberName)
        {
            if (type == null) return null;

            //#1 property
            var property = type.GetProperty(memberName, BindingFlags.Public | BindingFlags.Instance);
            if (property != null)
            {
                return property.PropertyType;
            }

            //#2 field
            var field = type.GetField(memberName, BindingFlags.Public | BindingFlags.Instance);
            if (field != null)
            {
                return field.FieldType;
            }
            return null;
        }

        #endregion



        public static Func<T, object> GetFieldSelector_ByReflection<T>(Func<T, object> before, string fieldName)
        {
            return (T ori) =>
            {

                var midValue = before == null ? ori : before(ori);
                if (midValue == null) return null;

                var midType = midValue.GetType();


                //#1 Array or List
                if (midType.IsGenericType && typeof(IEnumerable).IsAssignableFrom(midType) && int.TryParse(fieldName, out var index))
                {
                    var elemType = midType.GetGenericArguments()[0];

                    var methodInfo = new Func<IEnumerable<object>, int, object>(Enumerable.ElementAt).GetMethodInfo().GetGenericMethodDefinition().MakeGenericMethod(elemType);

                    return methodInfo.Invoke(midValue, new object[] { index });
                }

                //#2 property
                var property = midType.GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance);
                if (property != null && property.CanRead)
                {
                    return property.GetValue(midValue);
                }

                //#3 field
                var field = midType.GetField(fieldName, BindingFlags.Public | BindingFlags.Instance);
                if (field != null)
                {
                    return field.GetValue(midValue);
                }

                //#4 null
                return null;
            };
        }

        public static Func<T, object> GetFieldSelector_ByReflection<T>(string fieldPath)
        {
            Func<T, object> getField = null;
            foreach (var fieldName in fieldPath?.Replace("]", "").Replace("[", ".").Split('.'))
            {
                getField = GetFieldSelector_ByReflection(getField, fieldName);
            }
            return getField;
        }


        public static Expression<Func<T, object>> GetFieldExpression_ByReflection<T>(string fieldPath)
        {
            Func<T, object> getField = GetFieldSelector_ByReflection<T>(fieldPath);
            return t => getField(t);
        }


    }
}
