// ================================================================================
// <copyright file="DateTimeJsonConverter.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using System.Globalization;

namespace ThingsLibrary.DataType.Json.Converters
{
    public class DateTimeJsonAttribute : JsonConverterAttribute
    {
        public string Format { get; init; }

        public DateTimeJsonAttribute(string format = "yyyy-MM-dd")
        {
            this.Format = format;
        }

        public override JsonConverter CreateConverter(Type typeToConvert)
        {
            if (typeToConvert != typeof(DateTime))
            {
                throw new ArgumentException($"This converter only works with DateTime, and it was provided {typeToConvert.Name}.");
            }

            return new DateTimeJsonConverter(this.Format);
        }
    }

    public class DateTimeJsonConverter : JsonConverter<DateTime>
    {
        public string Format { get; init; }

        public DateTimeJsonConverter()
        {
            this.Format = "yyyy-MM-dd";
        }

        /// <summary>
        /// Specify the date time serialization format
        /// </summary>
        /// <param name="format">DateTime format to use (Default: yyyy-MM-dd)</param>
        public DateTimeJsonConverter(string format = "yyyy-MM-dd")
        {
            this.Format = format;
        }

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.ParseExact(reader.GetString()!, this.Format, CultureInfo.InvariantCulture);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(this.Format, CultureInfo.InvariantCulture));
        }        
    }

}
