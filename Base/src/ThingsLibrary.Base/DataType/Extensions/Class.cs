// ================================================================================
// <copyright file="Class.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using ThingsLibrary.Attributes;
using System.Reflection;

namespace ThingsLibrary.DataType.Extensions
{
    public static class ClassExtensions
    {
        #region --- Validation --- 

        /// <summary>
        /// Validate the class based on the Data Annotations
        /// </summary>
        /// <param name="instance">class object to validate</param>
        /// <param name="flatten">Flattens all CompositeValidationResults into a single listing</param>
        /// <returns></returns>
        /// <remarks>This method evaluates each ValidationAttribute instance that is attached to the object type. It also checks whether each property that is marked with RequiredAttribute is provided. It does not recursively validate the property values of the object.</remarks>
        public static ICollection<ValidationResult> ToValidationResults(this object instance, bool flatten = false)
        {
            var results = new List<ValidationResult>();

            Validator.TryValidateObject(instance, new ValidationContext(instance), results, true);

            if (flatten)
            {
                return results.Flatten();
            }
            else
            {
                return results;
            }            
        }

        /// <summary>
        /// Flatten the composite validation results into a single flat listing
        /// </summary>
        /// <param name="results">Validation Results</param>
        /// <returns></returns>
        public static ICollection<ValidationResult> Flatten(this ICollection<ValidationResult> results)
        {
            var list = new List<ValidationResult>();

            foreach (var result in results)
            {
                if (result is CompositeValidationResult compositeResult)
                {
                    list.AddRange(compositeResult.Flatten());
                }
                else
                {
                    list.Add(result);
                }
            }

            //return list.OrderBy(x => x.MemberNames.FirstOrDefault()).ToList();
            return list;
        }

        /// <summary>
        /// Flatten the composite validation results into a single flat listing
        /// </summary>
        /// <param name="result">Composite Validation Result</param>
        /// <returns></returns>
        public static ICollection<ValidationResult> Flatten(this CompositeValidationResult result)
        {
            var list = new List<ValidationResult>();

            if (result.Results.Any())
            {
                var memberName = result.MemberNames.FirstOrDefault();
                foreach (var subResult in result.Results)
                {
                    if (subResult is CompositeValidationResult compositeResult)
                    {
                        var flatList = compositeResult.Flatten();
                        flatList.ForEach(result =>
                        {
                            list.Add(result);
                        });
                    }
                    else
                    {
                        list.Add(new ValidationResult(subResult.ErrorMessage, [ $"{memberName}.{subResult.MemberNames.FirstOrDefault()}" ]));
                    }
                }
            }
            else
            {
                list.Add(result as ValidationResult);
            }

            return list;
        }

        /// <summary>
        /// Logs the validation results to Debug Serilog
        /// </summary>
        /// <param name="results">Validation Results</param>
        public static void LogResults(this ICollection<ValidationResult> results)
        {
            if (results.Count == 0) { return; }
                        
            foreach (var result in results)
            {
                if (result is CompositeValidationResult compositeResult)
                {
                    compositeResult.LogResult();
                }                
            }
        }

        /// <summary>
        /// Logs the composite validation result
        /// </summary>
        /// <param name="result">Validation Result</param>
        /// <param name="prefix">Prefix to put before each log entry</param>
        public static void LogResult(this CompositeValidationResult result, string prefix = "")
        {
            if (result.Results.Any())
            {
                foreach (var subResult in result.Results)
                {
                    if (subResult is CompositeValidationResult compositeResult)
                    {
                        compositeResult.LogResult(prefix + "  ");
                    }                    
                }
            }            
        }

        #endregion

        /// <summary>
        /// Convert class object into query string
        /// </summary>
        /// <param name="instance">Class Instance</param>
        /// <param name="separator">Separator character</param>
        /// <returns></returns>
        public static string ToQueryString(this object instance, string separator = "&")
        {
            var step1 = JsonSerializer.Serialize(instance);

            var step2 = JsonSerializer.Deserialize<IDictionary<string, object>>(step1);
            if (step2 == null) { return string.Empty; }

            var step3 = step2.Select(x => $"{HttpUtility.UrlEncode(x.Key)}={HttpUtility.UrlEncode(x.Value.ToString())}");

            return string.Join(separator, step3);
        }

        /// <summary>
        /// Get the value at the property of the instance
        /// </summary>
        /// <param name="instance">Class Instance</param>        
        /// <param name="property">Property</param>
        /// <returns>List of key values</returns>
        public static T GetPropertyValue<T>(this object instance, PropertyInfo property, T devaultValue)
        {
            var value = property.GetValue(instance);
            if (value == null) { return devaultValue; }

            return (T)value;
        }

        /// <summary>
        /// Get the value at the property of the instance
        /// </summary>
        /// <param name="property">Property</param>
        /// <param name="instance">Class Instance</param> 
        /// <returns>List of key values</returns>
        public static T GetPropertyValue<T>(this PropertyInfo property, object instance, T devaultValue)
        {
            var value = property.GetValue(instance);
            if (value == null) { return devaultValue; }

            return (T)value;
        }

        /// <summary>
        /// Get the custom attribute T data annotated fields of the class object
        /// </summary>
        /// <param name="instance">Class Instance</param>        
        /// <param name="key">Key</param>
        /// <returns>List of key values</returns>
        public static TReturn GetPropertyValue<TAttribute, TReturn>(this object instance, TReturn defaultValue) where TAttribute : Attribute
        {            
            var type = instance.GetType();

            var attrib = type.GetProperties().FirstOrDefault(x => x.GetCustomAttributes(typeof(TAttribute), false).Any());
            if (attrib == null) 
            {                
                return defaultValue; 
            }

            return GetPropertyValue<TReturn>(instance, attrib, defaultValue);
        }

        /// <summary>
        /// Get the [key] data annotated fields of the class object
        /// </summary>
        /// <param name="instance">Class Instance</param>        
        /// <param name="instanceType">Class Type if available (fetches if null)</param>e"></param>
        /// <returns>List of key values</returns>
        /// <exception cref="ArgumentException">When no [Key] fields exist</exception>
        public static List<object> GetKeys(this object instance, Type? instanceType = null)
        {
            // limit reflection if possible
            if (instanceType == null)
            {
                instanceType = instance.GetType();
            }

            var keys = instanceType.GetProperties().Where(x => x.GetCustomAttributes(typeof(KeyAttribute), false).Length > 0).ToList();
            if (keys.Count == 0) { throw new ArgumentException("No [Key] annotated fields found in class instance"); }

            return instance.GetPropertyValues(keys);
        }

        /// <summary>
        /// Get the property values for the list of properties
        /// </summary>
        /// <param name="instance">Class Instance</param>        
        /// <param name="properties">Properties to get the values</param>
        /// <returns>List of key values</returns>
        public static List<object> GetPropertyValues(this object instance, IEnumerable<PropertyInfo> properties)
        {
            // allocate memory for our list based on how many items we will have
            var keyList = new List<object>(properties.Count());

            foreach (var property in properties)
            {
                var value = property.GetValue(instance, null);
                if (value == null) { continue; }

                keyList.Add(value);
            }

            return keyList;
        }

        ///// <summary>
        ///// Copy the public properties of one class object to the other class object
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="source"></param>
        ///// <param name="target"></param>
        ///// <exception cref="ArgumentNullException"></exception>
        //public static void CopyPropertyValues<TEntity>(this TEntity source, TEntity target) where TEntity : class
        //{
        //    ArgumentNullException.ThrowIfNull(source);
        //    ArgumentNullException.ThrowIfNull(target);

        //    var properties = typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        //    foreach (PropertyInfo property in properties)
        //    {
        //        if (property.CanRead && property.CanWrite)
        //        {
        //            object? value = property.GetValue(source);
        //            property.SetValue(target, value);
        //        }
        //    }
        //}

        /// <summary>
        /// Copy the public properties of one class object to the other class object
        /// </summary>
        /// <typeparam name="TEntity">Class entity</typeparam>
        /// <param name="source">Source Object</param>
        /// <param name="destination">Destination Object</param>
        public static void CopyPropertyValues<TEntity>(this TEntity source, object destination) where TEntity : class
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(destination);

            var sourceType = source.GetType();

            var destinationType = destination.GetType();
            var destinationProperties = destinationType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanWrite);

            foreach (var destinationProperty in destinationProperties)
            {
                var destinationSetProperty = destinationProperty.GetSetMethod(false);
                if (destinationSetProperty?.IsPrivate != true) { continue; }

                var sourceProperty = sourceType.GetProperty(destinationProperty.Name, BindingFlags.Public | BindingFlags.Instance);
                if (sourceProperty?.CanRead != true) { continue; }

                if (!destinationProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType)) { continue; }

                var value = sourceProperty.GetValue(source, null);
                destinationProperty.SetValue(destination, value, null);
            }
        }

        /// <summary>
        /// Get the key values from the instance delimited by the delimiter char
        /// </summary>
        /// <param name="instance">Class Instance</param>
        /// <param name="delimiter">Delimiter</param>
        /// <param name="type">Class Type if available (fetches if null)</param>
        /// <returns></returns>
        public static string GetCompositeKey(this object instance, char delimiter = '|', Type? type = null)
        {
            return string.Join(delimiter, instance.GetKeys(type));
        }

        /// <summary>
        /// Get the key values from the instance delimited by the delimiter char
        /// </summary>
        /// <param name="instance">Class Instance</param>
        /// <param name="delimiter">Delimiter</param>
        /// <param name="type">Class Type if available (fetches if null)</param>
        /// <returns></returns>
        public static string GetCompositeKey(this object instance, IEnumerable<PropertyInfo> keys, char delimiter = '|')
        {
            return string.Join(delimiter, instance.GetPropertyValues(keys));
        }
    }
}
