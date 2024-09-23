// ================================================================================
// <copyright file="MemoryMetrics.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Metrics
{
    /// <summary>
    /// Memory Details (linux get requires procps library installed for the 'free -m' option)
    /// </summary>
    public class MemoryMetrics
    {
        /// <summary>
        /// Total Memory (KB)
        /// </summary>
        public long TotalKB { get; private set; }

        /// <summary>
        /// Available Memory (KB)
        /// </summary>
        public long AvailableKB { get; private set; }

        /// <summary>
        /// Used Memory (KB)
        /// </summary>
        public long UsedKB { get; private set; }

        /// <summary>
        /// Available Memory as percent of total
        /// </summary>
        // calculated        
        public decimal AvailablePercent => (this.TotalKB > 0 ? this.AvailableKB / (decimal)this.TotalKB : 0);

        /// <summary>
        /// Used memory as a percent of total
        /// </summary>
        public decimal UsedPercent => (this.TotalKB > 0 ? this.UsedKB / (decimal)this.TotalKB : 0);

        /// <summary>
        /// Return a snapshot of the memory data
        /// </summary>
        /// <returns><see cref="MemoryMetrics"</returns>
        /// <exception cref="NotImplementedException"></exception>
        public static MemoryMetrics? GetSnapshot()
        {
            if (MachineMetrics.IsWindows())
            {
                return MemoryMetrics.GetWindowsMetrics();
            }
            else if (MachineMetrics.IsUnix())
            {
                return MemoryMetrics.GetUnixMetrics();
            }
            else 
            {
                throw new NotImplementedException("Unknown OS Type.");
            }
        }

        private static MemoryMetrics? GetWindowsMetrics()
        {
            var wmiMetrics = GetWindowsMetrics("OS", "FreePhysicalMemory,TotalVisibleMemorySize");
            if(wmiMetrics == null) { return null; }

            var metrics = new MemoryMetrics();
            if (wmiMetrics.ContainsKey("FreePhysicalMemory"))
            {
                metrics.AvailableKB = long.Parse(wmiMetrics["FreePhysicalMemory"]);         //measurement is in kilobytes)
            }

            if (wmiMetrics.ContainsKey("TotalVisibleMemorySize"))
            {
                metrics.TotalKB = long.Parse(wmiMetrics["TotalVisibleMemorySize"]);    //measurement is in kilobytes
            }

            metrics.UsedKB = (metrics.TotalKB - metrics.AvailableKB);

            return metrics;
        }

        private static Dictionary<string, string> GetWindowsMetrics(string type, string arguments)
        {
            try
            {
                var info = new ProcessStartInfo();
                info.FileName = "wmic";
                info.Arguments = $"{type} get {arguments} /Value";
                info.RedirectStandardOutput = true;

                using(var process = Process.Start(info))
                {
                    if (process is null) { return []; }

                    return process?.StandardOutput.ReadToEnd()
                        .Trim()
                        .Split('\n')
                        .Select(x => x.Split('='))
                        .ToDictionary(x => x[0], x => x[1])!;
                }
            }
            catch
            { 
                return new Dictionary<string, string>();
            }
        }

        private static MemoryMetrics? GetUnixMetrics()
        {
            try
            {
                var info = new ProcessStartInfo("free -m");
                info.FileName = "/bin/bash";
                info.Arguments = "-c \"free -m\"";
                info.RedirectStandardOutput = true;

                var output = "";
                using (var process = Process.Start(info))
                {
                    if(process == null) { return null; }

                    output = process.StandardOutput.ReadToEnd();                    
                }
                if (string.IsNullOrEmpty(output)) { return null; }

                var lines = output.Split("\n");
                var memory = lines[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);

                return new MemoryMetrics
                {
                    TotalKB = long.Parse(memory[1]),
                    UsedKB = long.Parse(memory[2]),
                    AvailableKB = long.Parse(memory[3])
                };            
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());

                return null;
            }
        }
    }
}
