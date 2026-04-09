// ================================================================================
// <copyright file="JsonDocument.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.DataType.Extensions
{
    public static partial class JsonDocumentExtensions
    {
        /// <summary>
        /// Get property from string path
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="jDocument"><see cref="JsonDocument"/></param>
        /// <param name="propertyPath">Property Path</param>
        /// <param name="defaultValue">Default value if not found</param>
        /// <returns>Property cast as return type if found, default value if not found </returns>
        public static T GetProperty<T>(this JsonDocument jDocument, string propertyPath, T defaultValue)
        {
            return jDocument.RootElement.GetProperty<T>(propertyPath, defaultValue);
        }

        public static T ToObject<T>(this JsonDocument jDocument)
        {
            var json = jDocument.RootElement.GetRawText();

            return JsonSerializer.Deserialize<T>(json) ?? throw new ArgumentException("Unable to deserialize object.");
        }
    }
}
