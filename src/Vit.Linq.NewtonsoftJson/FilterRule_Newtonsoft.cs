using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Vit.Linq.QueryBuilder.NewtonsoftJson
{
    /// <summary>
    /// This class is used to define a hierarchical filter for a given collection. This type can be serialized/deserialized by JSON.NET without needing to modify the data structure from QueryBuilder.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FilterRule_Newtonsoft : IFilterRule
    {
        /// <summary>
        /// condition - acceptable values are "and" and "or".
        /// </summary>
        public string condition { get; set; }


        public string field { get; set; }


        public string @operator { get; set; }

        /// <summary>
        ///  nested filter rules.
        /// </summary>
        public List<FilterRule_Newtonsoft> rules { get; set; }


        /// <summary>
        /// Gets or sets the value of the filter.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public object value
        {
            get
            {
                if (_value is JToken jt)
                {
                    return GetPrimitiveValue(jt);
                }
                return _value;
            }
            set => _value = value;
        }

        private object _value;

        IEnumerable<IFilterRule> IFilterRule.rules => rules;


        public static FilterRule_Newtonsoft FromString(string filter)
        {
            return JsonConvert.DeserializeObject<FilterRule_Newtonsoft>(filter, serializeSetting);
        }

        static readonly global::Newtonsoft.Json.JsonSerializerSettings serializeSetting = new global::Newtonsoft.Json.JsonSerializerSettings() { Converters = { new ValueConverter() } };


        public static object GetPrimitiveValue(JToken value)
        {
            if (value is JValue jv)
            {
                return jv.Value;
            }

            if (value is JArray ja)
            {
                List<object> values = new List<object>();
                foreach (JValue item in ja)
                {
                    values.Add(item.Value);
                }
                return values;
            }
            return value;
        }


        class ValueConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(object);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                JToken token = JToken.Load(reader);
                return GetPrimitiveValue(token);
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
    }
}
