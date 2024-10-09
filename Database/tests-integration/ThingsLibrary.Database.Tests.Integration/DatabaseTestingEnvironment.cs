// ================================================================================
// <copyright file="DatabaseTestingEnvironment.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using System.Diagnostics.CodeAnalysis;
using ThingsLibrary.Testing.Environment;

namespace ThingsLibrary.Database.Tests.Integration
{
    [ExcludeFromCodeCoverage]
    public class DatabaseTestingEnvironment : TestEnvironment
    {        
        public DataContext? DB { get; set; }

        public DatabaseTestingEnvironment() : base()
        {            
            // if we have no connection string we have nothing to test
            if (string.IsNullOrWhiteSpace(this.ConnectionString))
            {
                Console.WriteLine("NO CONNECTION STRING TO USE FOR TESTING.");
                return;
            }
        }

        ~DatabaseTestingEnvironment()
        {
            this.DisposeAsync().Wait();
        }

        public override async Task StartAsync()
        {
            // start test environment
            await base.StartAsync();
        }
                
        public override async Task DisposeAsync()
        {
            if (this.IsDisposed) { return; }
                        
            await base.DisposeAsync();

            // if we aren't using a test container, clean up our test database
            if (this.TestContainer == null && DB != null)
            {
                await DB.Database.EnsureDeletedAsync();    // deletes the test database
            }
        }        
    }
}