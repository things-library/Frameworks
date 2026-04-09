// ================================================================================
// <copyright file="ServerTests.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using System.Diagnostics.CodeAnalysis;
using ThingsLibrary.Database.Tests.Integration.Base;
using ThingsLibrary.Testing.Attributes;
using ThingsLibrary.Testing.Environment;

namespace ThingsLibrary.Database.Tests.Integration.SqlServer
{
    [TestClassIf, IgnoreIf(nameof(IgnoreTests)), ExcludeFromCodeCoverage]
    public class ServerTests
    {
        public static DatabaseTestingEnvironment DbTestEnvironment { get; set; } = new DatabaseTestingEnvironment();

        #region --- Provider ---

        private static DataContext? DB { get; set; }

        // ======================================================================
        // Called once before ALL tests
        // ======================================================================
        [ClassInitialize]
        public static async Task ClassInitialize(TestContext testContext)
        {
            await DbTestEnvironment.StartAsync();

            // see if we have any reason to just exit and ignore tests
            if (DbTestEnvironment.IgnoreTests()) { return; }


            ServerTests.DB = DataContext.Create(DbTestEnvironment.ConnectionString);

            DbTestEnvironment.DB = ServerTests.DB;
            //DbTestEnvironment.DB.Database.EnsureDeleted();        //clean up for bad run last time (if exists)
            DbTestEnvironment.DB.Database.EnsureCreated();
        }

        // ======================================================================
        // Called once AFTER all tests
        // ======================================================================
        [ClassCleanup]
        public static async Task ClassCleanup()
        {
            await DbTestEnvironment.DisposeAsync();
        }

        public static bool IgnoreTests()
        {
            return (DB == null);
        }

        #endregion

        [TestMethodIf]
        public void Inherited_AddUpdateDelete()
        {
            ArgumentNullException.ThrowIfNull(DB);

            var entityTester = new EntityTester<TestData.TestInheritedClass>(DB, DB.TestInheritedClasses);

            var expectedData = TestData.TestInheritedClass.GetInherited();

            Assert.IsTrue(entityTester.AddUpdateDelete(expectedData));
        }
    }
}