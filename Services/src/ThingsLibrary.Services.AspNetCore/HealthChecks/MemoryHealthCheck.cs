// ================================================================================
// <copyright file="MemoryHealthCheck.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ThingsLibrary.Services.AspNetCore.HealthChecks
{
    /// <summary>
    /// Configuration Variables: 
    ///     HealthCheck.Memory.Process.Used (KB), 
    ///     HealthCheck.Memory.Machine.Used (KB), 
    ///     HealthCheck.Memory.Machine.UsedPercent, 
    ///     HealthCheck.Memory.Machine.Available (KB), 
    ///     HealthCheck.Memory.Machine.AvailablePercent
    /// </summary>
    public class MemoryHealthCheck : IHealthCheck
    {        
        private IConfiguration Configuration { get; set; }
        private ILogger Logger { get; set; }
        
        /// <summary>
        /// Constructor
        /// </summary>
        public MemoryHealthCheck(IConfiguration configuration, ILogger<MemoryHealthCheck> logger)
        {
            this.Configuration = configuration;// App.Service.Configuration;
            this.Logger = logger;
        }

        /// <summary>
        /// Check the health
        /// </summary>
        /// <param name="context">Health Check Context</param>
        /// <param name="cancellationToken">Cancel Token</param>
        /// <returns>Health Status</returns>
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var description = "Reports degraded status when:";

                var machineMemory = Metrics.MemoryMetrics.GetSnapshot();
                var processMemoryUsedKB = Metrics.MachineMetrics.ProcessUsedMemoryKB;

                // annoy the operator
                if (machineMemory == null)
                {
                    //Requires 'procps' library in Linux to run MemoryMetrics.GetSnapshot
                    this.Logger.LogWarning("HealthCheck.Memory: Error getting machine memory!! Missing 'procps' linux library?");
                }

                var data = new Dictionary<string, object>()
                {
                    { "ProcessUsedKB", $"{processMemoryUsedKB}" },
                    { "MachineUsedKB", $"{machineMemory ?.UsedKB}"},
                    { "MachineFreedKB", $"{machineMemory ?.AvailableKB}"},
                    { "MachineTotalKB", $"{machineMemory?.TotalKB}"}
                };

                var status = HealthStatus.Healthy;

                if (this.Configuration.GetSection("App:HealthChecks:Memory").Exists())
                {
                    // =============================================================================================
                    // PROCESS 
                    // =============================================================================================
                    if (this.Configuration.GetSection("App:HealthChecks:Memory:Process:Used").Exists())
                    {
                        var thresholdUsedMemoryKB = this.Configuration.GetValue<long>("App:HealthChecks:Memory:Process:Used", 0);
                        if (thresholdUsedMemoryKB > 0)
                        {
                            description += $" process memory more than {thresholdUsedMemoryKB} KB;";

                            data.Add("ProcessUsedThresholdPct", System.Math.Round((processMemoryUsedKB / (double)thresholdUsedMemoryKB) * 100, 2));

                            if (processMemoryUsedKB > thresholdUsedMemoryKB)
                            {
                                status = HealthStatus.Unhealthy;
                            }
                        }
                    }

                    // =============================================================================================
                    // MACHINE
                    // =============================================================================================
                    if (machineMemory != null && this.Configuration.GetSection("App:HealthChecks:Memory:Machine:Used").Exists())
                    {
                        var thresholdUsedMemoryKB = this.Configuration.GetValue<long>("App:HealthChecks:Memory:Machine:Used", 0);
                        if (thresholdUsedMemoryKB > 0)
                        {
                            description += $" machine memory more than {thresholdUsedMemoryKB} KB;";

                            data.Add("MachineUsedThresholdPct", System.Math.Round((machineMemory.UsedKB / (double)thresholdUsedMemoryKB) * 100, 2));

                            if (machineMemory.UsedKB > thresholdUsedMemoryKB)
                            {
                                status = HealthStatus.Unhealthy;
                            }
                        }
                    }

                    if (machineMemory != null && this.Configuration.GetSection("App:HealthChecks:Memory:Machine:UsedPercent").Exists())
                    {
                        var thresholdUsedPercent = this.Configuration.GetValue<decimal>("App:HealthChecks:Memory:Machine:UsedPercent", 0);
                        if (thresholdUsedPercent > 0)
                        {
                            description += $" machine memory more than {thresholdUsedPercent} percent;";

                            if (machineMemory.UsedPercent > thresholdUsedPercent)
                            {
                                status = HealthStatus.Unhealthy;
                            }
                        }
                    }

                    if (machineMemory != null && this.Configuration.GetSection("App:HealthChecks:Memory:Machine:Available").Exists())
                    {
                        var thresholdAvailable = this.Configuration.GetValue<long>("App:HealthChecks:Memory:Machine:Available", 0);
                        if (thresholdAvailable > 0)
                        {
                            description += $" machine memory less than {thresholdAvailable} KB available;";

                            data.Add("MachineAvailableThresholdPct", System.Math.Round((machineMemory.AvailableKB / (double)thresholdAvailable) * 100, 2));

                            if (machineMemory.AvailableKB > thresholdAvailable)
                            {
                                status = HealthStatus.Unhealthy;
                            }
                        }
                    }

                    if (machineMemory != null && this.Configuration.GetSection("App:HealthChecks:Memory:Machine:AvailablePercent").Exists())
                    {
                        var thresholdAvailablePercent = this.Configuration.GetValue<decimal>("App:HealthChecks:Memory:Machine:AvailablePercent", 0);
                        if (thresholdAvailablePercent > 0)
                        {
                            description += $" machine memory less than {thresholdAvailablePercent} percent available;";

                            if (machineMemory.AvailablePercent > thresholdAvailablePercent)
                            {
                                status = HealthStatus.Unhealthy;
                            }
                        }
                    }
                }
                else
                {
                    description = " Never (No Checks)";
                }

                return Task.FromResult(new HealthCheckResult
                (
                    status: status,
                    description: description,
                    exception: null,
                    data: data)
                );
            }
            catch (Exception ex)
            {
                return Task.FromResult(new HealthCheckResult
                (
                    status: HealthStatus.Unhealthy,
                    description: ex.Message,
                    exception: ex,
                    data: null)
                );
            }
        }
    }
}
