// ================================================================================
// <copyright file="IServiceCollection.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using Microsoft.Extensions.DependencyInjection;

namespace ThingsLibrary.Notification.Azure.Extensions
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Register Library Services
        /// </summary>
        /// <param name="services"></param>
        public static void AddSmsService(this IServiceCollection services, SmsServiceOptions options)
        {
            services.AddSingleton(options);

            services.AddScoped<SmsService>();
        }


        /// <summary>
        /// Register Library Services
        /// </summary>
        /// <param name="services"></param>
        public static void AddEmailService(this IServiceCollection services, EmailServiceOptions options)
        {
            services.AddSingleton(options);

            services.AddScoped<EmailService>();
        }


    }
}
