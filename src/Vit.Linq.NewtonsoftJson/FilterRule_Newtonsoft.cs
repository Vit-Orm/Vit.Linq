using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Vit.Linq.MoreFilter;

namespace Vit.Linq.NewtonsoftJson
{
    /// <summary>
    /// This class is used to define a hierarchical filter for a given collection. This type can be serialized/deserialized by JSON.NET without needing to modify the data structure from QueryBuilder.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FilterRule_Newtonsoft : FilterRuleWithMethod<FilterRule_Newtonsoft>
    {

        protected override object GetPrimitiveValue(Object value)
        {
            if (value is JToken item)
            {
                return GetPrimitiveValueFromJson(item);
            }
            return value;
        }




        public static FilterRule_Newtonsoft FromString(string filter)
        {
            return JsonConvert.DeserializeObject<FilterRule_Newtonsoft>(filter, serializeSetting);
        }

        static readonly global::Newtonsoft.Json.JsonSerializerSettings serializeSetting = new global::Newtonsoft.Json.JsonSerializerSettings() { Converters = { new ValueConverter() } };


        public static object GetPrimitiveValueFromJson(JToken value)
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
                return GetPrimitiveValueFromJson(token);
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
    }
}
