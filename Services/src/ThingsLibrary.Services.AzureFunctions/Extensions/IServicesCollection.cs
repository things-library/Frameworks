using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

//using ThingsLibrary.Security.OAuth2.Interfaces;
//using ThingsLibrary.Security.OAuth2.Services;
//using ThingsLibrary.Services.AzureFunctions.Middleware;

//namespace ThingsLibrary.Services.AzureFunctions.Extensions
//{
//    public static class IServicesCollectionExtensions
//    {
//        public static IServiceCollection AddClaimsPrincipalAccessor(this IServiceCollection services)
//        {
//            if (services == null) { throw new ArgumentNullException(nameof(services)); }

//            services.TryAddSingleton<IClaimsPrincipalAccessor, ClaimsPrincipalAccessor>();

//            return services;
//        }

//        public static IFunctionsWorkerApplicationBuilder AddAuth(this IFunctionsWorkerApplicationBuilder builder)
//        {
//            // We want to use this middleware only for http trigger invocations since only those have a claims principal
//            builder.UseWhen<AuthenticationMiddleware>(context => context.IsHttpTriggerFunction());
                        
//            builder.Services.TryAddSingleton<IClaimsPrincipalAccessor, ClaimsPrincipalAccessor>();

//            // for chaining
//            return builder;
//        }
//    }
//}
