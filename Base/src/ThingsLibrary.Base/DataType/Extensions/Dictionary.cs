// ================================================================================
// <copyright file="Dictionary.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using System.ComponentModel.Design;

namespace ThingsLibrary.DataType.Extensions
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Clones the simple dictionary
        /// </summary>
        /// <param name="source">Dictionary</param>
        /// <returns></returns>
        public static Dictionary<string, string> Clone(this Dictionary<string, string> source)
        {
            return source.ToDictionary(x => x.Key, x => x.Value);
        }
         
        /// <summary>
        /// Add range of items
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="source"></param>
        /// <param name="collection"></param>
        /// <param name="replaceDuplicates"></param>
        public static void AddRange<T, S>(this IDictionary<T, S> source, Dictionary<T, S> collection, bool replaceDuplicates = true)
        {
            foreach (var item in collection)
            {
                if (!source.ContainsKey(item.Key))
                {
                    source.Add(item.Key, item.Value);
                }
                else if (replaceDuplicates)
                {
                    // handle duplicate key issue
                    source[item.Key] = item.Value;
                }
            }
        }

        /// <summary>
        /// Get/Convert from a string dictionary of many different possible data type records
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="dictionary">Dictionary Lookup</param>
        /// <param name="key">Key to lookup</param>
        /// <param name="defaultValue">Default Value</param>
        /// <returns></returns>
        public static T GetValue<T>(this IDictionary<string, string> dictionary, string key, T defaultValue)
        {
            if (!dictionary.TryGetValue(key, out string? value))
            {
                return defaultValue;
            }
                       
            var itemType = typeof(T);

            // parse datetime differently
            if (itemType == typeof(DateTime))
            {
                if (DateTime.TryParse(value, out DateTime result))
                {
                    return result.ConvertTo<T>();
                }
                else
                {
                    return defaultValue;
                }
            }

            if (itemType == typeof(DateTimeOffset))
            {
                if (DateTimeOffset.TryParse(value, out DateTimeOffset result))
                {
                    return result.ConvertTo<T>();
                }
                else
                {
                    return defaultValue;
                }
            }

            return value.ConvertTo<T>();
        }

        public static T GetValue<T>(this IDictionary<string, object> dictionary, string key, T defaultValue)
        {            
            if (!dictionary.TryGetValue(key, out object? value)) { return defaultValue; }
                        
            var returnType = typeof(T);            
            if (returnType != value.GetType())
            {
                return defaultValue;    //TODO: FIXME
            }

            return value.ConvertTo<T>();
        }

        public static string GetValueString(this IDictionary<string, object> dictionary, string key, string defaultValue)
        {
            if (!dictionary.TryGetValue(key, out object? value)) { return defaultValue; }

            return $"{value}";
        }


        public static bool UpdateKey<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey oldKey, TKey newKey)
        {
            if (!dictionary.Remove(oldKey, out TValue? value)) { return false; }

            dictionary[newKey] = value;  // or dict.Add(newKey, value) depending on ur comfort

            return true;
        }
    }
}
