// ================================================================================
// <copyright file="AppService.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using ThingsLibrary.Metrics;

namespace ThingsLibrary.Services
{
    public static class App
    {
        /// <summary>
        /// AppService Details but static 
        /// </summary>
        public static readonly AppService Service = new AppService();
    }

    /// <summary>
    /// Static App service instance with app related items
    /// </summary>
    public class AppService
    {
        /// <summary>
        /// Persistent Unique App Id
        /// </summary>
        public Guid InstanceId { get; init; }

        /// <summary>
        /// Started DateTime
        /// </summary>
        public DateTimeOffset StartedOn { get; } = DateTimeOffset.Now;
        
        /// <summary>
        /// Cancellation Token Source
        /// </summary>
        public CancellationTokenSource CancellationTokenSource { get; } = new CancellationTokenSource();

        /// <summary>
        /// Cancellation Token
        /// </summary>
        public CancellationToken CancellationToken => this.CancellationTokenSource.Token;

        /// <summary>
        /// Service Canvas
        /// </summary>
        public Schema.Canvas.CanvasRoot? Canvas { get; set; }

        /// <summary>
        /// Debugging / Development State
        /// </summary>
        /// <remarks>Returns true if debugger is attached and not production</remarks>
        public bool IsDebug
        {
            get
            {
                if (this.IsProduction()) { return false; }

                return System.Diagnostics.Debugger.IsAttached;
            }
        }

        /// <summary>
        /// Flag to indicate if this instance is ready for requests
        /// </summary>
        public bool IsReady { get; set; }

        /// <summary>
        /// If The app is running in a container
        /// </summary>
        public bool IsContainer => (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true");

        /// <summary>
        /// Constructor
        /// </summary>
        public AppService()
        {
            // each container gets its own unique id so we can keep track internally and externally
            this.InstanceId = this.GetOrCreateInstanceId();           
        }
        
        #region --- Environment ---   

        public Uri? HostUri { get; set; }

        public string EnvironmentName { get; set; } = "Production"; //unless otherwise specified as production will be the most locked down

        public bool IsEnvironment(string environmentName) => string.Equals(this.EnvironmentName, environmentName, StringComparison.OrdinalIgnoreCase);
        
        public bool IsProduction() => this.IsEnvironment("Production");

        public bool IsDevelopment() => this.IsEnvironment("Development");

        #endregion

        #region --- Assembly Properties ---

        private AssemblyMetrics? _assemblyMetrics;

        public AssemblyMetrics Assembly
        {
            get
            {
                // only allociate the memory if we are using it
                if (_assemblyMetrics == null) { _assemblyMetrics = new AssemblyMetrics(); }

                return _assemblyMetrics;
            }

            set { _assemblyMetrics = value; }
        }

        /// <summary>
        /// App Namespace ID
        /// </summary>
        public string Id => this.Assembly.Namespace();

        /// <summary>
        /// Product Name        
        /// </summary>
        public string Name => this.Assembly.ProductName();

        /// <summary>
        /// File Version Long
        /// </summary>
        public long VersionLong => this.Assembly.FileVersionLong();

        /// <summary>
        /// File Version String
        /// </summary>
        public string Version => this.Assembly.FileVersionStr();

        #endregion

        #region --- Service Metrics ---
                
        /// <summary>
        /// Instance metrics
        /// </summary>
        public RootItemDto Metrics
        {
            get => this.GetMetrics();            
        }
        private RootItemDto? _metrics;

        public RootItemDto GetMetrics(int ttl = 30)
        {
            // limit the speed to which we can call memory snapshots            
            if (_metrics != null)
            {
                var lastRefreshOn = _metrics.Date ?? default;
                if (_metrics.Date > DateTimeOffset.Now.Subtract(TimeSpan.FromSeconds(ttl)))
                {
                    _metrics.Tags["CacheTTL"] = DateTimeOffset.UtcNow.Subtract(lastRefreshOn.AddSeconds(ttl)).ToClock();

                    return _metrics;
                }
            }

            var rootMetrics = new RootItemDto
            {
                Key = $"{this.InstanceId}",
                Type = "instance",
                Name = this.InstanceId.ToString(),                

                Tags = new Dictionary<string, string>
                {
                    // APP Metrics
                    { "instance_id", $"{this.InstanceId}" },
                    { "app_name", this.Name },
                    { "app_containerized", $"{this.IsContainer}" },

                    // Host Environment                
                    { "debugger", $"{this.IsDebug}" },
                    { "dotnet_framework", this.Assembly.NetFrameworkVersion() },
                }
            };            

            //assembly metrics
            var assemblyItem = new RootItemDto
            {
                Key = "assembly",
                Type = "assembly",
                Name = this.Assembly.Name(),                

                Tags = new Dictionary<string, string>()
                {
                    { "name", this.Assembly.Name()},
                    { "namespace", this.Assembly.Namespace()},
                    { "version", this.Assembly.Version()},
                    { "file_version", this.Assembly.FileVersionStr()},
                    { "date", $"{this.Assembly.CreatedOn()} ({this.Assembly.CreatedOn().ToHHMMSS(true)})"},
                    { "product", this.Assembly.ProductName()},
                    { "company", this.Assembly.Company()},
                    { "description", this.Assembly.Description()},
                    { "copyright", this.Assembly.Copyright()},
                    { "path", this.Assembly.Path()},
                    { "data_path", this.Assembly.AppDataPath()},
                    { "temp_path", this.Assembly.TempDirectoryPath()}
                }
            };
            rootMetrics.Items.Add(assemblyItem.Key, assemblyItem);

            // machine metrics
            var machineItem = new RootItemDto
            {
                Key = "machine",
                Type = "machine",
                Name = MachineMetrics.MachineName(),

                Tags = new Dictionary<string, string>()
                {
                    { "name", MachineMetrics.MachineName() },
                    { "cpu_count", $"{MachineMetrics.CpuCount()}" },
                    { "os_version", MachineMetrics.OsVersion() },
                    { "ip_address", string.Join(";", MachineMetrics.LocalIPAddresses()) },
                    { "mac_address", string.Join(";", MachineMetrics.MacAddresses()) },
                    { "timezone", TimeZoneInfo.Local.DisplayName }
                }
            };            
            rootMetrics.Items.Add(machineItem.Key, machineItem);

            _metrics = rootMetrics;

            return _metrics;
        }

        #endregion

        #region --- Heartbeat Metrics ---
        
        /// <summary>
        /// Heartbeat related metrics
        /// </summary>
        public RootItemDto LastHeartbeat
        {
            get => this.GetHeartbeatMetrics();
        }
        private RootItemDto? _lastHeartbeat;

        /// <summary>
        /// Gets constantly changing metrics about the application
        /// </summary>
        /// <param name="ttl">Time to live (seconds)</param>
        /// <returns></returns>
        public RootItemDto GetHeartbeatMetrics(int ttl = 30)
        {
            // limit the speed to which we can call memory snapshots            
            if (_lastHeartbeat != null)
            {
                var lastRefreshOn = _lastHeartbeat.Date ?? DateTimeOffset.Now;
                if (lastRefreshOn > DateTimeOffset.Now.Subtract(TimeSpan.FromSeconds(ttl)))
                {
                    _lastHeartbeat.Tags["cache_ttl"] = DateTimeOffset.UtcNow.Subtract(lastRefreshOn.AddSeconds(ttl)).ToClock();

                    return _lastHeartbeat;
                }
            }                  

            // get new heartbeat
            var memory = MemoryMetrics.GetSnapshot();

            var dateTime = DateTimeOffset.UtcNow;

            // since we are going to the trouble keep track of it
            var memoryItem = new RootItemDto
            {
                Key = $"hb_{dateTime.ToUnixTimeSeconds()}",

                Type = "instance_hb",
                Name = "Instance Heartbeat",
                Date = DateTime.Now,

                Tags = new Dictionary<string, string>()
                {       
                    // machine metrics
                    { "memory_process_used", $"{MachineMetrics.ProcessUsedMemoryKB}"},
                    { "harddrive_available", $"{MachineMetrics.GetAvailableHardDriveSpaceKB()}" },
                    
                    // memory metrics
                    { "memory_available", $"{memory?.AvailableKB}" },
                    { "memory_available_percent", $"{(memory != null ? System.Math.Round(memory.AvailablePercent * 100, 1) : "")}"},
                    { "memory_used", $"{memory?.UsedKB}"},
                    { "memory_used_percent", $"{(memory != null ? System.Math.Round(memory.UsedPercent * 100, 1) : "")}"}
                }
            };

            memoryItem.Tags["cache_ttl"] = DateTimeOffset.UtcNow.Subtract(memoryItem.Date.Value.AddSeconds(ttl)).ToClock();

            _lastHeartbeat = memoryItem;

            return _lastHeartbeat;
        }

        #endregion

        /// <summary>
        /// Agent String that explains the application and its version
        /// </summary>
        /// <returns></returns>
        public string AgentString()
        {
            return $"{this.Name}/{this.Version} ({this.Id}; {MachineMetrics.OsVersion()})";
        }

        /// <summary>
        /// Get the instance status details
        /// </summary>
        /// <returns></returns>
        public RootItemDto GetInstanceStatus()
        {
            var itemDto = App.Service.GetMetrics();

            itemDto.Items.Add("instance_hb", App.Service.GetHeartbeatMetrics());

            itemDto.Meta["TimeNow"] = DateTimeOffset.Now.ToString("O");
            itemDto.Meta["TimeUtcNow"] = DateTimeOffset.UtcNow.ToString("O");

            return itemDto;
        }

        ///// <summary>
        ///// Get the whoami details from the claims principal
        ///// </summary>
        ///// <param name="claimsPrincipal"></param>
        ///// <returns></returns>
        ///// <exception cref="ArgumentNullException"></exception>
        //public Contracts.WhoamiDto GetWhoami(ClaimsPrincipal claimsPrincipal)
        //{
        //    if (claimsPrincipal == null) { throw new ArgumentNullException(nameof(claimsPrincipal)); }

        //    // figure out if the user is in the app role
        //    var appRoles = new Dictionary<string, bool>();
        //    var appPolicies = new Dictionary<string, bool>();

        //    // figure out the app role membership
        //    if (App.Service?.Canvas?.Auth?.AppRoles != null)
        //    {
        //        App.Service.Canvas.Auth.AppRoles.ForEach(appRole => appRoles[appRole] = claimsPrincipal.IsInRole(appRole));
        //    }

        //    // figure out if the user is in the app policy
        //    if (App.Service?.Canvas?.Auth?.PolicyClaimsMap != null)
        //    {
        //        App.Service.Canvas.Auth.PolicyClaimsMap.ForEach(appPolicy => appPolicies[appPolicy.Key] = claimsPrincipal.IsInPolicy(appPolicy.Key));
        //    }

        //    var claims = new Dictionary<string, string>();
        //    foreach (Claim claim in claimsPrincipal.Claims)
        //    {
        //        if (claims.ContainsKey(claim.Type))
        //        {
        //            claims[claim.Type] = $"{claims[claim.Type]};{claim.Value}";
        //        }
        //        else
        //        {
        //            claims[claim.Type] = claim.Value;
        //        }
        //    }

        //    return new Contracts.WhoamiDto
        //    {                
        //        Oid = claimsPrincipal.GetClaim("oid"),
        //        Upn = claimsPrincipal.GetClaim("upn", "preferred_username"),
        //        Username = claimsPrincipal.GetClaim("preferred_username", "upn", "username", "email", "unique_name"),
        //        Name = claimsPrincipal.GetClaim("name"),
        //        IPAddress = claimsPrincipal.GetClaim("ipaddr"),
        //        Email = claimsPrincipal.GetClaim("email"),
        //        Roles = claimsPrincipal.GetClaims("roles").OrderBy(x => x).ToList(),
        //        Claims = claims.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value),

        //        AppRoles = appRoles,
        //        AppPolicies = appPolicies
        //    };
        //}


        /// <summary>
        /// Get a unique ID for this machine instance or create it and persist it.
        /// </summary>
        /// <returns>Unique Guid</returns>
        private Guid GetOrCreateInstanceId()
        {
            try
            {
                // create a linux/windows friendly file path
                var filePath = Path.Combine(this.Assembly.AppDataPath(), "id.json");

                Guid id;
                
                if (File.Exists(filePath))
                {
                    var doc = JsonDocument.Parse(File.ReadAllText(filePath));

                    id = doc.GetProperty<Guid>("id", Guid.Empty);
                    if(id != Guid.Empty)
                    {
                        Log.Information("= Instance Id: {AppInstanceId} (found)", id);
                        return id;
                    }
                }

                id = SequentialGuid.NewGuid();
                var fileData = JsonSerializer.Serialize(new { id = id });

                var folderPath = Path.GetDirectoryName(filePath) ?? throw new ArgumentException($"Unable to get directory path from '{filePath}'");

                Log.Information("= Instance Id: {AppInstanceId} (creating)", id);
                IO.Directory.VerifyPath(folderPath);

                // save out the file                 
                File.WriteAllText(filePath, fileData);

                return id;
            }
            catch (Exception ex)
            {
                Log.Warning("Unable to create persistent instance Id.");
                Log.Warning(ex, ex.Message);

                return Guid.NewGuid();
            }            
        }
    }
}
