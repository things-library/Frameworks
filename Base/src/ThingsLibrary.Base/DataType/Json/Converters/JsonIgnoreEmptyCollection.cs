// ================================================================================
// <copyright file="JsonIgnoreEmptyCollection.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using System.Text.Json.Serialization.Metadata;

namespace ThingsLibrary.DataType.Json.Converters
{
    /// <summary>
    /// Json Ignore Attribute that when added to collections will JsonIgnore the collection when empty.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class JsonIgnoreEmptyCollectionAttribute : Attribute { }

    /// <summary>
    /// Json Ignore Attribute that when added to collections will JsonIgnore the collection when empty.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class JsonIgnoreDefaultAttribute : Attribute { }

    /// <summary>
    /// Json ignores any collection that is empty that has the above attribute attached to it.
    /// </summary>
    /// <remarks>To use this resolver it must be registered with the json serialization options:
    /// TypeInfoResolver = new DefaultJsonTypeInfoResolver
    /// {
    ///     Modifiers = { JsonIgnoreEmptyCollection.IgnoreEmptyCollections
    /// }
    /// </remarks>
    public static class JsonIgnoreEmptyCollection
    {
        /// <summary>
        /// Finds all properties of the class being serialized that have a JsonIgnoreEmptyCollection attribute with 0 elements in the collection and ignores it.
        /// </summary>
        /// <param name="jsonTypeInfo"></param>
        public static void IgnoreEmptyCollections(JsonTypeInfo jsonTypeInfo)
        {
            if (jsonTypeInfo.Kind != JsonTypeInfoKind.Object) { return; }

            foreach (JsonPropertyInfo propertyInfo in jsonTypeInfo.Properties)
            {
                // property MUST have a JsonIgnoreEmptyCollection attribute declaration
                if (propertyInfo.AttributeProvider?.GetCustomAttributes(typeof(JsonIgnoreEmptyCollectionAttribute), true).Length > 0)
                {
                    propertyInfo.ShouldSerialize = (obj, prop) =>
                    {
                        if (prop == null) { return false; }

                        var collectionType = typeof(ICollection<>);

                        foreach (var interfaceType in prop.GetType().GetInterfaces())
                        {
                            if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == collectionType)
                            {
                                var countProperty = interfaceType.GetProperty("Count");
                                if (countProperty != null && (int)countProperty.GetValue(prop)! == 0)   //if the property exist then it is a int
                                {
                                    return false;
                                }
                            }
                        }

                        return true;
                    };
                }

                // STRING EMPTY OR NULL
                if (propertyInfo.AttributeProvider?.GetCustomAttributes(typeof(JsonIgnoreDefaultAttribute), true).Length > 0)
                {
                    propertyInfo.ShouldSerialize = (obj, propValue) =>
                    {
                        if (propValue == null) { return false; }

                        if (propertyInfo.PropertyType == typeof(String))
                        {
                            return ((string)propValue != string.Empty);
                        }

                        if (propertyInfo.PropertyType == typeof(Int16))
                        {
                            return ((Int16)propValue != 0);
                        }

                        return true;
                    };
                }
            }
        }
    }
}
