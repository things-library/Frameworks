// ================================================================================
// <copyright file="ArrayOrSingleConverter.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.DataType.Json.Converters
{
    public class ArrayOrSingleConverter<T> : JsonConverter<List<T>>
    {
        public override List<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // is it just an array already?
            if (reader.TokenType == JsonTokenType.StartArray)
            {
                return JsonSerializer.Deserialize<List<T>>(ref reader) ?? [];
            }
            else
            {
                // make it a list so we can return deserialize it as it if was a single item array
                var value = JsonSerializer.Deserialize<T>(ref reader);
                if(value == null) { return []; }

                return [value];
            }
        }

        public override void Write(Utf8JsonWriter writer, List<T> value, JsonSerializerOptions options)
        {
            // write out single generic or list of generics as json
            //if (value.Count == 1)
            //{
            //    JsonSerializer.Serialize(writer, value[0]);
            //}
            //else
            //{
                JsonSerializer.Serialize(writer, value, options);
            //}
        }
    }
}
