// ================================================================================
// <copyright file="IServiceCollection.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

//using ThingsLibrary.DataType.Extensions;

//namespace ThingsLibrary.Portal.Extensions
//{
//    public static class IServiceCollectionExtensions
//    {
//        /// <summary>
//        /// Configure the base pretty json serialization options that we prefer
//        /// </summary>
//        /// <param name="services"></param>
//        /// <param name="jsonOptions">Json Serializer Options</param>
//        /// <returns></returns>
//        public static IServiceCollection ConfigureJsonSerializerOptions(this IServiceCollection services, JsonSerializerOptions jsonOptions)
//        {
//            // ================================================================================
//            // JSON SERIALIZER OPTIONS
//            // ================================================================================
            
//            services.Configure<JsonSerializerOptions>(options =>
//            {
//                jsonOptions.CopyPropertyValues(options);

//                //options.AllowTrailingCommas = jsonOptions.AllowTrailingCommas;
//                //options.WriteIndented = jsonOptions.WriteIndented;
//                //options.DefaultIgnoreCondition = jsonOptions.DefaultIgnoreCondition;
//                //options.PropertyNamingPolicy = jsonOptions.PropertyNamingPolicy;
//                //options.PropertyNameCaseInsensitive = jsonOptions.PropertyNameCaseInsensitive;
//                //options.TypeInfoResolver = jsonOptions.TypeInfoResolver;
//            });

//            services.Configure<JsonOptions>(options =>
//            {
//                jsonOptions.CopyPropertyValues(options);               
//            });

//            // Needed for TypedResults responses
//            services.ConfigureHttpJsonOptions(options =>
//            {
//                jsonOptions.CopyPropertyValues(options);                
//            });

//            return services;
//        }
//    }
//}
