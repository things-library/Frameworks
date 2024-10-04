// ================================================================================
// <copyright file="ServerTests.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

//using System.Diagnostics.CodeAnalysis;
//using ThingsLibrary.Database.Tests.Integration.Base;
//using ThingsLibrary.Testing.Attributes;
//using ThingsLibrary.Testing.Environment;

//namespace ThingsLibrary.Database.Tests.Integration.SqlServer
//{
//    [TestClassIf, IgnoreIf(nameof(Base.ServerTests.IgnoreTests)), ExcludeFromCodeCoverage]
//    public class ServerTests
//    {        
//        #region --- Provider ---
       
//        private static DataContext DB { get; set; }
        

//        // ======================================================================
//        // Called once before ALL tests
//        // ======================================================================
//        [ClassInitialize]
//        public static async Task ClassInitialize(TestContext testContext)
//        {
//            await Base.ServerTests.ClassInitialize(testContext);

//            //DB = DataContext.Create(TestEnvironment.ConnectionString);
//            //DB.Database.EnsureDeleted();        //clean up for bad run last time (if exists)
//            //DB.Database.EnsureCreated();
//        }

//        // ======================================================================
//        // Called once AFTER all tests
//        // ======================================================================
//        [ClassCleanup]
//        public static async Task ClassCleanup()
//        {
//            await Base.ServerTests.ClassCleanup();
//        }
        

//        #endregion

//        [TestMethod]
//        public void Inherited_AddUpdateDelete()
//        {
//            var entityTester = new EntityTester<TestData.TestInheritedClass>(DataContext, DataContext.TestInheritedClasses);

//            var expectedData = TestData.TestInheritedClass.GetInherited();

//            Assert.IsTrue(entityTester.AddUpdateDelete(expectedData));
//        }
//    }
//}