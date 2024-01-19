using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

using Vit.Linq.MoreFilter;

namespace Vit.Linq.SystemTextJson
{
    /// <summary>
    /// This class is used to define a hierarchical filter for a given collection. This type can be serialized/deserialized by JSON.NET without needing to modify the data structure from QueryBuilder.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FilterRule_SystemTextJson : FilterRuleWithMethod<FilterRule_SystemTextJson>
    {
        protected override object GetPrimitiveValue(Object value)
        {
            if (value is JsonElement je)
            {
                return GetPrimitiveValueFromJson(je);
            }
            return value;
        }

        public static FilterRule_SystemTextJson FromString(string filter)
        {
            return JsonSerializer.Deserialize<FilterRule_SystemTextJson>(filter, options);
        }

        static readonly JsonSerializerOptions options = GetDefaultOptions();


        public static JsonSerializerOptions GetDefaultOptions()
        {
            var options = new JsonSerializerOptions
            {
                // avoid transfer chinese character, for example {"title":"\u4ee3\u7801\u6539\u53d8\u4e16\u754c"}
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(UnicodeRanges.All),
                IncludeFields = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };

            options.Converters.Add(new CustomObjectConverter());

            return options;
        }




        public static object GetPrimitiveValueFromJson(JsonElement value)
        {
            switch (value.ValueKind)
            {
                case JsonValueKind.Null: return null;
                case JsonValueKind.Undefined: return null;
                case JsonValueKind.True: return true;
                case JsonValueKind.False: return false;
                case JsonValueKind.Number: return value.GetDecimal();
                case JsonValueKind.String: return value.GetString();
                case JsonValueKind.Array:
                    return value.EnumerateArray().Select(GetPrimitiveValueFromJson).ToList();
            }

            return value;
        }


        class CustomObjectConverter : JsonConverter<object>
        {
            public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.Number:
                        if (reader.TryGetInt32(out int intValue))
                        {
                            return intValue;
                        }
                        if (reader.TryGetDouble(out double doubleValue))
                        {
                            return doubleValue;
                        }
                        break;
                    case JsonTokenType.True:
                        return true;
                    case JsonTokenType.False:
                        return false;
                    case JsonTokenType.String:
                        return reader.GetString();
                    case JsonTokenType.Null:
                        return null;
                }
                using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
                {
                    return doc.RootElement.Clone();
                }
            }

            public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
            {
                JsonSerializer.Serialize(writer, value, value.GetType(), options);
            }
        }


    }
}
