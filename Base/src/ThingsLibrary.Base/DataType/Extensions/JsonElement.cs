// ================================================================================
// <copyright file="JsonElement.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.DataType.Extensions
{
    public static partial class JsonElementExtensions
    {
        /// <summary>
        /// Get property from string path
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="jsonElement"><see cref="JsonElement"/></param>
        /// <param name="propertyPath">Property Path</param>
        /// <param name="defaultValue">Default value if not found</param>
        /// <returns>Property cast as return type if found, default value if not found </returns>
        public static T GetProperty<T>(this JsonElement jsonElement, string propertyPath, T defaultValue)
        {
            if (string.IsNullOrEmpty(propertyPath)) { throw new ArgumentNullException(nameof(propertyPath)); }

            var elementNames = propertyPath.Split('/');

            var propertyElement = jsonElement;
            for (var i = 0; i < elementNames.Length; i++)
            {
                if (!propertyElement.TryGetProperty(elementNames[i], out propertyElement)) { return defaultValue; }
            }

            return propertyElement.ToObject<T>();
        }

        public static T ToObject<T>(this JsonElement jElement)
        {
            var json = jElement.GetRawText();

            return JsonSerializer.Deserialize<T>(json) ?? throw new ArgumentException("Unable to deserialize object.");
        }
    }
}
