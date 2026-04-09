// ================================================================================
// <copyright file="TestEnvironmentOptions.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Testing.Environment
{
    /// <summary>
    /// Various elements of what is the makeup of the test environment
    /// </summary>
    public class TestEnvironmentOptions
    {
        /// <summary>
        /// Connection String Variable to use
        /// </summary>
        public string ConnectionStringVariable { get; init; } = string.Empty;

        /// <summary>
        /// Test Container Definition
        /// </summary>
        public TestContainerOptions? TestContainer { get; init; }

        /// <summary>
        /// If test container should be used/started
        /// </summary>
        public bool UseExistingContainer { get; init; } = false;
    }
}
