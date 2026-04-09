// ================================================================================
// <copyright file="StringOrEmptyArrayConverter.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using System.Collections.Generic;

namespace ThingsLibrary.DataType.Json.Converters
{
    public class StringOrEmptyArrayConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type objectType, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var value = reader.GetString();
                if (value == null) { return string.Empty; }

                return value;
            }
            else
            {
                var list = JsonSerializer.Deserialize<List<string>>(ref reader) ?? throw new ArgumentException("Unable to deserialize object.");
                if (list.Count > 0) { throw new ArgumentException($"Unexpected multiple values for TokenType '{reader.TokenType}'."); }

                return "";
            }
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value);
        }
    }
}
