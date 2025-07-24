// ================================================================================
// <copyright file="JsonSerializer.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using System.Diagnostics;
using System.Text.Json.Serialization.Metadata;
using Microsoft.AspNetCore.Mvc;

namespace ThingsLibrary.Portal.Extensions
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Configure the base pretty json serialization options that we prefer
        /// </summary>
        /// <param name="services"></param>
        /// <param name="jsonOptions">Json Serializer Options</param>
        /// <returns></returns>
        public static IServiceCollection ConfigureJsonSerializerOptions(this IServiceCollection services, JsonSerializerOptions jsonOptions)
        {
            // ================================================================================
            // JSON SERIALIZER OPTIONS
            // ================================================================================
            
            services.Configure<JsonSerializerOptions>(options =>
            {
                options.CopyPropertyValues(jsonOptions);                
            });

            services.Configure<JsonOptions>(options =>
            {
                options.JsonSerializerOptions.CopyPropertyValues(jsonOptions);
            });

            // Needed for TypedResults responses
            services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.TypeInfoResolver = jsonOptions.TypeInfoResolver;
            });

            return services;
        }
    }
}
