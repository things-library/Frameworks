// ================================================================================
// <copyright file="Assembly.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using System.Reflection;
using ThingsLibrary.DataType.Extensions;

namespace ThingsLibrary.Base.DataType.Extensions
{
    public static class AssemblyExtensions
    {
        public static string AgentString(this Assembly assembly)
        {
            ArgumentNullException.ThrowIfNull(assembly);

            var name = assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product ?? throw new ArgumentNullException("Assembly Product Name");

            var version = assembly.GetName()?.Version ?? throw new ArgumentNullException("Assembly File Version");

            return $"{name}/{version.ToDotString()} ({Metrics.MachineMetrics.OsVersion()})";
        }
    }
}
