using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Vit.Linq.ExpressionTree.ComponentModel
{

    public partial class ValueType
    {
        /// <summary>
        ///   ValueType:      String | Int32 | Int64 | Single | Double | Boolean | DateTime | ...
        ///   Nullable:       int?
        ///   Object:         Object
        ///   List:           List<String>
        ///   Array:          String[]
        ///   Queryable:      IQueryable<string>
        ///   Enumerable:     IEnumerable<string>
        ///   Collection:     ICollection<string>
        /// </summary>
        public string typeName { get; set; }

        public ValueType[] genericArgumentTypes { get; set; }


        #region FromType
        public static ValueType FromType(Type type)
        {
            if (type == null) return null;
            if (type == typeof(string))
            {
                return new ValueType
                {
                    typeName = "String"
                };
            }
            if (type == typeof(object))
            {
                return new ValueType
                {
                    typeName = "Object"
                };
            }

            if (type.IsValueType)
            {
                if (type.IsGenericType && typeof(Nullable<>) == type.GetGenericTypeDefinition())
                {
                    var elementType = type.GetGenericArguments()[0];
                    var genericArgumentTypes = new[] { FromType(elementType) };
                    return new ValueType
                    {
                        typeName = "Nullable",
                        genericArgumentTypes = genericArgumentTypes
                    };
                }

                var typeName = type.Name;
                return new ValueType
                {
                    typeName = typeName
                };
            }

            if (type.IsArray)
            {
                var elementType = type.GetElementType();
                var genericArgumentTypes = new[] { FromType(elementType) };
                return new ValueType
                {
                    typeName = "Array",
                    genericArgumentTypes = genericArgumentTypes
                };
            }

            if (type.IsGenericType)
            {
                string typeName = null;

                if (typeof(IList).IsAssignableFrom(type))
                {
                    typeName = "List";
                }
                else if (typeof(IQueryable).IsAssignableFrom(type))
                {
                    typeName = "Queryable";
                }
                else if (typeof(IEnumerable).IsAssignableFrom(type))
                {
                    typeName = "Enumerable";
                }
                else if (typeof(ICollection).IsAssignableFrom(type))
                {
                    typeName = "Collection";
                }

                if (typeName != null)
                {
                    var elementType = type.GetGenericArguments()[0];
                    var genericArgumentTypes = new[] { FromType(elementType) };
                    return new ValueType
                    {
                        typeName = typeName,
                        genericArgumentTypes = genericArgumentTypes
                    };
                }
            }
            return null;
        }
        #endregion



        #region ConvertToType
        public static object ConvertToType(object oriValue, Type targetType)
        {
            if (oriValue != null && targetType.IsAssignableFrom(oriValue.GetType()))
            {
                return oriValue;
            }

            if (targetType.IsValueType || targetType == typeof(string))
            {
                // #1 Nullable
                if (targetType.IsGenericType && typeof(Nullable<>) == targetType.GetGenericTypeDefinition())
                {
                    if (oriValue == null) return null;
                    var elementType = targetType.GetGenericArguments()[0];
                    return ConvertToPrimitiveType(oriValue, elementType);
                }

                // #2 valueType  and string and DateTime
                return ConvertToPrimitiveType(oriValue, targetType);
            }

            if (oriValue == null) return null;

            // #3 collection  include(Array,List,Queryable,Enumerable)
            if (targetType.IsArray)
            {
                var elementType = targetType.GetElementType();
                return ConvertToList(oriValue, targetType, elementType);
            }

            if (targetType.IsGenericType)
            {
                if (typeof(IList).IsAssignableFrom(targetType)
                    || typeof(IQueryable).IsAssignableFrom(targetType)
                    || typeof(IEnumerable).IsAssignableFrom(targetType)
                    || typeof(ICollection).IsAssignableFrom(targetType)
                )
                {
                    var elementType = targetType.GetGenericArguments()[0];
                    return ConvertToList(oriValue, targetType, elementType);
                }
            }

            // #4 other
            throw new ArgumentException($"can not convert value({oriValue}) to type({targetType.Name}) from type({oriValue?.GetType().Name})");
        }
        static object ConvertToList(object items, Type collectionType, Type elemType)
        {
            return new Func<object, Type, object>(ConvertToList<string>)
                   .Method.GetGenericMethodDefinition().MakeGenericMethod(elemType)
               .Invoke(null, new[] { items, collectionType });
        }
        static object ConvertToList<T>(object items, Type collectionType)
        {
            var type = typeof(T);
            List<T> list = new List<T>();
            if (items is IEnumerable enumerable)
            {
                foreach (var item in enumerable)
                {
                    list.Add((T)ConvertToType(item, type));
                }
            }
            if (collectionType.IsArray)
                return list.ToArray();

            if (typeof(IQueryable).IsAssignableFrom(collectionType))
                return list.AsQueryable();

            return list;
        }

        /// <summary>
        /// targetType must be ValueType or string or DateTime
        /// </summary>
        /// <param name="oriValue"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static object ConvertToPrimitiveType(object oriValue, Type targetType)
        {
            if (oriValue == null)
            {
                if (targetType == typeof(string)) return null;
                return Activator.CreateInstance(targetType);
            }

            if (oriValue.GetType() == targetType) return oriValue;

            var typeName = targetType.Name.ToLower();

            // double -> int         "12.1"  12.1  ->  12
            if (typeName.Contains("int") || typeName.Contains("byte"))
            {
                oriValue = ((decimal)Convert.ChangeType(oriValue, typeof(decimal))).ToString("#");
            }
            // bool -> string       true/false  -> "true" / "false"
            else if (targetType == typeof(string) && oriValue is bool b)
            {
                return b ? "true" : "false";
            }

            return Convert.ChangeType(oriValue, targetType);
        }
        #endregion




        #region ToType
        public Type ToType()
        {
            if (string.IsNullOrWhiteSpace(typeName)) return null;

            switch (typeName)
            {
                case "Nullable":
                    {
                        var baseType = genericArgumentTypes[0].ToType();
                        return typeof(Nullable<>).MakeGenericType(baseType);
                    }
                case "Array":
                    {
                        var baseType = genericArgumentTypes[0].ToType();
                        return baseType.MakeArrayType();
                    }
                case "List":
                    {
                        var baseType = genericArgumentTypes[0].ToType();
                        return typeof(List<string>).GetGenericTypeDefinition().MakeGenericType(baseType);
                    }
                case "Queryable":
                    {
                        var baseType = genericArgumentTypes[0].ToType();
                        return typeof(IQueryable<string>).GetGenericTypeDefinition().MakeGenericType(baseType);
                    }
                case "Enumerable":
                    {
                        var baseType = genericArgumentTypes[0].ToType();
                        return typeof(IEnumerable<string>).GetGenericTypeDefinition().MakeGenericType(baseType);
                    }
                case "Collection":
                    {
                        var baseType = genericArgumentTypes[0].ToType();
                        return typeof(ICollection<string>).GetGenericTypeDefinition().MakeGenericType(baseType);
                    }
                case "Object":
                    {
                        return typeof(object);
                    }
                default: return Type.GetType("System." + typeName);
            }
        }

        #endregion
    }

}
