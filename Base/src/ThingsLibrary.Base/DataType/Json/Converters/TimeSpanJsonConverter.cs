// ================================================================================
// <copyright file="TimeSpanJsonConverter.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using System.Globalization;

namespace ThingsLibrary.DataType.Json.Converters
{
    public class TimeSpanJsonAttribute : JsonConverterAttribute
    {
        public TimeSpanJsonAttribute()
        {
            
        }

        public override JsonConverter CreateConverter(Type typeToConvert)
        {
            if (typeToConvert != typeof(TimeSpan))
            {
                throw new ArgumentException($"This converter only works with TimeSpan, and it was provided {typeToConvert.Name}.");
            }

            return new DateTimeJsonConverter();
        }
    }

    public class TimeSpanJsonConverter : JsonConverter<TimeSpan>
    {   
        public TimeSpanJsonConverter()
        {
            
        }

        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return TimeSpan.Parse(reader.GetString()!, CultureInfo.InvariantCulture);
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }        
    }

}
