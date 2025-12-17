// ================================================================================
// <copyright file="UnitTests.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using Microsoft.Extensions.DependencyModel;
using ThingsLibrary.ItemStore.Entities;
using ThingsLibrary.ItemStore.Extensions;
using ThingsLibrary.ItemStore.Interfaces;
using ThingsLibrary.Schema.Library.Base;

namespace ThingsLibrary.Entity.Tests
{

#pragma warning disable MSTEST0030 // Type containing '[TestMethod]' should be marked with '[TestClass]'
    public class UnitTests
#pragma warning restore MSTEST0030 // Type containing '[TestMethod]' should be marked with '[TestClass]'
    {
        public static string ConnectionString { get; set; } = string.Empty;

        public static IItemStore EntityStore { get; set; }


        [TestMethod]
        public async Task Test_InsertFetch()
        {
            // COMPOSE
            var testClass = TestData.GetTestClass();

            var partitionKey = SchemaBase.GenerateKey();
            var resourceKey = $"{partitionKey}/{SchemaBase.GenerateKey()}";

            var envelope = testClass.ToEnvelope(partitionKey, resourceKey);

            // INSERT
            await EntityStore.InsertAsync(partitionKey, resourceKey, envelope, Base.CancellationToken);

            // FETCH
            var envelope2 = await EntityStore.GetAsync(partitionKey, resourceKey, Base.CancellationToken);

            // VERIFY
            Assert.IsNotNull(envelope2);

            Assert.AreEqual(envelope.Type, envelope2.Type);

            Assert.IsNotEmpty(envelope2.Data.Tags["upc"] ?? "");
            Assert.AreEqual(envelope.Data.Tags["upc"], envelope2.Data.Tags["upc"]); // in case one of them is empty but shouldn't be
        }

        [TestMethod]
        public async Task Test_InsertUpdate()
        {
            // COMPOSE
            var testClass = TestData.GetTestClass();

            var partitionKey = SchemaBase.GenerateKey();
            var resourceKey = $"{partitionKey}/{SchemaBase.GenerateKey()}";

            var envelope = testClass.ToEnvelope(partitionKey, resourceKey);

            // INSERT
            await EntityStore.InsertAsync(partitionKey, resourceKey, envelope, Base.CancellationToken);

            // FETCH
            var envelope2 = await EntityStore.GetAsync(partitionKey, resourceKey, Base.CancellationToken);

            // EDIT
            envelope2.Data.Date = DateTimeOffset.UtcNow;
            envelope2.Data.Tags["test_key"] = "test_value";

            // UPDATE
            await EntityStore.UpdateAsync(partitionKey, resourceKey, envelope2, Base.CancellationToken);

            // FETCH
            var envelope3 = await EntityStore.GetAsync(partitionKey, resourceKey, Base.CancellationToken);

            // VERIFY
            var delta = envelope3.Data.Date.Value.Subtract(envelope2.Data.Date.Value);

            Assert.AreEqual(0 , System.Math.Round(delta.TotalSeconds, 3));
            Assert.AreEqual("test_value", envelope3.Data.Tags["test_key"]);
        }

    }
}
