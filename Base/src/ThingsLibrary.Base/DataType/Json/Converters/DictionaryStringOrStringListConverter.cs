using System.Collections.Generic;

namespace ThingsLibrary.Base.DataType.Json.Converters
{
    public class DictionaryStringOrStringListConverter : JsonConverter<Dictionary<string, object>>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(Dictionary<string, object>);
        }

        public override Dictionary<string, object> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException($"JsonTokenType was of type {reader.TokenType}, only objects are supported");
            }

            var dictionary = new Dictionary<string, object>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject) { return dictionary; }
                if (reader.TokenType != JsonTokenType.PropertyName) { throw new JsonException("JsonTokenType was not PropertyName"); }

                var propertyName = reader.GetString();
                if (string.IsNullOrWhiteSpace(propertyName)) { throw new JsonException("Failed to get property name"); }

                reader.Read();

                dictionary.Add(propertyName!, ExtractValue(ref reader));
            }

            return dictionary;
        }

        public override void Write(Utf8JsonWriter writer, Dictionary<string, object> value, JsonSerializerOptions options)
        {
            // We don't need any custom serialization logic for writing the json.
            // Ideally, this method should not be called at all. It's only called if you supply JsonSerializerOptions that contains this JsonConverter in it's Converters list.
            // Don't do that, you will lose performance because of the cast needed below.

            // Cast to avoid infinite loop: https://github.com/dotnet/docs/issues/19268
            JsonSerializer.Serialize(writer, (IDictionary<string, object>)value, options);
        }

        private static object ExtractValue(ref Utf8JsonReader reader)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    {
                        var value = reader.GetString();
                        if (value == null) { return string.Empty; }

                        return value;                        
                    }

                case JsonTokenType.StartArray:
                    {
                        var list = new List<string>();
                        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                        {
                            var value = reader.GetString();
                            if(value == null) { continue; }

                            list.Add(value);
                        }
                        return list;
                    }

                default:
                    {
                        throw new JsonException($"'{reader.TokenType}' is not supported");
                    }
            }
        }
    }
}
