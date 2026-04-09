// ================================================================================
// <copyright file="DbConnectionStringBuilder.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using System.Data.Common;

namespace ThingsLibrary.DataType.Extensions
{
    /// <summary>
    /// Parse Database connection string builders
    /// </summary>
    public static class DbConnectionStringExtensions
    {     
        /// <summary>
        /// Get specific value from the connnection string parameters
        /// </summary>
        /// <typeparam name="T">Value Type</typeparam>
        /// <param name="builder">Connection string builder</param>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value to return if no key found</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static T GetValue<T>(this DbConnectionStringBuilder builder, string key, T defaultValue)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(builder.ConnectionString, nameof(builder));
            
            if(!builder.ContainsKey(key)) { return defaultValue; }

            return builder[key].ConvertTo<T>();            
        }
    }
}
