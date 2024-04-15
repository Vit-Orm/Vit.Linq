using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vit.Linq.ExpressionTree.ComponentModel
{
    public partial class ValueType
    {
        /// <summary>
        /// valueType:      String | Int32 | Int64 | Single | Double | Boolean | ...
        /// Object:         Object
        /// List:           List<String>
        /// Array:          String[]
        /// List:           List<String>
        /// Nullable:       int?
        /// </summary>
        public string typeName { get; set; }


        public ValueType[] genericArgumentTypes { get; set; }


        #region FromType
        public static ValueType FromType(Type type)
        {
            if (type == null) return null;
            if (type == typeof(string))
                return new ValueType { typeName = "String" };

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
                return new ValueType { typeName = typeName };
            }

            if (type.IsArray)
            {
                var elementType = type.GetElementType();
                var genericArgumentTypes = new[] { FromType(elementType) };
                return new ValueType { typeName = "Array", genericArgumentTypes = genericArgumentTypes };
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
                        return null;
                    }
                default: return Type.GetType("System." + typeName);
            }
        }
        #endregion


    }
}
