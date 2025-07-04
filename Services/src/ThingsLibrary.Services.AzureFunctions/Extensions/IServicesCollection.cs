// ================================================================================
// <copyright file="IServicesCollection.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

using ThingsLibrary.Security.OAuth2.Services;
using ThingsLibrary.Services.AzureFunctions.Middleware;

namespace ThingsLibrary.Services.AzureFunctions.Extensions
{
    public static class IServicesCollectionExtensions
    {
        public static IServiceCollection AddClaimsPrincipalAccessor(this IServiceCollection services)
        {
            if (services == null) { throw new ArgumentNullException(nameof(services)); }

            services.TryAddSingleton<IClaimsPrincipalAccessor, ClaimsPrincipalAccessor>();

            return services;
        }

        public static IFunctionsWorkerApplicationBuilder AddAuth(this IFunctionsWorkerApplicationBuilder builder)
        {
            // We want to use this middleware only for http trigger invocations since only those have a claims principal
            builder.UseWhen<AuthenticationMiddleware>(context => context.IsHttpTriggerFunction());

            builder.Services.TryAddSingleton<IClaimsPrincipalAccessor, ClaimsPrincipalAccessor>();

            // for chaining
            return builder;
        }
    }
}
