// ================================================================================
// <copyright file="Assembly.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using System.Reflection;
using System.Runtime.Versioning;
using ThingsLibrary.DataType.Extensions;

namespace ThingsLibrary.DataType.Extensions
{
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Agent String based on Assembly Product Name/Version and OS Version
        /// </summary>
        /// <param name="assembly">Assembly</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string AgentString(this Assembly assembly)
        {
            ArgumentNullException.ThrowIfNull(assembly);

            var name = assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product ?? throw new ArgumentNullException("Assembly Product Name");

            var version = assembly.GetName()?.Version ?? throw new ArgumentNullException("Assembly File Version");

            return $"{name}/{version.ToDotString()} ({Metrics.MachineMetrics.OsVersion()})";
        }

        /// <summary>
        /// Get the ID from the Guid Attribute property  of the assembly is available.. Guid.Empty it not found
        /// </summary>
        /// <param name="assembly">Assembly</param>
        /// <returns>Guid of assembly, Guid.Empty is not found</returns>
        public static Guid GetId(this Assembly assembly)
        {
            // Requires an AssemblyInfo.cs with a [assembly: Guid("11111111-1111-1111-1111-111111111111")] style attribute

            var guid = assembly.GetCustomAttribute<System.Runtime.InteropServices.GuidAttribute>();
            if (guid == null) { return Guid.Empty; }

            return Guid.Parse(guid.Value);
        }

        /// <summary>
        /// Name (same as Title)
        /// </summary>
        /// <param name="assembly">Assembly</param>
        /// <returns></returns>
        public static string Name(this Assembly assembly) => Title(assembly);

        /// <summary>
        /// Title
        /// </summary>
        /// <param name="assembly">Assembly</param>
        /// <returns></returns>
        public static string Title(this Assembly assembly)
        {
            ArgumentNullException.ThrowIfNull(assembly);

            return assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title ?? string.Empty;
        }

        /// <summary>
        /// Namespace
        /// </summary>
        /// <param name="assembly">Assembly</param>
        /// <returns></returns>
        public static string Namespace(this Assembly assembly)
        {
            ArgumentNullException.ThrowIfNull(assembly);
         
            return assembly.GetName()?.Name ?? string.Empty;
        }

        /// <summary>
        /// Version as a string
        /// </summary>
        /// <param name="assembly">Assembly</param>
        /// <returns></returns>
        public static string Version(this Assembly assembly)
        {
            ArgumentNullException.ThrowIfNull(assembly);

            return assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? string.Empty;            
        }

        /// <summary>
        /// File Version as a Version data type
        /// </summary>
        /// <param name="assembly">Assembly</param>
        /// <returns></returns>
        public static Version FileVersion(this Assembly assembly)
        {
            ArgumentNullException.ThrowIfNull(assembly);
            return assembly.GetName()?.Version ?? new System.Version();
        }

        /// <summary>
        /// File version as a string
        /// </summary>
        /// <param name="assembly">Assembly</param>
        /// <returns></returns>
        public static string FileVersionStr(this Assembly assembly)
        {
            var version = assembly.FileVersion();
            if (version == null) { return "0"; }

            return version.ToDotString();
        }

        /// <summary>
        /// File Version as a long data type
        /// </summary>
        /// <param name="assembly">Assembly</param>
        /// <returns></returns>
        public static long FileVersionLong(this Assembly assembly) => assembly.FileVersion().ToLong();

        /// <summary>
        /// Product Name
        /// </summary>
        /// <param name="assembly">Assembly</param>
        /// <returns></returns>
        public static string ProductName(this Assembly assembly)
        {
            ArgumentNullException.ThrowIfNull(assembly);
            return assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product ?? string.Empty;
        }

        public static string Company(this Assembly assembly)
        {
            ArgumentNullException.ThrowIfNull(assembly);
            return assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company ?? string.Empty;
        }

        /// <summary>
        /// Retrieves the description text specified in the assembly's <see cref="AssemblyDescriptionAttribute"/>.
        /// </summary>
        /// <remarks>This method returns the value of the <see cref="AssemblyDescriptionAttribute"/>
        /// applied to the specified assembly. If the attribute is not present, an empty string is returned.</remarks>
        /// <param name="assembly">The assembly from which to obtain the description. Cannot be <see langword="null"/>.</param>
        /// <returns>A string containing the description of the assembly, or an empty string if no description is defined.</returns>
        public static string Description(this Assembly assembly)
        {
            ArgumentNullException.ThrowIfNull(assembly);
            return assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description ?? string.Empty;
        }


        /// <summary>
        /// Copyright
        /// </summary>
        /// <param name="assembly">Assembly</param>
        /// <returns></returns>
        public static string Copyright(this Assembly assembly)
        {
            ArgumentNullException.ThrowIfNull(assembly);
            return assembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright ?? string.Empty;
        }

        /// <summary>
        /// DotNet Framework Version
        /// </summary>
        /// <param name="assembly">Assembly</param>
        /// <returns></returns>
        public static string NetFrameworkVersion(this Assembly assembly)
        {
            ArgumentNullException.ThrowIfNull(assembly);
            return assembly.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName ?? string.Empty;
        }

        /// <summary>
        /// Assembly creation date
        /// </summary>
        /// <param name="assembly">Assembly</param>
        /// <returns></returns>
        public static DateTime CreatedOn(this Assembly assembly)
        {
            ArgumentNullException.ThrowIfNull(assembly);
            return File.GetCreationTimeUtc(assembly.Location);
        }

        /// <summary>
        /// Assembly last written / updated date
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static DateTime UpdatedOn(this Assembly assembly)
        {
            ArgumentNullException.ThrowIfNull(assembly);
            return File.GetLastWriteTimeUtc(assembly.Location);
        }

        /// <summary>
        /// Gets the file system path of the specified assembly.
        /// </summary>
        /// <remarks>This method returns the value of the <see cref="Assembly.Location"/> property. If the
        /// assembly was loaded from a byte array rather than a file, the returned path will be an empty
        /// string.</remarks>
        /// <param name="assembly">The assembly for which to retrieve the file system path. Cannot be null.</param>
        /// <returns>The full path to the file that contains the assembly. Returns an empty string if the assembly was loaded
        /// from a byte array.</returns>
        public static string Path(this Assembly assembly)
        {
            ArgumentNullException.ThrowIfNull(assembly);
            return assembly.Location;
        }

        /// <summary>
        /// Directory of the executing assembly
        /// </summary>
        /// <param name="assembly">Assembly</param>
        /// <returns></returns>
        public static string DirectoryPath(this Assembly assembly)
        {
            ArgumentNullException.ThrowIfNull(assembly);
            return System.IO.Path.GetDirectoryName(assembly.Location) ?? string.Empty;
        }

        /// <summary>
        /// Application Data Path
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static string AppDataPath(this Assembly assembly)
        {
            ArgumentNullException.ThrowIfNull(assembly);
            var appDataPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), assembly.ProductName()); 
            
            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }

            return appDataPath;
        }

        /// <summary>
        /// Temp directory path (temp path + product name)
        /// </summary>
        /// <param name="assembly">Assembly</param>
        /// <returns></returns>
        public static string TempDirectoryPath(this Assembly assembly)
        {
            ArgumentNullException.ThrowIfNull(assembly);

            var tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), assembly.ProductName());
            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }
            return tempPath;
        }

        /// <summary>
        /// Dependency Versions
        /// </summary>
        /// <param name="assembly">Assembly</param>
        /// <returns></returns>
        public static IDictionary<string, string> DependencyVersions(this Assembly assembly)
        {
            ArgumentNullException.ThrowIfNull(assembly);
                        
            var references = assembly.GetReferencedAssemblies();
            
            var dict = new Dictionary<string, string>();
            foreach (var reference in references)
            {
                if (string.IsNullOrEmpty(reference.Name)) { continue; } // todo?  not sure what should happen here.

                dict[reference.Name] = reference.Version?.ToString() ?? string.Empty;
            }
            return dict;
        }

    }
}
