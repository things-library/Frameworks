// ================================================================================
// <copyright file="IId.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Interfaces
{
    /// <summary>
    /// Standard Interface to force a standard on ID in a class
    /// </summary>
    public interface IId
    {
        /// <summary>
        /// ID Field
        /// </summary>        
        public Guid Id { get; }
    }

    /// <summary>
    /// Standard Interface to force a standard on ID in a class
    /// </summary>
    public interface IIdShort
    {
        /// <summary>
        /// ID Field
        /// </summary>        
        public short Id { get; }
    }
}
