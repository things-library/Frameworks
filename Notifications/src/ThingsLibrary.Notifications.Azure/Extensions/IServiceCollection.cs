// ================================================================================
// <copyright file="IServiceCollection.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using ThingsLibrary.Schema.Library;
using ThingsLibrary.Services.Extensions;

namespace ThingsLibrary.Notification.Azure.Extensions
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Register Library Services
        /// </summary>
        /// <param name="services"></param>
        public static void AddSmsAzureService(this IServiceCollection services, SmsServiceOptions options)
        {
            services.AddSingleton(options);

            services.AddScoped<SmsService>();
        }


        /// <summary>
        /// Register Library Services
        /// </summary>
        /// <param name="services"></param>
        public static void AddEmailAzureService(this IServiceCollection services, ItemDto options, string addressFrom, IConfiguration configuration)// EmailServiceOptions options)
        {
            var emailConnectionString = configuration.TryGetConnectionString(options["connection_string_variable"] ?? "Notifications");
            
            var emailOptions = new EmailServiceOptions(emailConnectionString, addressFrom);

            services.AddSingleton(emailOptions);

            services.AddScoped<EmailNotificationService>();
        }


    }
}
