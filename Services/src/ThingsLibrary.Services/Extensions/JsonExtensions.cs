// ================================================================================
// <copyright file="JsonExtensions.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using ThingsLibrary.Schema.Library.Base;

namespace ThingsLibrary.Services.Extensions
{
    public static class JsonExtensions
    {
        /// <summary>
        /// Update the default JsonSerializerOptions to match the SchemaBase serializer options
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <returns>IServiceCollection for chaining</returns>
        public static IServiceCollection ConfigureJsonSerializer(this IServiceCollection services)
        {
            services.Configure<JsonSerializerOptions>(options =>
            {
                // override the default value
                options.AllowTrailingCommas = SchemaBase.JsonSerializerOptions.AllowTrailingCommas;
                options.DefaultIgnoreCondition = SchemaBase.JsonSerializerOptions.DefaultIgnoreCondition;
                options.PropertyNamingPolicy = SchemaBase.JsonSerializerOptions.PropertyNamingPolicy;
                options.PropertyNameCaseInsensitive = SchemaBase.JsonSerializerOptions.PropertyNameCaseInsensitive;
                options.ReferenceHandler = SchemaBase.JsonSerializerOptions.ReferenceHandler;                
                options.WriteIndented = SchemaBase.JsonSerializerOptions.WriteIndented;
                options.TypeInfoResolver = SchemaBase.JsonSerializerOptions.TypeInfoResolver;

                //foreach(var typeResolver in SchemaBase.JsonSerializerOptions.TypeInfoResolverChain)
                //{
                //    options.TypeInfoResolverChain.Add(typeResolver);
                //}                
            });            

            return services;
        }        
    }
}
