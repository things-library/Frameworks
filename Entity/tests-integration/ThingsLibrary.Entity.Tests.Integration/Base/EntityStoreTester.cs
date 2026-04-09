using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Starlight.Entity.Interfaces;

namespace Starlight.Entity.Tests.Integration.Base
{
    [ExcludeFromCodeCoverage]
    public class EntityStoreTester<TEntity> where TEntity : class
    {
        /// <summary>
        /// Entity Keys
        /// </summary>
        public PropertyInfo Key { get; init; }

        /// <summary>
        /// Partition Key (if exists)
        /// </summary>
        public PropertyInfo PartitionKey { get; init; }

        /// <summary>
        /// Timestamp Field
        /// </summary>
        public PropertyInfo Timestamp { get; init; }

        public List<PropertyInfo> TestProperties { get; init; }

        /// <summary>
        /// Entity Store Under Test
        /// </summary>
        public IEntityStore<TEntity> EntityStore { get; init; }

        /// <summary>
        /// Known system keys that will change and unrelated to our entity we are testing
        /// </summary>
        public List<string> ExcludedFields { get; set; } = new List<string> { "PartitionKey", "RowKey", "odata.etag", "Timestamp" };

        /// <summary>
        /// Get the property info (first or defailt) for the entity T
        /// </summary>
        /// <typeparam name="T">Entity we want property info from</typeparam>
        /// <param name="attribute">Custom Attribute</param>
        /// <returns><see cref="PropertyInfo"/> that has the provided attribute</returns>
        private static PropertyInfo GetPropertyInfo<T, TAttribute>() where TAttribute : Attribute
        {
            return typeof(T).GetProperties().FirstOrDefault(x => x.GetCustomAttributes(typeof(TAttribute), false).Any());
        }


        public EntityStoreTester(IEntityStore<TEntity> entityStore)
        {
            // get the key
            this.Key = GetPropertyInfo<TEntity, KeyAttribute>();
            this.PartitionKey = GetPropertyInfo<TEntity, Attributes.PartitionKeyAttribute>();
            this.Timestamp = GetPropertyInfo<TEntity, TimestampAttribute>();

            // add the three fields that are system fields so we aren't duplicating up data in storage
            if (!this.ExcludedFields.Contains(this.Key.Name)) { this.ExcludedFields.Add(this.Key.Name); }
            if (this.PartitionKey != null && !this.ExcludedFields.Contains(this.PartitionKey.Name)) { this.ExcludedFields.Add(this.PartitionKey.Name); }
            if (this.Timestamp != null && !this.ExcludedFields.Contains(this.Timestamp.Name)) { this.ExcludedFields.Add(this.Timestamp.Name); }

            this.TestProperties = typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => !this.ExcludedFields.Contains(x.Name)).ToList();

            this.EntityStore = entityStore;
        }

        private void CompareEntities(TEntity storeEntity, TEntity testEntity)
        {
            Assert.AreNotEqual(null, storeEntity);

            Assert.AreEqual(this.Key.GetValue(storeEntity), this.Key.GetValue(testEntity));
            if (this.PartitionKey != null)
            {
                Assert.AreEqual(this.PartitionKey.GetValue(storeEntity), this.PartitionKey.GetValue(testEntity));
            }

            // timestamp should have been updated when inserted
            if (this.Timestamp != null)
            {
                Assert.AreNotEqual(this.Timestamp.GetValue(storeEntity), this.Timestamp.GetValue(testEntity));
            }

            //test other properties to make sure they haven't changed
            foreach (var testProperty in this.TestProperties)
            {
                Assert.AreEqual(testProperty.GetValue(storeEntity), testProperty.GetValue(testEntity));
            }
        }


        private void ChangeRandomFields(TEntity entity)
        {
            var guidProperty = this.TestProperties.Where(x => x.PropertyType == typeof(Guid)).FirstOrDefault();
            if (guidProperty != null)
            {
                guidProperty.SetValue(entity, Guid.NewGuid());            
            }

            var dateTimeProperty = this.TestProperties.Where(x => x.PropertyType == typeof(DateTime)).FirstOrDefault();
            if(dateTimeProperty != null)
            {
                var dt = DateTime.UtcNow;   // lets not mess with subseconds since that might not be a precision that the document system has                
                dateTimeProperty.SetValue(entity, new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, DateTimeKind.Utc));
            }
        }

        /// <summary>
        /// This Tests insert, fetch, update, fetch, delete, fetch
        /// </summary>
        /// <param name="testEntity"></param>
        public void InsertUpdateDelete(TEntity testEntity)
        {
            // so it is a unique record that we don't already have
            this.SetValue(testEntity, this.Key, Guid.NewGuid());

            object key = this.Key.GetValue(testEntity);
            object partitionKey = this.PartitionKey?.GetValue(testEntity);

            // ================================================================================
            // add it
            // ================================================================================
            this.EntityStore.InsertEntityAsync(testEntity).Wait();

            // ================================================================================
            // fetch it
            // ================================================================================
            var storeEntity = this.EntityStore.GetEntityAsync(key, partitionKey).Result;
                        
            this.CompareEntities(storeEntity, testEntity);

            // ================================================================================
            // UPDATE THE ENTITY!
            // ================================================================================
            this.ChangeRandomFields(storeEntity);
            this.EntityStore.UpsertEntityAsync(storeEntity).Wait();

            var storeEntity2 = this.EntityStore.GetEntityAsync(key, partitionKey).Result;

            this.CompareEntities(storeEntity2, storeEntity);
                        
            // ================================================================================
            // DELETE THE ENTITY!
            // ================================================================================
            this.EntityStore.DeleteEntityAsync(key, partitionKey).Wait();

            // ================================================================================
            // try to fetch and make sure it is gone
            // ================================================================================
            var storeEntity3 = this.EntityStore.GetEntityAsync(key, partitionKey).Result;
            Assert.IsNull(storeEntity3);
        }

        /// <summary>
        /// This tests upserting multiple times and deleting
        /// </summary>
        /// <param name="testEntity"></param>
        public void UpsertUpsertAndDelete(TEntity testEntity)
        {
            // so it is a unique record that we don't already have
            this.SetValue(testEntity, this.Key, Guid.NewGuid());

            object key = this.Key.GetValue(testEntity);
            object partitionKey = this.PartitionKey?.GetValue(testEntity);            

            // ================================================================================
            // add it
            // ================================================================================
            this.EntityStore.UpsertEntityAsync(testEntity).Wait();

            // ================================================================================
            // fetch it
            // 
            var storeEntity = this.EntityStore.GetEntityAsync(key, partitionKey).Result;

            this.CompareEntities(storeEntity, testEntity);

            // ================================================================================
            // edit and re-insert
            // ================================================================================
            this.ChangeRandomFields(storeEntity);     
            this.EntityStore.UpsertEntityAsync(storeEntity).Wait();

            // ================================================================================
            // fetch again and test
            // ================================================================================
            var storeEntity2 = this.EntityStore.GetEntityAsync(key, partitionKey).Result;

            this.CompareEntities(storeEntity, storeEntity2);
                        
            // ================================================================================
            // DELETE THE ENTITY!
            // ================================================================================
            this.EntityStore.DeleteEntityAsync(key, partitionKey).Wait();

            // ================================================================================
            // try to fetch and make sure it is gone
            // ================================================================================
            var sotreEntity3 = this.EntityStore.GetEntityAsync(key, partitionKey).Result;
            Assert.AreEqual(null, sotreEntity3);
        }

        /// <summary>
        /// This tests inserting twice since the second time should fail
        /// </summary>
        /// <param name="testEntity"></param>
        public void InsertTwice(TEntity testEntity)
        {
            // so it is a unique record that we don't already have
            this.SetValue(testEntity, this.Key, Guid.NewGuid());

            object key = this.Key.GetValue(testEntity);
            object partitionKey = this.PartitionKey?.GetValue(testEntity);

            // ================================================================================
            // add it
            // ================================================================================
            this.EntityStore.InsertEntityAsync(testEntity).Wait();

            // ================================================================================
            // fetch and test
            // ================================================================================
            var storeEntity = this.EntityStore.GetEntityAsync(key, partitionKey).Result;

            this.CompareEntities(storeEntity, testEntity);

            // ================================================================================
            // edit and insert it again (should error out)
            // ================================================================================            
            this.ChangeRandomFields(storeEntity);
            Assert.ThrowsException<IOException>(() => this.EntityStore.InsertEntityAsync(storeEntity).Wait());
        }

        /// <summary>
        /// This tests inserting twice since the second time should fail
        /// </summary>
        /// <param name="testEntity"></param>
        public void Update(TEntity testEntity)
        {
            // so it is a unique record that we don't already have
            this.SetValue(testEntity, this.Key, Guid.NewGuid());

            // ================================================================================
            // update it (should error because it doesn't exist)
            // ================================================================================
            Assert.ThrowsException<IOException>(() => this.EntityStore.UpdateEntityAsync(testEntity).Wait());
        }


        private void SetValue(TEntity entity, PropertyInfo entityProperty, object value)
        {
            // direct conversion? (most likely the case)
            if (value.GetType() == entityProperty.PropertyType)
            {
                entityProperty.SetValue(entity, value);
            }
            else if (entityProperty.PropertyType == typeof(byte) || entityProperty.PropertyType == typeof(byte?))
            {
                var toValue = Convert.ToByte(value);
                entityProperty.SetValue(entity, toValue);
            }
            else if (entityProperty.PropertyType == typeof(short) || entityProperty.PropertyType == typeof(short?))
            {
                var toValue = Convert.ToInt16(value);
                entityProperty.SetValue(entity, toValue);
            }
            else if (entityProperty.PropertyType == typeof(int) || entityProperty.PropertyType == typeof(int?))
            {
                var toValue = Convert.ToInt32(value);
                entityProperty.SetValue(entity, toValue);
            }
            else if (entityProperty.PropertyType == typeof(float) || entityProperty.PropertyType == typeof(float?))
            {
                var toValue = Convert.ToSingle(value);
                entityProperty.SetValue(entity, toValue);
            }
            else if (entityProperty.PropertyType == typeof(DateTime) || entityProperty.PropertyType == typeof(DateTime?))
            {
                var toValue = (DateTimeOffset)value;
                entityProperty.SetValue(entity, toValue.DateTime);
            }
            else if (entityProperty.PropertyType == typeof(Guid) || entityProperty.PropertyType == typeof(Guid?))
            {
                var toValue = Guid.Parse(value.ToString());
                entityProperty.SetValue(entity, toValue);
            }
            else if (entityProperty.PropertyType == typeof(string))
            {                
                entityProperty.SetValue(entity, value.ToString());
            }
            else
            {

                entityProperty.SetValue(entity, value);
            }
        }
    }


}

