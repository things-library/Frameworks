using ThingsLibrary.Entity.Interfaces;

/* Unmerged change from project 'ThingsLibrary.Entity.Tests.Integration (net6.0)'
Before:
using System.IO;
After:
using ThingsLibrary.Entity.Tests.Integration.Base.TestData;
using System.IO;
*/
using ThingsLibrary.Entity.Tests.Integration.Base.TestData;
using System.IO;
using System.Reflection;

namespace ThingsLibrary.Entity.Tests.Integration.Base
{
    //[ExcludeFromCodeCoverage]
    //public static class BaseTests
    //{        
    //    public static string TableName { get; set; } = $"aaaTestTable";
    //    public static TestData TestData { get; set; } = new TestData();

    //    public static void CompareEntities<TEntity>(TEntity storeEntity, TEntity testEntity)
    //    {
    //        var testProperties = typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).ToList();

    //        Assert.IsNotNull(storeEntity);
    //        Assert.IsNotNull(testEntity);

    //        //test other properties to make sure they haven't changed
    //        foreach (var testProperty in testProperties)
    //        {
    //            Assert.AreEqual(testProperty.GetValue(storeEntity), testProperty.GetValue(testEntity));
    //        }
    //    }

    //    #region --- Tests ---

    //    public static void InsertUpdateDelete(IEntityStore<TestClass> entityStore)
    //    {
    //        var key = new Guid("99999999-9999-9999-9999-999999999900");
    //        var partitionKey = "Partition1";

    //        var testClass = TestClass.Get();
    //        testClass.RowKey = key;

    //        // ================================================================================
    //        // add it
    //        // ================================================================================
    //        entityStore.InsertEntity(testClass);

    //        // ================================================================================
    //        // fetch it
    //        // ================================================================================
    //        var storeEntity = entityStore.GetEntity(key, partitionKey);

    //        BaseTests.CompareEntities<TestClass>(storeEntity, testClass);

    //        //Assert.AreNotEqual(null, storeEntity);

    //        //Assert.AreEqual(testClass.RowKey, storeEntity.RowKey);
    //        //Assert.AreEqual(testClass.GuidValue, storeEntity.GuidValue);
    //        //Assert.AreEqual(testClass.GuidNullibleValue, storeEntity.GuidNullibleValue);
    //        //Assert.AreEqual(testClass.IntValue, storeEntity.IntValue);
    //        //Assert.AreEqual(testClass.StringValue, storeEntity.StringValue);
    //        //Assert.AreEqual(testClass.IntNullibleValue, storeEntity.IntNullibleValue);

    //        // timestamp should have been updated when inserted
    //        Assert.AreNotEqual(testClass.Timestamp, storeEntity.Timestamp);

    //        // ================================================================================
    //        // UPDATE THE ENTITY!
    //        // ================================================================================
    //        testClass.StringValue = "NewValue";
    //        entityStore.UpsertEntity(testClass);

    //        var entity2 = entityStore.GetEntity(key, partitionKey);

    //        Assert.AreNotEqual(null, entity2);
    //        Assert.AreEqual("NewValue", entity2.StringValue);

    //        // ================================================================================
    //        // DELETE THE ENTITY!
    //        // ================================================================================
    //        entityStore.DeleteEntity(key, partitionKey);

    //        // ================================================================================
    //        // try to fetch and make sure it is gone
    //        // ================================================================================
    //        storeEntity = entityStore.GetEntity(key, partitionKey);
    //        Assert.IsTrue(storeEntity == null);
    //    }

    //    public static void InsertDelete_InheritedClass(IEntityStore<TestClass> entityStore)
    //    {
    //        var key = new Guid("99999999-9999-9999-9999-999999999911");
    //        var partitionKey = "Partition2";

    //        var testClass = TestInheritedClass.Get();
    //        testClass.RowKey = key;
    //        testClass.PartitionKey = partitionKey;

    //        // ================================================================================
    //        // add it
    //        // ================================================================================
    //        entityStore.InsertEntity(testClass);

    //        // ================================================================================
    //        // fetch it
    //        // ================================================================================
    //        var storeEntity = entityStore.GetEntity(key, partitionKey);

    //        BaseTests.CompareEntities<TestClass>(storeEntity, testClass);

    //        Assert.AreNotEqual(null, storeEntity);
    //        Assert.AreEqual(testClass.IntValue, storeEntity.IntValue);
    //        Assert.AreEqual(testClass.StringValue, storeEntity.StringValue);

    //        // timestamp should have been updated when inserted
    //        Assert.AreNotEqual(testClass.Timestamp, storeEntity.Timestamp);

    //        // ================================================================================
    //        // DELETE THE ENTITY!
    //        // ================================================================================
    //        entityStore.DeleteEntity(key, partitionKey);

    //        // ================================================================================
    //        // try to fetch and make sure it is gone
    //        // ================================================================================
    //        storeEntity = entityStore.GetEntity(key, partitionKey);
    //        Assert.AreEqual(null, storeEntity);
    //    }

    //    public static void InsertDeleteNullible(IEntityStore<TestClass> entityStore)
    //    {
    //        var key = new Guid("99999999-9999-9999-9999-999999999922");
    //        var partitionKey = "Partition3";

    //        var testClass = TestClass.GetNulled();
    //        testClass.RowKey = key;
    //        testClass.PartitionKey = partitionKey;

    //        // ================================================================================
    //        // add it
    //        // ================================================================================
    //        entityStore.InsertEntity(testClass);

    //        // ================================================================================
    //        // fetch it
    //        // ================================================================================
    //        var entity = entityStore.GetEntity(key, partitionKey);

    //        Assert.AreNotEqual(null, entity);
    //        Assert.AreEqual(testClass.IntValue, entity.IntValue);
    //        Assert.AreEqual(testClass.StringValue, entity.StringValue);

    //        // timestamp should have been updated when inserted
    //        Assert.AreNotEqual(testClass.Timestamp, entity.Timestamp);

    //        // ================================================================================
    //        // DELETE THE ENTITY!
    //        // ================================================================================
    //        entityStore.DeleteEntity(key, partitionKey);

    //        // ================================================================================
    //        // try to fetch and make sure it is gone
    //        // ================================================================================
    //        entity = entityStore.GetEntity(key, partitionKey);
    //        Assert.IsTrue(entity == null);
    //    }

    //    public static void UpsertUpsertAndDelete(IEntityStore<TestClass> entityStore)
    //    {
    //        var key = new Guid("99999999-9999-9999-9999-999999999933");
    //        var partitionKey = "Partition4";

    //        var testClass = TestClass.GetNulled();            
    //        testClass.RowKey = key;
    //        testClass.PartitionKey = partitionKey;

    //        // ================================================================================
    //        // add it
    //        // ================================================================================
    //        entityStore.UpsertEntity(testClass);

    //        // ================================================================================
    //        // fetch it
    //        // ================================================================================
    //        var entity = entityStore.GetEntity(key, partitionKey);

    //        Assert.AreNotEqual(null, entity);
    //        Assert.AreEqual(testClass.IntValue, entity.IntValue);
    //        Assert.AreEqual(testClass.StringValue, entity.StringValue);

    //        // timestamp should have been updated when inserted
    //        Assert.AreNotEqual(testClass.Timestamp, entity.Timestamp);

    //        // ================================================================================
    //        // edit and re-insert
    //        // ================================================================================
    //        var newName = "NEWNAME2";
    //        entity.StringValue = newName;
    //        entityStore.UpsertEntity(entity);

    //        // ================================================================================
    //        // fetch again and test
    //        // ================================================================================
    //        var entity2 = entityStore.GetEntity(key, partitionKey);

    //        Assert.AreNotEqual(null, entity2);
    //        Assert.AreEqual(testClass.IntValue, entity2.IntValue);
    //        Assert.AreEqual(newName, entity2.StringValue);

    //        // timestamp should have been updated when inserted
    //        Assert.AreNotEqual(entity.Timestamp, entity2.Timestamp);

    //        // ================================================================================
    //        // DELETE THE ENTITY!
    //        // ================================================================================
    //        entityStore.DeleteEntity(key, partitionKey);

    //        // ================================================================================
    //        // try to fetch and make sure it is gone
    //        // ================================================================================
    //        var entity3 = entityStore.GetEntity(key, partitionKey);
    //        Assert.AreEqual(null, entity3);
    //    }

    //    public static void InsertTwice(IEntityStore<TestClass> entityStore)
    //    {
    //        var key = new Guid("99999999-9999-9999-9999-999999999944");
    //        var partitionKey = "Partition4";

    //        var testClass = TestClass.Get();
    //        testClass.RowKey = key;
    //        testClass.PartitionKey = partitionKey;

    //        // ================================================================================
    //        // add it
    //        // ================================================================================
    //        entityStore.InsertEntity(testClass);

    //        var entity = entityStore.GetEntity(key, partitionKey);
    //        Assert.AreNotEqual(null, entity);

    //        // ================================================================================
    //        // edit and add it again (should error out)
    //        // ================================================================================            
    //        entity.StringValue = "NEWNAME2";

    //        Assert.ThrowsException<IOException>(() => entityStore.InsertEntity(testClass));
    //    }
                
    //    public static void GetEntities(IEntityStore<TestClass> entityStore)
    //    {
    //        var startedOn = DateTime.UtcNow;
    //        System.Threading.Thread.Sleep(1000);

    //        string partitionKey = "Partition9";

    //        // ================================================================================
    //        // add it
    //        // ================================================================================
    //        var testClass = TestClass.Get();            
    //        testClass.RowKey = Guid.NewGuid();
    //        testClass.PartitionKey = partitionKey;
    //        entityStore.InsertEntity(testClass);

    //        // ================================================================================
    //        // add it
    //        // ================================================================================
    //        var testClass2 = TestClass.Get();
    //        testClass2.RowKey = Guid.NewGuid();
    //        testClass2.PartitionKey = partitionKey;
    //        entityStore.InsertEntity(testClass2);

    //        // ================================================================================
    //        // add it
    //        // ================================================================================
    //        var testClass3 = TestClass.Get();
    //        testClass3.RowKey = Guid.NewGuid();
    //        testClass.PartitionKey = partitionKey;
    //        entityStore.InsertEntity(testClass3);

    //        // ================================================================================
    //        // Test it
    //        // ================================================================================
    //        var entities = entityStore.GetEntities(startedOn);

    //        // we can't test for count since other tests might be adding items at the same time.. but we can make sure the items we expect to be in the list to be in the list
    //        Assert.IsTrue(entities.Any(x => x.RowKey == testClass.RowKey));
    //        Assert.IsTrue(entities.Any(x => x.RowKey == testClass2.RowKey));
    //        Assert.IsTrue(entities.Any(x => x.RowKey == testClass3.RowKey));

    //        // ================================================================================
    //        // add another and test again but move forward the 'since'
    //        // ================================================================================
    //        var lastInsertOn = DateTime.UtcNow;
    //        System.Threading.Thread.Sleep(1000);

    //        var testClass4 = TestClass.Get();
    //        testClass4.RowKey = Guid.NewGuid();
    //        testClass4.PartitionKey= partitionKey;
    //        entityStore.InsertEntity(testClass4);

    //        entities = entityStore.GetEntities(lastInsertOn);

    //        Assert.IsTrue(entities.Any(x => x.RowKey == testClass4.RowKey));

    //        Assert.IsTrue(!entities.Any(x => x.RowKey == testClass.RowKey));
    //        Assert.IsTrue(!entities.Any(x => x.RowKey == testClass2.RowKey));
    //        Assert.IsTrue(!entities.Any(x => x.RowKey == testClass3.RowKey));

    //        // ================================================================================
    //        // GET ALL TEST
    //        // ================================================================================
    //        entities = entityStore.GetEntities();

    //        Assert.IsTrue(entities.Any(x => x.RowKey == testClass.RowKey));
    //        Assert.IsTrue(entities.Any(x => x.RowKey == testClass2.RowKey));
    //        Assert.IsTrue(entities.Any(x => x.RowKey == testClass3.RowKey));
    //        Assert.IsTrue(entities.Any(x => x.RowKey == testClass4.RowKey));
    //    }

    //    #endregion
    //}
}

