namespace ThingsLibrary.Database.Extensions
{
    /// <summary>
    /// Property Builder Extensions
    /// </summary>
    public static class PropertyBuilderExtensions
    {
        /// <summary>
        /// Json Options
        /// </summary>
        public static JsonSerializerOptions JsonOptions { get; set; } = new JsonSerializerOptions();

        /// <summary>
        /// Json Document Options
        /// </summary>
        public static JsonDocumentOptions JsonDocumentOptions { get; set; } = new JsonDocumentOptions();

        /// <summary>
        /// Has Json Conversion
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="propertyBuilder">PropertyBuilder</param>
        /// <returns></returns>
        public static PropertyBuilder<T> HasJsonConversion<T>(this PropertyBuilder<T> propertyBuilder) where T : class, new()
        {
            var converter = new ValueConverter<T, string>
            (
                v => JsonSerializer.Serialize(v, JsonOptions),
                v => JsonSerializer.Deserialize<T>(v, JsonOptions) ?? new T()
            );

            var comparer = new ValueComparer<T>
            (
                (l, r) => JsonSerializer.Serialize(l, JsonOptions) == JsonSerializer.Serialize(r, JsonOptions),
                v => v == null ? 0 : JsonSerializer.Serialize(v, JsonOptions).GetHashCode(),
                v => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(v, JsonOptions), JsonOptions)
            );

            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.Metadata.SetValueComparer(comparer);
            //propertyBuilder.HasColumnType("varchar");
            //propertyBuilder.HasColumnType("nvarchar(max)");

            return propertyBuilder;
        }

        /// <summary>
        /// Has Json Conversion
        /// </summary>
        /// <param name="propertyBuilder">Property Builder</param>
        /// <returns></returns>
        public static PropertyBuilder<JsonDocument> HasJsonConversion(this PropertyBuilder<JsonDocument> propertyBuilder)
        {
            var converter = new ValueConverter<JsonDocument, string>
            (
                v => JsonSerializer.Serialize(v, JsonOptions),
                v => JsonDocument.Parse(v, JsonDocumentOptions) ?? default
            );

            var comparer = new ValueComparer<JsonDocument>
            (
                (l, r) => JsonSerializer.Serialize(l, JsonOptions) == JsonSerializer.Serialize(r, JsonOptions),
                v => v == null ? 0 : JsonSerializer.Serialize(v, JsonOptions).GetHashCode(),
                v => JsonDocument.Parse(JsonSerializer.Serialize(v, JsonOptions), JsonDocumentOptions)
            );

            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.Metadata.SetValueComparer(comparer);
            //propertyBuilder.HasColumnType("nvarchar");

            return propertyBuilder;
        }


        /// <summary>
        /// Has Json Conversion
        /// </summary>
        /// <param name="propertyBuilder">Property Builder</param>
        /// <returns></returns>
        public static PropertyBuilder<Dictionary<string, string>> HasJsonConversion(this PropertyBuilder<Dictionary<string, string>> propertyBuilder)
        {
            var converter = new ValueConverter<Dictionary<string, string>, string>
            (
                v => JsonSerializer.Serialize(v, JsonOptions),
                v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, new JsonSerializerOptions()) ?? new Dictionary<string, string>()
            );

            var comparer = new ValueComparer<JsonDocument>
            (
                (l, r) => JsonSerializer.Serialize(l, JsonOptions) == JsonSerializer.Serialize(r, JsonOptions),
                v => v == null ? 0 : JsonSerializer.Serialize(v, JsonOptions).GetHashCode(),
                v => JsonDocument.Parse(JsonSerializer.Serialize(v, JsonOptions), JsonDocumentOptions)
            );

            propertyBuilder.HasConversion(converter);
            propertyBuilder.Metadata.SetValueConverter(converter);
            propertyBuilder.Metadata.SetValueComparer(comparer);
            //propertyBuilder.HasColumnType("nvarchar");

            return propertyBuilder;
        }
    }
}
