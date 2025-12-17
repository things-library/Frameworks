using LiteDB;

using Starlight.Entity.Tests.Integration.Base.TestData;
using Starlight.Entity.Types;

namespace Starlight.Entity.Tests.Integration.LocalFiles
{
    [TestClassIf, IgnoreIf(nameof(IgnoreTests)), ExcludeFromCodeCoverage]
    public class LocalFilesTests //: Base.IBaseTests
    {
        private const EntityStoreType ENTITY_STORE_TYPE = EntityStoreType.Local;

        public static string TableName { get; set; } = $"aaaTestTable";
        
        public static EntityStoreTester<Base.TestData.TestClass> EntityTester { get; set; } = null;

        #region --- Provider ---

        private static void Init()
        {          
            var entityStore = new Local.EntityStore<TestClass>("Filename=litedatabase.db;Password=1234;Upgrade=true;Files=true", TableName);
            Assert.IsTrue(entityStore.IsFileSaving);
                        
            EntityTester = new EntityStoreTester<Base.TestData.TestClass>(entityStore);            
        }

        // ======================================================================
        // Called once before ALL tests
        // ======================================================================
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            Init();
        }

        // ======================================================================
        // Called once AFTER all tests
        // ======================================================================
        [ClassCleanup]
        public static void ClassCleanup()
        {
            EntityTester?.EntityStore.DeleteStoreAsync().Wait();
        }

        public static bool IgnoreTests()
        {
            return (EntityTester == null);
        }

        #endregion

        #region --- Provider Specific Tests

        [TestMethodIf]
        public void StoreType()
        {
            // just so there is a basic test in here where all the others are inherited
            Assert.AreEqual(ENTITY_STORE_TYPE, EntityTester.EntityStore.StoreType);            
        }

        #endregion

        #region --- BASE TESTS  ---

        [TestMethodIf]
        public void Base_InsertUpdateDelete()
        {
            EntityTester.InsertUpdateDelete(Base.TestData.TestClass.Get());
        }

        [TestMethodIf]
        public void Base_UpsertUpsertAndDelete()
        {
            EntityTester.UpsertUpsertAndDelete(Base.TestData.TestClass.Get());
        }

        [TestMethodIf]
        public void Base_InsertTwice()
        {
            EntityTester.InsertTwice(Base.TestData.TestClass.Get());
        }

        [TestMethodIf]
        public void Base_Update()
        {
            EntityTester.Update(Base.TestData.TestClass.Get());
        }


        [TestMethodIf]
        public void Nullable_InsertUpdateDelete()
        {
            EntityTester.InsertUpdateDelete(Base.TestData.TestClass.GetNulled());
        }

        [TestMethodIf]
        public void Nullable_UpsertUpsertAndDelete()
        {
            EntityTester.UpsertUpsertAndDelete(Base.TestData.TestClass.GetNulled());
        }

        [TestMethodIf]
        public void Nullable_InsertTwice()
        {
            EntityTester.InsertTwice(Base.TestData.TestClass.GetNulled());
        }

        [TestMethodIf]
        public void Nullable_Update()
        {
            EntityTester.Update(Base.TestData.TestClass.GetNulled());
        }


        [TestMethodIf]
        public void Inherited_InsertUpdateDelete()
        {
            EntityTester.InsertUpdateDelete(Base.TestData.TestInheritedClass.GetInherited());
        }

        [TestMethodIf]
        public void Inherited_UpsertUpsertAndDelete()
        {
            EntityTester.UpsertUpsertAndDelete(Base.TestData.TestInheritedClass.GetInherited());
        }

        [TestMethodIf]
        public void Inherited_InsertTwice()
        {
            EntityTester.InsertTwice(Base.TestData.TestInheritedClass.GetInherited());
        }

        [TestMethodIf]
        public void Inherited_Update()
        {
            EntityTester.Update(Base.TestData.TestInheritedClass.Get());
        }


        #endregion
    }
}

