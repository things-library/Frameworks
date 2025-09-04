// ================================================================================
// <copyright file="ITrackingLocalizer.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Services.AspNetCore.DataType
{
    /// <summary>
    /// Provides localized strings with tracking capabilities.
    /// </summary>
    public interface ITrackingLocalizer
    {
        /// <summary>
        /// Gets the localized string for the specified key.
        /// </summary>
        /// <param name="key">The key to localize.</param>
        /// <returns>The localized string.</returns>
        public string this[string key] { get; }

        /// <summary>
        /// Gets the localized string for the specified key and value.
        /// </summary>
        /// <param name="key">The key to localize.</param>
        /// <param name="value">The value associated with the key.</param>
        /// <returns>The localized string.</returns>
        public string this[string key, string value] { get; }

        /// <summary>
        /// Gets the localized string for the specified key, value, and formatting arguments.
        /// </summary>
        /// <param name="key">The key to localize.</param>
        /// <param name="value">The value associated with the key.</param>
        /// <param name="arguments">An array of objects to format.</param>
        /// <returns>The formatted localized string.</returns>
        public string this[string key, string value, params object[] arguments] { get; }
    }
}