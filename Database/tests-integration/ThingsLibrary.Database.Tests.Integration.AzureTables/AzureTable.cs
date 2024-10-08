﻿using System.Diagnostics.CodeAnalysis;
using ThingsLibrary.Database.AzureTable;


//namespace ThingsLibrary.Entity.Tests.Integration.AzureTable
//{
//    [TestClassIf, IgnoreIf(nameof(IgnoreTests)), ExcludeFromCodeCoverage]
//    public class AzureTests //: Base.IBaseTests
//    {
//        private const EntityStoreType ENTITY_STORE_TYPE = EntityStoreType.Azure_Table;
//        public static string TableName { get; set; } = $"aaaTestTable";
                
//        public static EntityStoreTester<Base.TestData.TestClass> EntityTester { get; set; } = null;
//        public static EntityStoreTester<TestData.TestClass> NativeEntityTester { get; set; } = null;

//        #region --- Provider ---

//        private static TestEnvironment TestEnvironment { get; set; }

//        // ======================================================================
//        // Called once before ALL tests
//        // ======================================================================
//        [ClassInitialize]
//        public static async Task ClassInitialize(TestContext testContext)
//        {
//            TestEnvironment = new TestEnvironment();

//            // if we have no connection string we have nothing to test
//            if (string.IsNullOrWhiteSpace(TestEnvironment.ConnectionString))
//            {
//                Console.WriteLine("NO CONNECTION STRING TO USE FOR TESTING.");
//                return;
//            }

//            // start test environment
//            await TestEnvironment.StartAsync();

//            // create our objects we are going to test
//            var entityStore = new EntityStore<Base.TestData.TestClass>(TestEnvironment.ConnectionString, TableName);
//            EntityTester = new EntityStoreTester<Base.TestData.TestClass>(entityStore);

//            var nativeEntityStore = new EntityStore<TestData.TestClass>(TestEnvironment.ConnectionString, TableName + "Native");
//            NativeEntityTester = new EntityStoreTester<TestData.TestClass>(nativeEntityStore);
//        }

//        // ======================================================================
//        // Called once AFTER all tests
//        // ======================================================================
//        [ClassCleanup]
//        public static async Task ClassCleanup()
//        {
//            if (TestEnvironment != null)
//            {
//                await TestEnvironment.DisposeAsync();
//            }

//            // if we aren't using a test container, clean up our test bucket

//            if (EntityTester?.EntityStore != null)
//            {
//                await EntityTester.EntityStore.DeleteStoreAsync();
//            }

//            if (NativeEntityTester?.EntityStore != null)
//            {
//                await NativeEntityTester.EntityStore?.DeleteStoreAsync();
//            }
//        }

//        public static bool IgnoreTests()
//        {
//            return (EntityTester == null);
//        }

//        #endregion

//        #region --- Provider Specific Tests

//        [TestMethodIf("AzureTests.StoreType")]
//        public void Base_StoreType()
//        {
//            // just so there is a basic test in here where all the others are inherited
//            Assert.AreEqual(ENTITY_STORE_TYPE, EntityTester.EntityStore.StoreType);
//            Assert.AreEqual(ENTITY_STORE_TYPE, NativeEntityTester.EntityStore.StoreType);
//        }

//        #endregion

//        #region --- BASE TESTS  ---

//        [TestMethodIf]
//        public void Base_InsertUpdateDelete()
//        {
//            EntityTester.InsertUpdateDelete(Base.TestData.TestClass.Get());
//        }

//        [TestMethodIf]
//        public void Base_UpsertUpsertAndDelete()
//        {
//            EntityTester.UpsertUpsertAndDelete(Base.TestData.TestClass.Get());
//        }

//        [TestMethodIf]
//        public void Base_InsertTwice()
//        {
//            EntityTester.InsertTwice(Base.TestData.TestClass.Get());
//        }

//        [TestMethodIf]
//        public void Base_Update()
//        {
//            EntityTester.Update(Base.TestData.TestClass.Get());
//        }


//        [TestMethodIf]
//        public void Nullable_InsertUpdateDelete()
//        {
//            EntityTester.InsertUpdateDelete(Base.TestData.TestClass.GetNulled());
//        }

//        [TestMethodIf]
//        public void Nullable_UpsertUpsertAndDelete()
//        {
//            EntityTester.UpsertUpsertAndDelete(Base.TestData.TestClass.GetNulled());
//        }

//        [TestMethodIf]
//        public void Nullable_InsertTwice()
//        {
//            EntityTester.InsertTwice(Base.TestData.TestClass.GetNulled());
//        }

//        [TestMethodIf]
//        public void Nullable_Update()
//        {
//            EntityTester.Update(Base.TestData.TestClass.GetNulled());
//        }


//        [TestMethodIf]
//        public void Inherited_InsertUpdateDelete()
//        {
//            EntityTester.InsertUpdateDelete(Base.TestData.TestInheritedClass.GetInherited());
//        }

//        [TestMethodIf]
//        public void Inherited_UpsertUpsertAndDelete()
//        {
//            EntityTester.UpsertUpsertAndDelete(Base.TestData.TestInheritedClass.GetInherited());
//        }

//        [TestMethodIf]
//        public void Inherited_InsertTwice()
//        {
//            EntityTester.InsertTwice(Base.TestData.TestInheritedClass.GetInherited());
//        }

//        [TestMethodIf]
//        public void Inherited_Update()
//        {
//            EntityTester.Update(Base.TestData.TestInheritedClass.Get());
//        }


//        #endregion

//        #region --- NATIVE TESTS ---

//        [TestMethodIf]
//        public void Native_InsertUpdateDelete()
//        {
//            NativeEntityTester.InsertUpdateDelete(TestData.TestClass.Get());
//        }

//        [TestMethodIf]
//        public void Native_UpsertUpsertAndDelete()
//        {
//            NativeEntityTester.UpsertUpsertAndDelete(TestData.TestClass.Get());
//        }

//        [TestMethodIf]
//        public void Native_InsertTwice()
//        {
//            NativeEntityTester.InsertTwice(TestData.TestClass.Get());
//        }

//        [TestMethodIf]
//        public void Native_Update()
//        {
//            NativeEntityTester.Update(TestData.TestClass.Get());
//        }

//        [TestMethodIf]
//        public void Native_InsertUpdateDeleteNullable()
//        {
//            NativeEntityTester.InsertUpdateDelete(TestData.TestClass.GetNulled());
//        }

//        [TestMethodIf]
//        public void Native_InsertTwiceNullable()
//        {
//            NativeEntityTester.InsertTwice(TestData.TestClass.GetNulled());
//        }

//        [TestMethodIf]
//        public void Native_UpdateNullable()
//        {
//            NativeEntityTester.Update(TestData.TestClass.GetNulled());
//        }

//        [TestMethodIf]
//        public void NativeInherited_InsertUpdateDelete()
//        {
//            NativeEntityTester.InsertUpdateDelete(TestData.TestInheritedClass.GetInherited());
//        }

//        [TestMethodIf]
//        public void NativeInherited_UpsertUpsertAndDelete()
//        {
//            NativeEntityTester.UpsertUpsertAndDelete(TestData.TestClass.Get());
//        }

//        [TestMethodIf]
//        public void NativeInherited_InsertTwice()
//        {
//            NativeEntityTester.InsertTwice(TestData.TestInheritedClass.GetInherited());
//        }

//        [TestMethodIf]
//        public void NativeInherited_Update()
//        {
//            NativeEntityTester.Update(TestData.TestInheritedClass.Get());
//        }

//        #endregion
//    }
//}

