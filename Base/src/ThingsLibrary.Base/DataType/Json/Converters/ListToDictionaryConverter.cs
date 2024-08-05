using ThingsLibrary.Interfaces;

namespace ThingsLibrary.DataType.Json.Converters
{
    /// <summary>
    /// The purpose of this converter is so we aren't serializing the 'key'.  Once for the dictionary key and once inside of the object.  When deserializing we need to put it back however
    /// </summary>
    /// <typeparam name="T">Class with IKey interface</typeparam>
    /// <remarks>Key property on T object should be inheriting the IKey interface</remarks>
    public class ListToDictionaryConverter<T> : JsonConverter<Dictionary<string, T>> where T : IKey
    {
        public override Dictionary<string, T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var items = JsonSerializer.Deserialize<List<T>>(ref reader, options) ?? throw new ArgumentException("Unable to deserialize list of items.");

            return items.ToDictionary(x => x.Key, x => x);
        }

        public override void Write(Utf8JsonWriter writer, Dictionary<string, T> thingAttributes, JsonSerializerOptions options)
        {
            var items = thingAttributes.Values.ToList();

            writer.WriteRawValue(JsonSerializer.Serialize(items, options));
        }
    }
}
