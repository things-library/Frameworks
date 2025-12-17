// ================================================================================
// <copyright file="UnitTests.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using Microsoft.Extensions.Configuration;

using ThingsLibrary.ItemStore.Extensions;
using ThingsLibrary.ItemStore.Interfaces;
using ThingsLibrary.Testing.Attributes;

namespace ThingsLibrary.ItemStore.Tests
{

#pragma warning disable MSTEST0030 // Type containing '[TestMethod]' should be marked with '[TestClass]'    
    public class UnitTests
#pragma warning restore MSTEST0030 // Type containing '[TestMethod]' should be marked with '[TestClass]'
    {
        public static IConfiguration Configuration { get; set; }
        public static CancellationToken CancellationToken { get; set; } = new CancellationToken();

        public static string ConnectionString { get; set; } = string.Empty;
        public static string DatabaseName { get; set; } = string.Empty;

        public static IItemStore? ItemStore { get; set; }

        [TestMethod]
        public void Canvas_Test()
        {
            Assert.IsNotNull(App.Service.Canvas);
        }

        /// <summary>
        /// If The entity store has been set
        /// </summary>
        public static bool ItemStoreMissing => (ItemStore == null);


        [TestMethodIf]
        public async Task Test_InsertFetch()
        {
            Assert.IsNotNull(ItemStore);

            // ======================================================================
            // COMPOSE
            var testClass = TestData.GetTestClass();

            var partitionKey = $"insert_fetch_{Random.Shared.Next(1000, 9999)}";
            var resourceKey = $"{partitionKey}/{Random.Shared.Next(1000, 9999)}";

            var envelope = testClass.ToEnvelope(partitionKey, resourceKey);

            // add some sort of system metadata
            envelope.SystemMeta["source"] = "Test_InsertFetch";
            envelope.SystemMeta["correlation_id"] = Guid.NewGuid().ToString();
            envelope.SystemMeta["$visible"] = "false";

            // ======================================================================
            // INSERT
            await ItemStore.InsertAsync(envelope, UnitTests.CancellationToken);

            // ======================================================================
            // FETCH
            var envelope2 = await ItemStore.GetAsync(partitionKey, resourceKey, UnitTests.CancellationToken);

            // ======================================================================
            // VERIFY
            Assert.IsNotNull(envelope2);

            // verify the base attributes
            Assert.AreEqual(envelope.ResourceKey, envelope2.ResourceKey);
            Assert.AreEqual(envelope.Type, envelope2.Type);
            Assert.HasCount(envelope.SystemMeta.Count, envelope2.SystemMeta);
            Assert.HasCount(envelope.Data.Tags.Count, envelope2.Data.Tags);
                                   
            Assert.IsNotEmpty(envelope2.Data.Tags["upc"] ?? "");
            Assert.AreEqual(envelope.Data.Tags["upc"], envelope2.Data.Tags["upc"]); // in case one of them is empty but shouldn't be
        }

        [TestMethodIf]
        public async Task Test_InsertUpdate()
        {
            Assert.IsNotNull(ItemStore);

            // ======================================================================
            // COMPOSE
            var testClass = TestData.GetTestClass();

            var partitionKey = $"insert_update_{Random.Shared.Next(1000, 9999)}";
            var resourceKey = $"{partitionKey}/{Random.Shared.Next(1000, 9999)}";

            var envelope = testClass.ToEnvelope(partitionKey, resourceKey);

            // add some sort of system metadata
            envelope.SystemMeta["source"] = "Test_InsertUpdate";
            envelope.SystemMeta["correlation_id"] = Guid.NewGuid().ToString();
            envelope.SystemMeta["$visible"] = "false";

            // ======================================================================
            // INSERT
            await ItemStore.InsertAsync(envelope, UnitTests.CancellationToken);

            // ======================================================================
            // FETCH
            var envelope2 = await ItemStore.GetAsync(partitionKey, resourceKey, UnitTests.CancellationToken);

            // verify the base attributes
            Assert.IsNotNull(envelope2);
            Assert.AreEqual(envelope.ResourceKey, envelope2.ResourceKey);            
            Assert.AreEqual(envelope.Type, envelope2.Type);
            Assert.HasCount(envelope.SystemMeta.Count, envelope2.SystemMeta);
            Assert.HasCount(envelope.Data.Tags.Count, envelope2.Data.Tags);

            // ======================================================================
            // EDIT
            envelope2.Data.Date = DateTimeOffset.UtcNow;
            envelope2.Data.Tags["test_key"] = "test_value";
            envelope.SystemMeta["$visible"] = "true";

            // ======================================================================
            // UPDATE
            await ItemStore.UpdateAsync(envelope2, UnitTests.CancellationToken);

            // ======================================================================
            // FETCH
            var envelope3 = await ItemStore.GetAsync(partitionKey, resourceKey, UnitTests.CancellationToken);

            // ======================================================================
            // VERIFY
            Assert.IsNotNull(envelope3?.Data?.Date);

            var delta = envelope3!.Data.Date.Value.Subtract(envelope2.Data.Date.Value);

            // verify the base attributes
            Assert.AreEqual(envelope.ResourceKey, envelope3.ResourceKey);
            Assert.AreEqual(envelope.Type, envelope3.Type);
            Assert.HasCount(envelope.SystemMeta.Count, envelope3.SystemMeta);
            Assert.HasCount(envelope.Data.Tags.Count + 1, envelope3.Data.Tags);

            Assert.AreEqual(0, System.Math.Round(delta.TotalSeconds, 1));
            Assert.AreEqual("test_value", envelope3.Data.Tags["test_key"]);
        }

        [TestMethodIf]
        public async Task Test_InsertChildrenFetch()
        {
            Assert.IsNotNull(ItemStore);

            // ======================================================================
            // COMPOSE
            var testClass = TestData.GetTestClass();

            var partitionKey = $"insert_children_{Random.Shared.Next(1000, 9999)}";
            var resourceKey = $"{partitionKey}/{Random.Shared.Next(1000, 9999)}";

            var envelopes = testClass.ToEnvelopes(partitionKey, resourceKey);
            Assert.HasCount(3, envelopes);

            // ======================================================================
            // INSERT
            int i = 0;
            foreach(var env in envelopes)
            {
                // add some sort of system metadata
                env.SystemMeta["$count"] = i++.ToString();
                env.SystemMeta["source"] = "Test_InsertFetch";
                env.SystemMeta["correlation_id"] = Guid.NewGuid().ToString();
                env.SystemMeta["$visible"] = "false";

                await ItemStore.InsertAsync(env, UnitTests.CancellationToken);
            }            

            // ======================================================================
            // FETCH
            var envelopes2 = await ItemStore.GetAllAsync(partitionKey, resourceKey, UnitTests.CancellationToken);

            // ======================================================================
            // VERIFY
            Assert.HasCount(3, envelopes2);
        }

    }
}
