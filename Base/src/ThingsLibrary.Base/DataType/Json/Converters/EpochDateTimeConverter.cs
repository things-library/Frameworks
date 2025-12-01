// ================================================================================
// <copyright file="EpochDateTimeConverter.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.DataType.Json.Converters
{    
    public class EpochDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.Number)
            {
                throw new JsonException("Expected number for epoch time.");
            }

            long epochValue = reader.GetInt64();

            // Detect if epoch is in seconds or milliseconds
            if (epochValue > 9999999999)    // > year 2286 in seconds
            {
                return DateTimeOffset.FromUnixTimeMilliseconds(epochValue).UtcDateTime;
            }
            else
            {
                return DateTimeOffset.FromUnixTimeSeconds(epochValue).UtcDateTime;
            }                
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            // Write as seconds since epoch
            long epochSeconds = new DateTimeOffset(value).ToUnixTimeSeconds();
            writer.WriteNumberValue(epochSeconds);
        }
    }
}
