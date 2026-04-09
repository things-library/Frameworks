// ================================================================================
// <copyright file="TestEnvironment.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using System.Diagnostics;

namespace ThingsLibrary.Testing.Environment
{
    //public class TestEnvironment
    //{
    //    public Guid Id { get; } = Guid.NewGuid();
    //    public string TempPath { get; init; } = Path.GetTempPath();

    //    /// <summary>
    //    /// Connection String to services to test against
    //    /// </summary>
    //    public string ConnectionString { get; init; }

    //    /// <summary>
    //    /// If we already have a container running that we will be using for testing
    //    /// </summary>
    //    public bool UseExistingContainer { get; set; }

    //    /// <summary>
    //    /// Container Options
    //    /// </summary>
    //    public TestContainerOptions? ContainerOptions { get; init; }

    //    /// <summary>
    //    /// Loaded configuration items
    //    /// </summary>
    //    public IConfigurationRoot Configuration { get; init; }

    //    /// <summary>
    //    /// TestContainer instance
    //    /// </summary>
    //    public IContainer? TestContainer { get; init; }

    //    /// <summary>
    //    /// Configure the test environment
    //    /// </summary>
    //    /// <exception cref="ArgumentException"></exception>
    //    public TestEnvironment()
    //    {
    //        // get the configuration information
    //        this.Configuration = new ConfigurationBuilder()
    //           .AddJsonFile("appsettings.json", optional: false)               
    //           .AddUserSecrets(Assembly.GetCallingAssembly()!)
    //           .Build();

    //        // Get stragetic items from the config sections
    //        var testEnvironmentSection = this.Configuration.GetSection("TestEnvironment");
    //        if (!testEnvironmentSection.Exists()) { throw new ArgumentException("Unable to find test environment section in configuration details."); }

    //        var environmentOptions = testEnvironmentSection.Get<TestEnvironmentOptions>() ?? throw new ArgumentException("Unable deserialize test environment object in configuration details.");
            
    //        this.ConnectionString = this.Configuration.GetConnectionString(environmentOptions.ConnectionStringVariable) ?? string.Empty;
    //        if (string.IsNullOrEmpty(this.ConnectionString)) { return; }

    //        this.UseExistingContainer = environmentOptions.UseExistingContainer;
    //        this.ContainerOptions = environmentOptions.TestContainer;
            
    //        // make sure the directory exists (especially as we might make the container use it)
    //        Directory.CreateDirectory(this.TempPath);

    //        if (!this.UseExistingContainer && this.ContainerOptions != null)
    //        {
    //            //Check to make sure docker is even running
    //            if (!this.IsDockerRunning())
    //            {
    //                Console.WriteLine("DOCKER DAEMON NOT RUNNING!");
    //                return;
    //            }

    //            this.TestContainer = this.ContainerOptions
    //            .GetContainerBuilder()
    //            .Build();
    //        }            
    //    }        

    //    /// <summary>
    //    /// Start Test Environment
    //    /// </summary>
    //    public virtual async Task StartAsync()
    //    {            
    //        if(this.TestContainer != null && !this.UseExistingContainer)
    //        {
    //            Console.Write("Starting docker container...");
    //            await this.TestContainer.StartAsync().ConfigureAwait(false);

    //            Console.WriteLine("Done");
    //        }

    //        //TODO: do any other test environment things
    //    }

    //    public bool IsDockerRunning()
    //    {
    //        try
    //        {
    //            var info = new ProcessStartInfo() 
    //            {
    //                FileName = "docker", 
    //                Arguments = "ps", 
    //                RedirectStandardOutput = true 
    //            };
                
    //            using (var process = Process.Start(info))
    //            {                    
    //                if (process is null) { return false; }
    //                process.WaitForExit(TimeSpan.FromSeconds(10));

    //                // exit code = 0 is success
    //                return (process.ExitCode == 0);
    //            }
    //        }
    //        catch
    //        {
    //            return false;
    //        }
    //    } 
        
    //    public bool IgnoreTests()
    //    {
    //        // if we have no connection string we have nothing to test
    //        if (string.IsNullOrWhiteSpace(this.ConnectionString))
    //        {
    //            Console.WriteLine("NO CONNECTION STRING TO USE FOR TESTING.");
    //            return true;
    //        }

    //        if (!this.UseExistingContainer && this.TestContainer == null)
    //        {
    //            Console.WriteLine("NO TEST CONTAINER INITIALIZED TO USE FOR TESTING.");
    //            return true;
    //        }

    //        return false;
    //    }

    //    #region --- Cleanup / Dispose ---

    //    /// <summary>
    //    /// Clean up our test environment
    //    /// </summary>
    //    /// <returns></returns>
    //    public virtual async Task DisposeAsync()
    //    {
    //        // we already called this
    //        if (this.IsDisposed) { return; }

    //        if (this.TestContainer != null)
    //        {
    //            await TestContainer.DisposeAsync();
    //        }

    //        // try to clean up our temp folder path
    //        ThingsLibrary.IO.Directory.TryDeleteDirectory(this.TempPath);

    //        this.IsDisposed = true;
    //    }


    //    public bool IsDisposed { get; set; }

    //    ~TestEnvironment()
    //    {
    //        this.DisposeAsync().Wait();
    //    }

    //    #endregion
    //}
}
