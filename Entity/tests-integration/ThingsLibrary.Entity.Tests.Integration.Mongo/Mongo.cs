using Serilog;
using Starlight.Entity.Mongo;
using Starlight.Entity.Mongo.Models;
using Starlight.Entity.Types;

namespace Starlight.Entity.Tests.Integration.Mongo
{
    [TestClassIf, IgnoreIf(nameof(IgnoreTests)), ExcludeFromCodeCoverage]
    public class MongoTests
    {
        private const EntityStoreType ENTITY_STORE_TYPE = EntityStoreType.MongoDb;
        public static string TableName { get; set; } = $"aaaTestTable";

        public static EntityStoreTester<Base.TestData.TestClass> EntityTester { get; set; } = null;
        public static EntityStoreTester<TestData.TestClass> NativeEntityTester { get; set; } = null;

        #region --- Provider ---

        private static MongoDbContainer TestContainer { get; set; }
        
        private static void Init()
        {
            var configuration = Settings.GetConfigurationRoot();

            var connectionString = configuration.GetConnectionString("MongoDb");

            // see if we have an existing database we can connect to for testing
            if (string.IsNullOrEmpty(connectionString))
            {
                //TODO: REMOVE ME
                //MongoTests.TestContainer = new MongoDbBuilder().Build();

                //Log.Information("+ Starting test docker container...");
                //MongoTests.TestContainer.StartAsync().Wait(TimeSpan.FromMinutes(5));

                //// check to make sure it is running
                //if (MongoTests.TestContainer.State != DotNet.Testcontainers.Containers.TestcontainersStates.Running)
                //{
                //    Log.Error("Unable to start container in time.");
                //    return; //nothing we can do if it doesn't start                    
                //}

                //connectionString = MongoTests.TestContainer.GetConnectionString();

                //MongoTests.EntityStore = new EntityStore<TestClass>(connectionString, "test", MongoTests.TableName);
            }
            else
            {
                Log.Information("Existing database instance provided for testing...");
            }

            if (!string.IsNullOrEmpty(connectionString))
            {
                var options = new EntityStoreOptions(connectionString, "Test");
                
                var entityStore = new EntityStore<Base.TestData.TestClass>(options, TableName);
                EntityTester = new EntityStoreTester<Base.TestData.TestClass>(entityStore);

                var nativeEntityStore = new EntityStore<TestData.TestClass>(options, TableName + "Native");
                NativeEntityTester = new EntityStoreTester<TestData.TestClass>(nativeEntityStore);
            }
        }

        // ======================================================================
        // Called once before ALL tests
        // ======================================================================
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            // load it up so we don't run into a theading issue
            Init();
        }

        // ======================================================================
        // Called once AFTER all tests
        // ======================================================================
        [ClassCleanup]
        public static void ClassCleanup()
        {
            // figure out what we need to clean up.. is it a extra store we created or just a test container we created for testing                        
            if(EntityTester != null)
            {
                var entityStore = (EntityTester.EntityStore as EntityStore<Base.TestData.TestClass>);
                entityStore.Client.DropDatabase(entityStore.DatabaseName);
            }

            //EntityTester?.EntityStore.DeleteStore();
            TestContainer?.DisposeAsync();            
        }

        public static bool IgnoreTests()
        {
            return (EntityTester == null);
        }

        #endregion

        #region --- Provider Specific Tests

        [TestMethodIf("MongoTests.StoreType")]
        public void Base_StoreType()
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

        #region --- NATIVE TESTS ---

        [TestMethodIf]
        public void Native_InsertUpdateDelete()
        {
            NativeEntityTester.InsertUpdateDelete(TestData.TestClass.Get());
        }

        [TestMethodIf]
        public void Native_UpsertUpsertAndDelete()
        {
            NativeEntityTester.UpsertUpsertAndDelete(TestData.TestClass.Get());
        }

        [TestMethodIf]
        public void Native_InsertTwice()
        {
            NativeEntityTester.InsertTwice(TestData.TestClass.Get());
        }

        [TestMethodIf]
        public void Native_Update()
        {
            NativeEntityTester.Update(TestData.TestClass.Get());
        }


        [TestMethodIf]
        public void Native_InsertUpdateDeleteNullable()
        {
            NativeEntityTester.InsertUpdateDelete(TestData.TestClass.GetNulled());
        }

        [TestMethodIf]
        public void Native_UpsertUpsertAndDeleteNullable()
        {
            NativeEntityTester.UpsertUpsertAndDelete(TestData.TestClass.GetNulled());
        }

        [TestMethodIf]
        public void Native_InsertTwiceNullable()
        {
            NativeEntityTester.InsertTwice(TestData.TestClass.GetNulled());
        }

        [TestMethodIf]
        public void Native_UpdateNullable()
        {
            NativeEntityTester.Update(TestData.TestClass.GetNulled());
        }


        [TestMethodIf]
        public void NativeInherited_InsertUpdateDelete()
        {
            NativeEntityTester.InsertUpdateDelete(TestData.TestInheritedClass.GetInherited());
        }

        [TestMethodIf]
        public void NativeInherited_UpsertUpsertAndDelete()
        {
            NativeEntityTester.UpsertUpsertAndDelete(TestData.TestInheritedClass.GetInherited());
        }

        [TestMethodIf]
        public void NativeInherited_InsertTwice()
        {
            NativeEntityTester.InsertTwice(TestData.TestInheritedClass.GetInherited());
        }

        [TestMethodIf]
        public void NativeInherited_Update()
        {
            NativeEntityTester.Update(TestData.TestInheritedClass.Get());
        }

        #endregion
    }
}