// ================================================================================
// <copyright file="AssemblyMetrics.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using ThingsLibrary.DataType.Extensions;
using System.Reflection;
using System.Runtime.Versioning;

namespace ThingsLibrary.Metrics
{
    /// <summary>
    /// Assembly details
    /// </summary>
    public class AssemblyMetrics
    {
        /// <summary>
        /// Static Instance
        /// </summary>
        public static AssemblyMetrics Instance { get; set; } = new AssemblyMetrics();
        
        /// <summary>
        /// Referring Assembly
        /// </summary>
        public Assembly Assembly { get; private set; }

        /// <summary>
        /// Assembly based ID
        /// </summary>
        /// <returns>Assembly ID</returns>
        /// <remarks>
        /// Requires an AssemblyInfo.cs with a [assembly: Guid("11111111-1111-1111-1111-111111111111")] style attribute
        /// </remarks>
        public Guid Id() => AssemblyMetrics.GetId(this.Assembly);
    
        /// <summary>
        /// Name
        /// </summary>
        /// <returns></returns>
        public string Name() => this.Assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title ?? string.Empty;

        /// <summary>
        /// Namespace
        /// </summary>
        /// <returns></returns>
        public string Namespace() => this.Assembly.GetName()?.Name ?? string.Empty;

        /// <summary>
        /// App Agent String
        /// </summary>
        /// <returns>In agent string notation, RFC7231, {Assembly Namespace}/{Version}</returns>
        public string AgentString() => $"{this.Namespace()}/{this.Version()}";

        /// <summary>
        /// Version
        /// </summary>
        /// <returns></returns>
        public string Version() => this.Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? string.Empty;

        /// <summary>
        /// File Version
        /// </summary>
        /// <returns></returns>
        public Version FileVersion() => this.Assembly.GetName()?.Version ?? new System.Version();

        /// <summary>
        /// Get all the dependencies and their file version
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> DependencyVersions() => AssemblyMetrics.GetDependencyVersions(this.Assembly);

        /// <summary>
        /// File version as a string
        /// </summary>
        /// <returns></returns>
        public string FileVersionStr()
        {
            var version = this.FileVersion();   
            if(version == null) { return "0"; }

            return version.ToDotString();
        }

        /// <summary>
        /// File Version as a long data type
        /// </summary>
        /// <returns></returns>
        public long FileVersionLong() => this.FileVersion().ToLong();

        /// <summary>
        /// Product Name
        /// </summary>
        /// <returns></returns>
        public string ProductName() => this.Assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product ?? string.Empty;

        /// <summary>
        /// Title
        /// </summary>
        /// <returns></returns>
        public string Title() => this.Assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title ?? string.Empty;


        /// <summary>
        /// Company
        /// </summary>
        /// <returns></returns>
        public string Company() => this.Assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company ?? string.Empty;
               
        /// <summary>
        /// Description
        /// </summary>
        /// <returns></returns>
        public string Description() => this.Assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description ?? string.Empty;

        /// <summary>
        /// Copyright
        /// </summary>
        /// <returns></returns>
        public string Copyright() => this.Assembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright ?? string.Empty;
                
        /// <summary>
        /// .Net Framework Version
        /// </summary>
        /// <returns></returns>
        public string NetFrameworkVersion() => this.Assembly.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName ?? string.Empty;

        /// <summary>
        /// Date Created
        /// </summary>
        /// <returns></returns>
        public DateTime CreatedOn() => File.GetCreationTime(this.Assembly.Location).ToUniversalTime();

        /// <summary>
        /// File path to the executing assembly
        /// </summary>
        /// <returns></returns>
        public string Path() => this.Assembly.Location;

        /// <summary>
        /// Directory of the executing assemblt
        /// </summary>
        /// <returns></returns>
        public string DirectoryPath() => System.IO.Path.GetDirectoryName(this.Path()) ?? throw new IOException($"Unable to find directory for path: {this.Path}");

        /// <summary>
        /// App Data Path
        /// </summary>
        /// <returns></returns>
        public string AppDataPath() => System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), this.Name());

        /// <summary>
        /// Temp Directory Path
        /// </summary>
        /// <returns></returns>
        public string TempDirectoryPath() => System.IO.Path.Combine(System.IO.Path.GetTempPath(), this.Namespace());

        /// <summary>
        /// Assembly Metrics Constructor
        /// </summary>
        public AssemblyMetrics()
        {
            this.Assembly = Assembly.GetEntryAssembly() ?? throw new IOException("Unable to locate entry assembly.");
        }

        /// <summary>
        /// Assembly Metrics Constructor
        /// </summary>
        /// <param name="assembly"></param>
        public AssemblyMetrics(Assembly assembly)
        {
            this.Assembly = assembly;            
        }


        #region --- Static Methods ---

        /// <summary>
        /// Get the ID from the Guid Attribute property  of the assembly is available.. Guid.Empty it not found
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns>Guid of assembly, Guid.Empty is not found</returns>
        public static Guid GetId(Assembly assembly)
        {
            // Requires an AssemblyInfo.cs with a [assembly: Guid("11111111-1111-1111-1111-111111111111")] style attribute

            var guid = assembly.GetCustomAttribute<System.Runtime.InteropServices.GuidAttribute>();
            if (guid == null) { return Guid.Empty; }

            return Guid.Parse(guid.Value);
        }

        /// <summary>
        /// Get a list of all the dependencies referenced by the assembly and their file version
        /// </summary>
        /// <param name="assembly">Assembly to get dependencies</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetDependencyVersions(Assembly assembly)
        {            
            return assembly.GetReferencedAssemblies().ToDictionary(x => x.Name!, x => x.Version?.ToString() ?? string.Empty);
        }

        #endregion
    }
}
