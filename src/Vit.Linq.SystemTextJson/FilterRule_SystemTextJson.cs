using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using System.Text.Unicode;

namespace Vit.Linq.QueryBuilder.SystemTextJson
{
    /// <summary>
    /// This class is used to define a hierarchical filter for a given collection. This type can be serialized/deserialized by JSON.NET without needing to modify the data structure from QueryBuilder.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FilterRule_SystemTextJson : FilterRuleBase<FilterRule_SystemTextJson>
    {
        /// <summary>
        /// Gets or sets the value of the filter.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public override object value
        {
            get
            {
                if (_value is JsonElement je)
                {
                    return GetPrimitiveValue(je);
                }
                return _value;
            }
            set => _value = value;
        }

        private object _value;


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

            return options;
        }



        public static object GetPrimitiveValue(JsonElement value)
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
                    return value.EnumerateArray().Select(GetPrimitiveValue).ToList();
            }

            return value;
        }



    }
}
