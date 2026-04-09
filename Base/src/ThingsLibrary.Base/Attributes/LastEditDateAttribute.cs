// ================================================================================
// <copyright file="LastEditDateAttribute.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Attributes
{
    /// <summary>
    /// Specifies which class property is the edit timestamp key
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class LastEditDateAttribute : Attribute
    {
        /// <summary>
        /// Last Record Edit Date Timestamp
        /// </summary>
        public LastEditDateAttribute() { }
    }
}