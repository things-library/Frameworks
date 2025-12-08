// ================================================================================
// <copyright file="UnitTests.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using ThingsLibrary.Cache.Tests.TestData;

namespace ThingsLibrary.Cache.Tests
{
    [TestClass]
    public sealed class UnitTests
    {
        private static ICacheStore Cache { get; set; }

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {               
            // Create Service Provider
            var services = new ServiceCollection();

            services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));            
            services.AddDistributedMemoryCache();

            // JSON OPTIONS             
            services.TryAddSingleton<JsonSerializerOptions>(new JsonSerializerOptions());   //required 
            
            services.AddTransient<ICacheStore, CacheStore>();

            // BUILD
            var serviceProvider = services.BuildServiceProvider();
            
            UnitTests.Cache = serviceProvider.GetRequiredService<ICacheStore>();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            // This method is called once for the test class, after all tests of the class are run.
        }
                
        [TestMethod]
        public void CacheSetGet()
        {
            var key = "TestKey";
            var testClass = new TestClass()
            {
                Date = new DateTime(2020, 1, 2, 3, 4, 5, 6, 7, DateTimeKind.Utc),
                Name = "Test",
                Type = "parent",
                Items =
                {
                    new KeyValuePair<string, TestClass>("childKey", new TestClass
                    {
                        Name = "Child",                        
                        Type = "child"
                    })
                }
            };

            UnitTests.Cache.SetAsync<TestClass>(key, testClass, DateTime.UtcNow.AddSeconds(3), default).Wait();

            var cacheClass = UnitTests.Cache.GetAsync<TestClass>(key, default).Result;
            Assert.IsNotNull(cacheClass);

            Assert.AreEqual(testClass.Date, cacheClass.Date);
            Assert.AreEqual(testClass.Name, cacheClass.Name);

            Assert.AreEqual(testClass.Items["childKey"].Name, cacheClass.Items["childKey"].Name);
            Assert.AreEqual(testClass.Items["childKey"].Type, cacheClass.Items["childKey"].Type);
        }
    }
}
