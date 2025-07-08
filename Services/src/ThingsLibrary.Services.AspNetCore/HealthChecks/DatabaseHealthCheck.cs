// ================================================================================
// <copyright file="DatabaseHealthCheck.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

//using Microsoft.Extensions.Diagnostics.HealthChecks;

//namespace ThingsLibrary.Services.AspNetCore.HealthChecks
//{
//    /// <summary>
//    /// Configuration Variables: HealthCheck.Database.Response
//    /// </summary>
//    public class DatabaseHealthCheck : IHealthCheck
//    {
//        private IConfiguration Configuration { get; set; }
//        private DbContext DataContext { get; set; }

//        /// <summary>
//        /// Constructor
//        /// </summary>
//        /// <param name="configuration">IConfiguration</param>
//        /// <param name="context">Data Context</param>
//        public DatabaseHealthCheck(IConfiguration configuration, DbContext context)
//        {
//            this.Configuration = configuration;
//            this.DataContext = context;
//        }

//        /// <summary>
//        /// Check the health
//        /// </summary>
//        /// <param name="context">Health Check Context</param>
//        /// <param name="cancellationToken">Cancel Token</param>
//        /// <returns>Health Status</returns>
//        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
//        {
//            try
//            {
//                var description = "Reports degraded status when:";

//                var start = DateTime.UtcNow;

//                var rows = await this.DataContext.Database.ExecuteSqlRawAsync("SELECT 1");
//                if (rows != 1)
//                {
//                    Log.Debug("UNABLE TO FIND OBJECT TYPE 1!!!");
//                }

//                var responseTime = DateTime.UtcNow - start;

//                // build the payload
//                var data = new Dictionary<string, object>
//                {
//                    { "ResponseTimeMs", System.Math.Round(responseTime.TotalMilliseconds, 0) }
//                };

//                var status = HealthStatus.Healthy;

//                if (this.Configuration.GetSection("AppService:HealthChecks:Database").Exists())
//                {
//                    if (this.Configuration.GetSection("AppService:HealthChecks:Database:Response").Exists())
//                    {
//                        var maxResponse = this.Configuration.GetValue<double>("AppService:HealthChecks:Database:Response", 0);
//                        if (maxResponse > 0)
//                        {
//                            description += $" database response more than {maxResponse} ms;";

//                            data.Add("ResponseTimePercent", System.Math.Round((responseTime.TotalMilliseconds / maxResponse) * 100, 1));

//                            if (responseTime.TotalMilliseconds > maxResponse)
//                            {
//                                status = HealthStatus.Degraded;
//                            }
//                        }
//                    }
//                }
//                else
//                {
//                    description = " Never (No Checks)";
//                }

//                return new HealthCheckResult(
//                        status: status,
//                        description: description,
//                        exception: null,
//                        data: data);

//            }
//            catch (Exception ex)
//            {
//                return new HealthCheckResult(
//                    status: HealthStatus.Unhealthy,
//                    description: ex.Message,
//                    exception: ex,
//                    data: null);

//            }
//        }
//    }
//}
