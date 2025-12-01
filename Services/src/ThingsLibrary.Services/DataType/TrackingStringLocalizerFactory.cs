// ================================================================================
// <copyright file="TrackingStringLocalizerFactory.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using Microsoft.Extensions.Localization;
using System.Reflection;

namespace ThingsLibrary.Services.DataType
{
    /// <summary>
    /// Factory that creates TrackingLocalizer instances for dependency injection
    /// </summary>
    public class TrackingStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly IStringLocalizerFactory _innerFactory;
        private readonly string? _resourceDirectoryPath;

        public TrackingStringLocalizerFactory(IStringLocalizerFactory innerFactory, string? resourceDirectoryPath = null)
        {
            _innerFactory = innerFactory ?? throw new ArgumentNullException(nameof(innerFactory));
            _resourceDirectoryPath = resourceDirectoryPath;
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            return new TrackingStringLocalizer(_innerFactory, resourceSource.Assembly, resourceSource.FullName ?? "", _resourceDirectoryPath);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            // For non-generic cases, we don't have a specific type context
            var assembly = Assembly.Load(location);

            return new TrackingStringLocalizer(_innerFactory, assembly, baseName, _resourceDirectoryPath);
        }
    }
}