// ================================================================================
// <copyright file="Breadcrumb.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.DataType
{
    [DebuggerDisplay("DisplayMessage: {DisplayMessage}, Active: {Active}, Url: {Url}")]
    public class Breadcrumb
    {
        [JsonPropertyName("title")]
        public string Title { get; init; } = string.Empty;

        [JsonPropertyName("url")]
        public string Url { get; init; } = string.Empty;

        [JsonPropertyName("active")]
        public bool Active { get; init; }

        /// <summary>
        /// Breadcrumb
        /// </summary>
        public Breadcrumb()
        {
            //for serialization
        }

        public Breadcrumb(string title, string url, bool isActive = false)
        {
            this.Title = title;
            this.Url = url;
            this.Active = isActive;
        }
    }
}
