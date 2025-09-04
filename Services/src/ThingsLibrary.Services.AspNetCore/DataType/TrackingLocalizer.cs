// ================================================================================
// <copyright file="TrackingLocalizer.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using System.Xml.Linq;
using Microsoft.Extensions.Localization;

using Serilog;

namespace ThingsLibrary.Services.AspNetCore.DataType
{
    /// <summary>
    /// Localizer that also keeps track of usage and auto creates the missing terms
    /// </summary>
    public class TrackingLocalizer : IStringLocalizer
    {
        private static readonly string ResourceName = "SharedResource";
        private static readonly object ResourceFileLock = new();

        private IStringLocalizerFactory LocalizerFactory { get; set; }
        private string? SourceNamespace { get; set; }
        private string? ResourceFilePath { get; set; }

        private IStringLocalizer StringLocalizer { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="localizerFactory"></param>
        /// <param name="sourceNamespace"></param>
        public TrackingLocalizer(IStringLocalizerFactory localizerFactory, string? sourceNamespace)
        {
            this.LocalizerFactory = localizerFactory;
            this.SourceNamespace = sourceNamespace;

            // DO NOT DO THIS ON PRODUCTION AS THERE WILL NOT BE DEV ARTIFACTS
            if (!App.Service.IsProduction())
            {
                this.ResourceFilePath = Path.Combine(App.Service.Assembly.DirectoryPath(), "..", "..", "..", "SharedResource.resx");
            }           

            // load the resource file
            this.StringLocalizer = this.LoadResourceFile();
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

        private IStringLocalizer LoadResourceFile()
        {
            // Reload the localizer
            return this.LocalizerFactory.Create(TrackingLocalizer.ResourceName, App.Service.Assembly.Name());
        }

        private void AppendResourceFile(string key, string value)
        {
            // are we production?
            if (string.IsNullOrEmpty(this.ResourceFilePath)) { return; }

            // does the file even exist?  we can't create it from scratch
            if (!File.Exists(this.ResourceFilePath))
            {
                Log.Error($"Unable to find resource file: {this.ResourceFilePath}");
                return;
            }

            try
            {
                lock (TrackingLocalizer.ResourceFileLock)
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
                Log.Error(ex, ex.Message);
                Log.Error($"Error writing term '{key}:{value}' to resource file: {this.ResourceFilePath}");
            }            
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            throw new NotImplementedException();
        }
    }
}
