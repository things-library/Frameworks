namespace ThingsLibrary.DataType.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Support doing ForEach on anything that is IEnumerable not list Lists
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumeration"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {            
            foreach (T item in enumeration)
            {
                action(item);
            }
        }

        /// <summary>
        /// Get the property values for the collection of items based on property name
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="T">Property Type</typeparam>
        /// <param name="records">Collection of records</param>
        /// <param name="propertyName">Property Name to get the value</param>
        /// <returns></returns>
        public static IEnumerable<T> GetPropertyValues<TEntity, T>(this IEnumerable<TEntity> records, string propertyName) where TEntity : class
        {
            var recordType = typeof(TEntity);
            var values = new List<T>(records.Count());  // well it can't be more than the collection passed

            foreach (var record in records)
            {
                var property = recordType.GetProperty(propertyName);
                if(property == null) { continue; }

                var value = property.GetValue(record);
                if(value == null) { continue; }

                values.Add((T)value);
            }

            return values;
        }
    }
}
