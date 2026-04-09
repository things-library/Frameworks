// ================================================================================
// <copyright file="IServiceCollection.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using ThingsLibrary.Schema.Library;


namespace ThingsLibrary.Notification.Graph.Extensions
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Register Library Services
        /// </summary>
        /// <param name="services"></param>
        public static void AddEmailGraphService(this IServiceCollection services, ItemDto options)
        {   
            // register the singleton
            var emailOptions = new EmailServiceOptions(options);
            services.AddSingleton(emailOptions);

            // register the service
            services.AddScoped<IEmailNotifications, EmailNotificationService>();
        }
    }
}
