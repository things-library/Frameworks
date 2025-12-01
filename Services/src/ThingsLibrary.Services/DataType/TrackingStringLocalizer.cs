// ================================================================================
// <copyright file="TrackingStringLocalizer.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using System.Reflection;
using System.Xml.Linq;
using Microsoft.Extensions.Localization;

namespace ThingsLibrary.Services.DataType
{
    public static class TrackingLocalizerExtensions
    {
        public static IStringLocalizer GetTracked<T>(this IStringLocalizerFactory factory, string? sourceResourceFilePath = null) where T : class
        {
            return factory.GetTracked(typeof(T), sourceResourceFilePath);
        }

        public static IStringLocalizer GetTracked(this IStringLocalizerFactory factory, Type type, string? sourceResourceFilePath = null)
        {
            return new TrackingStringLocalizer(factory, type.Assembly, type.FullName ?? "", sourceResourceFilePath);
        }
    }

    /// <summary>
    /// Localizer that also keeps track of usage and auto creates the missing terms to aid in localization efforts
    /// </summary>
    public class TrackingStringLocalizer : IStringLocalizer
    {           
        private static readonly object ResourceFileLock = new();

        private IStringLocalizer StringLocalizer { get; set; }
                
        private string SourceNamespace { get; set; }
        private string? ResourceFilePath { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="localizerFactory">Localizer Factory</param>
        /// <param name="sourceNamespace">Context for where the term is used</param>
        /// <param name="autoUpdate">If new terms should automatically be added to resource file</param>
        public TrackingStringLocalizer(IStringLocalizerFactory localizerFactory, Assembly resourceAssembly, string sourceNamespace, string? resourceDirectoryPath = null)
        {
            ArgumentNullException.ThrowIfNull(localizerFactory);
            ArgumentNullException.ThrowIfNull(resourceAssembly);

            var assemblyNamespace = resourceAssembly.Namespace();
            
            this.StringLocalizer = localizerFactory.Create($"Resources.{assemblyNamespace}", assemblyNamespace);

            // load the resource file            
            this.SourceNamespace = sourceNamespace; // for tracking missing terms

            if (!string.IsNullOrEmpty(resourceDirectoryPath))
            {
                this.ResourceFilePath = Path.Combine(resourceDirectoryPath, $"{assemblyNamespace}.resx");
                if (!File.Exists(this.ResourceFilePath))
                {
                    Log.Warning("+ New Resource File {ResourceFilePath}", this.ResourceFilePath);
                    this.CreateResourceFile();
                }
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="localizerFactory">Localizer Factory</param>
        /// <param name="type">Location of the embedded shared resources</param>
        /// <param name="sourceResourceFilePath">Resource file to populate with missing terms</param>
        public TrackingStringLocalizer(IStringLocalizerFactory localizerFactory, Type type, string? resourceDirectoryPath = null)
        {
            ArgumentNullException.ThrowIfNull(localizerFactory);
            ArgumentNullException.ThrowIfNull(type);

            var assemblyNamespace = type.Assembly.Namespace();
            this.StringLocalizer = localizerFactory.Create($"Resources.{assemblyNamespace}", assemblyNamespace);

            // load the resource file            
            this.SourceNamespace = type.Namespace!;
            
            if (!string.IsNullOrEmpty(resourceDirectoryPath))
            {
                this.ResourceFilePath = Path.Combine(resourceDirectoryPath, $"{assemblyNamespace}.resx");
                if (!File.Exists(this.ResourceFilePath))
                {
                    Log.Warning("+ New Resource File {ResourceFilePath}", this.ResourceFilePath);
                    this.CreateResourceFile();
                }
            }
        }

        public LocalizedString this[string value]
        {
            get
            {
                var localizedValue = this.StringLocalizer[value];

                // TRACKING COMPONENT
                if (localizedValue.ResourceNotFound)
                {
                    this.AppendResourceFile(value, value);
                }
                
                return localizedValue;
            }
        }

        public LocalizedString this[string value, params object[] arguments]
        {
            get
            {
                var localizedValue = this.StringLocalizer[value, arguments];

                // TRACKING COMPONENT
                if (localizedValue.ResourceNotFound)
                {
                    this.AppendResourceFile(value, value);
                }
                
                return localizedValue;
            }
        }
                
        private void CreateResourceFile()
        {
            if (string.IsNullOrEmpty(this.ResourceFilePath)) { return; }

            try
            {
                // Get the template resource file from embedded resources
                var assembly = typeof(TrackingStringLocalizer).Assembly;
                var resourceName = $"{assembly.GetName().Name}.Templates.Resources.xml";
                
                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null)
                {
                    Log.Warning("Embedded resource template not found: {ResourceName}", resourceName);
                    return;
                }

                // Ensure the directory exists
                var directory = Path.GetDirectoryName(this.ResourceFilePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Save to the specified path
                using var fileStream = new FileStream(this.ResourceFilePath, FileMode.Create, FileAccess.Write);
                stream.CopyTo(fileStream);
                
                Log.Information("Resource file created at: {ResourceFilePath}", this.ResourceFilePath);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "{ErrorMessage}, Resource File Path: {ResourceFilePath}", ex.Message, this.ResourceFilePath);
            }            
        }

        private void AppendResourceFile(string key, string value)
        {
            // are we production?
            if (string.IsNullOrEmpty(this.ResourceFilePath)) { return; }

            // does the file even exist?  we can't create it from scratch
            if (!File.Exists(this.ResourceFilePath)) { return; }

            try
            {
                lock (TrackingStringLocalizer.ResourceFileLock)
                {
                    var doc = XDocument.Load(this.ResourceFilePath);
                    var root = doc.Element("root");
                    if (root != null)
                    {
                        // Check if key exists
                        bool exists = root.Elements("data").Any(e => e.Attribute("name")?.Value == key);
                        if (!exists)
                        {
                            var dataElem = new XElement("data",
                                new XAttribute("name", key),
                                new XAttribute(XNamespace.Xml + "space", "preserve"),
                                new XElement("value", value),
                                new XElement("comment", this.SourceNamespace)
                            );
                            root.Add(dataElem);

                            doc.Save(this.ResourceFilePath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "{ErrorMessage}, Resource File Path: {ResourceFilePath}", ex.Message, this.ResourceFilePath);
            }            
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            throw new NotImplementedException();
        }
    }
}
