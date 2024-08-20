namespace ThingsLibrary.Entity.Gcp
{
    public class EntityStore<T> : IEntityStore<T> where T : class
    {
        #region --- General ---

        /// <summary>
        /// Entity's Type
        /// </summary>
        public Type Type { get; init; }

        /// <summary>
        /// Entity Keys
        /// </summary>
        public PropertyInfo Key { get; init; }

        /// <summary>
        /// Entity Properties (public / instance)
        /// </summary>
        public List<PropertyInfo> Properties { get; init; }

        /// <summary>
        /// Entity Store Type
        /// </summary>
        public EntityStoreType StoreType => EntityStoreType.GCP_DataStore;

        /// <summary>
        /// Table Name (kind name)
        /// </summary>
        public string Name { get; init; }

        #endregion

        #region --- Store Specific ---

        // VENDOR SPECIFIC (if you aren't using the interface methods) allow for exposure of vendor specific objects
        public G.DatastoreDb GcpDataStore { get; set; }
        public G.KeyFactory GcpKeyFactory { get; set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString">Storage Connectionstring</param>
        /// <param name="name">Kind Name</param>
        /// <exception cref="ArgumentNullException">Connectionstring, ProjectId must not be null</exception>
        public EntityStore(string storageConnectionString, string name)
        {
            // validate the generic entity
            this.Type = typeof(T);
            this.Key = this.Type.GetProperties().FirstOrDefault(x => x.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.KeyAttribute), false).Any());
            this.Properties = this.Type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => this.Key != x).ToList();

            if (this.Key == null) { throw new ArgumentException($"Entity {typeof(T).Namespace}.{typeof(T).Name} has no [Key] attribute defined."); }

            // parse out the ProjectId and InstanceId from connectionstring
            if (string.IsNullOrEmpty(storageConnectionString)) { throw new ArgumentNullException(nameof(storageConnectionString)); }
            if (string.IsNullOrEmpty(name)) { throw new ArgumentNullException(nameof(name)); }

            var builder = new System.Data.Common.DbConnectionStringBuilder
            {
                ConnectionString = storageConnectionString
            };

            string projectId = builder.GetValue<string>("project_id", null) ?? throw new ArgumentException("No 'projectId' found in connection string.");
            string namespaceId = builder.GetValue<string>("namespace_id", "");
                        
            // validate name (I can't seem to find the requirements for Kind naming.. so this is just the same as the Azure naming requirements)
            if (!name.All(c => char.IsLetterOrDigit(c))) { throw new ArgumentException("Table names can only be alphanumeric characters.", nameof(name)); }
            if (name.Length < 3 || name.Length > 63) { throw new ArgumentException("Table names must be between 3 and 63 characters long.", nameof(name)); }
            if (!char.IsLetter(name[0])) { throw new ArgumentException("Table names must start with a letter", nameof(name)); }

            //core
            this.Name = name; //AKA: kind name
            
            // set up the GCP objects
            this.GcpDataStore = G.DatastoreDb.Create(projectId, namespaceId);
            this.GcpKeyFactory = this.GcpDataStore.CreateKeyFactory(this.Name);            
        }

        #region --- Entities ---

        /// <summary>
        /// Get entities from store
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetEntities(DateTime? sinceDate = null)
        {
            if(sinceDate != null)
            {
                var query = new G.Query(this.Name)
                {
                    Filter = G.Filter.GreaterThanOrEqual("Timestamp", sinceDate)
                };

                return this.GcpDataStore.RunQuery(query).Entities.Select(x => ToEntity(x));
            }
            else
            {
                var query = new G.Query(this.Name);

                return this.GcpDataStore.RunQuery(query).Entities.Select(x => ToEntity(x));
            }            
        }


        /// <summary>
        /// Get Entities by PartitionKey
        /// </summary>
        /// <returns><see cref="T"/> Records</returns>
        public IEnumerable<T> GetEntities(string partitionKey)
        {
            // GCP doesn't have partiton keys
            throw new NotImplementedException();
        }


        /// <summary>
        /// Get entities from store
        /// </summary>
        /// <param name="key">Composite Key (pipe '|' delimited)</param>
        /// <param name="partitionKey">Partition Key (not supported)</param>
        /// <returns></returns>
        public T GetEntity(object key, string partitionKey = "")
        {
            if (string.IsNullOrEmpty(partitionKey)) { throw new ArgumentException("Partion Key not supported on GCP at this time."); }

            var query = new G.Query(this.Name)
            {
                Filter = G.Filter.Equal("key", this.GcpKeyFactory.CreateKey(key.ToString()))
            };

            var entity = this.GcpDataStore.RunQuery(query).Entities.FirstOrDefault();
            if (entity == null) { return null; }

            return ToEntity(entity);
        }

        /// <summary>
        /// Insert Entity into store
        /// </summary>
        /// <param name="key">Entity Key</param>
        /// <param name="entity">Entity</param>
        /// <param name="partitionKey">Not Used for Local Entities</param>
        public void InsertEntity(object key, T entity, string partitionKey = "")
        {
            // insert the entity and get back the key assigned to it
            var gcpEntity = this.ToTableEntity(partitionKey, key, entity);

            try
            {
                this.GcpDataStore.Insert(gcpEntity);                
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        /// Update existing entity in store
        /// </summary>
        /// <param name="key">Entity Key</param>
        /// <param name="entity">Entity</param>
        /// <param name="partitionKey">Not Used for Local Entities</param>
        public void UpdateEntity(object key, T entity, string partitionKey = "")
        {
            // insert the entity and get back the key assigned to it
            this.GcpDataStore.Update(ToTableEntity(partitionKey, key, entity));
        }

        /// <summary>
        /// Upsert entity into store
        /// </summary>
        /// <param name="key">Entity Key</param>
        /// <param name="entity">Entity</param>
        /// <param name="partitionKey">Not Used for Local Entities</param>
        public void UpsertEntity(object key, T entity, string partitionKey = "")
        {
            // insert the entity and get back the key assigned to it
            this.GcpDataStore.Upsert(ToTableEntity(partitionKey, key, entity));
        }

        /// <summary>
        /// Delete entity from store
        /// </summary>        
        /// <param name="key">Entity Key</param>
        public void DeleteEntity(object key)
        {            
            this.GcpDataStore.Delete(this.GcpKeyFactory.CreateKey(key.ToString()));
        }

        #endregion

        public void DeleteStore()
        {
            throw new NotImplementedException("No direct 'table' delete as records are a 'kind' not belong to a table.");
        }

        #region --- Converstion ---

        private G.Entity ToTableEntity(string partitionKey, object key, T fromEntity)
        {            
            var toEntity = new G.Entity()
            {
                Key = this.GcpKeyFactory.CreateKey(key.ToString())
            };
                        
            // fetch the data from the object                        
            foreach (var fromProperty in this.Properties)
            {
                SetEntityValue(fromProperty, fromEntity, toEntity);
            }

            // always add/update timestamp and Partition Key properties
            toEntity["PartitionKey"] = partitionKey;
            toEntity["Timestamp"] = DateTimeOffset.UtcNow;

            return toEntity;
        }

        private T ToEntity(G.Entity fromEntity)
        {
            //var toEntity = new CloudEntity
            //{
            //    InstanceId = null,  //dataStore doesn't have a concept of instance/partitionkeys
            //    Id = fromEntity.Key.Path[0].Name,
            //    Tag = fromEntity.Key                
            //};

            //// pluck off the Instance ID property if exists
            //var value = fromEntity["InstanceId"];
            //if(value != null)
            //{
            //    toEntity.InstanceId = value.StringValue;
            //}

            //// pluck out the timestamp if we saved one
            //value = fromEntity["Timestamp"];
            //if(value != null && value.TimestampValue != null)
            //{                
            //    toEntity.UpdatedOn = value.TimestampValue.ToDateTime();
            //}

            //// fetch the data from the object            
            //string[] excludedKeys = { "InstanceId", "Timestamp" };

            //// The other values are added like a items to a dictionary
            //foreach (var fromProperty in fromEntity.Properties)
            //{
            //    if (excludedKeys.Contains(fromProperty.Key)) { continue; }

            //    SetEntityValue(fromProperty, toEntity);
            //}            

            //return toEntity;

            throw new NotImplementedException();
        }


        private static void SetEntityValue(PropertyInfo fromProperty, T fromEntity, G.Entity toEntity)
        {
            //None = 0,
            //NullValue = 11,

            //StringValue = 17,
            //IntegerValue = 2,
            //BooleanValue = 1,
            //TimestampValue = 10,

            //DoubleValue = 3,
            //KeyValue = 5,
            //BlobValue = 18,
            //GeoPointValue = 8,
            //EntityValue = 6,
            //ArrayValue = 9

            var value = fromProperty.GetValue(fromEntity);

            // no mapping null  (it should remove the value for the key instead)
            if (value == null) { return; }

            // commmonly used
            if (value is string) { toEntity[fromProperty.Name] = value as string; }
            else if (value is int) { toEntity[fromProperty.Name] = value as int?; }
            else if (value is bool) { toEntity[fromProperty.Name] = value as bool?; }

            else if (value is DateTime dataTime) { toEntity[fromProperty.Name] = DateTime.SpecifyKind(dataTime, DateTimeKind.Utc); }
            else if (value is DateTimeOffset dateTime) { toEntity[fromProperty.Name] = (dateTime).ToUniversalTime(); }

            // less commonly used
            else if (value is double) { toEntity[fromProperty.Name] = value as double?; }
            else if (value is long) { toEntity[fromProperty.Name] = value as long?; }
            else if (value is byte) { toEntity[fromProperty.Name] = value as byte?; }
            else if (value is short) { toEntity[fromProperty.Name] = value as short?; }
            else if (value is decimal) { toEntity[fromProperty.Name] = (double)(value as decimal?); }
            else
            {
                throw new ArgumentException($"Unexpected Type '{value.GetType()}'");
            }
        }

        private static void SetEntityValue(KeyValuePair<string, G.Value> fromProperty, T toEntity)
        {
            ////None = 0,
            ////NullValue = 11,

            ////StringValue = 17,
            ////IntegerValue = 2,
            ////BooleanValue = 1,
            ////TimestampValue = 10,

            ////DoubleValue = 3,
            ////KeyValue = 5,
            ////BlobValue = 18,
            ////GeoPointValue = 8,
            ////EntityValue = 6,
            ////ArrayValue = 9

            //// no mapping null  (it should remove the value for the key instead)
            //if (fromProperty.Value == null) { return; }

            //switch (fromProperty.Value.ValueTypeCase)
            //{
            //    // more common
            //    case G.Value.ValueTypeOneofCase.StringValue: { toEntity[fromProperty.Key] = fromProperty.Value.StringValue; break; }
            //    case G.Value.ValueTypeOneofCase.IntegerValue: { toEntity[fromProperty.Key] = fromProperty.Value.IntegerValue; break; }
            //    case G.Value.ValueTypeOneofCase.BooleanValue: { toEntity[fromProperty.Key] = fromProperty.Value.BooleanValue; break; }
            //    case G.Value.ValueTypeOneofCase.TimestampValue: { toEntity[fromProperty.Key] = fromProperty.Value.TimestampValue.ToDateTime(); break; }

            //    // less common
            //    case G.Value.ValueTypeOneofCase.None: { break; }
            //    case G.Value.ValueTypeOneofCase.NullValue: { toEntity[fromProperty.Key] = null; break; }
            //    case G.Value.ValueTypeOneofCase.DoubleValue: { toEntity[fromProperty.Key] = fromProperty.Value.DoubleValue; break; }

            //    default:
            //        {
            //            throw new ArgumentException($"Unexpected Type '{fromProperty.Value.ValueTypeCase}'");
            //        }
            //}

            throw new NotImplementedException();
        }

        //private static T GetValue<T>(G.Entity entity, string key, T defaultValue)
        //{
        //    if (entity == null) { return defaultValue; }

        //    var value = entity[key];
        //    if (value == null) { return defaultValue; }

        //    var itemType = typeof(T);

        //    // parse datetime differently
        //    if (itemType == typeof(DateTime))                
        //    {
        //        if (value.ValueTypeCase == G.Value.ValueTypeOneofCase.TimestampValue) 
        //        {
        //            return value.TimestampValue.ToDateTime().ConvertTo<T>();
        //        }
        //        else
        //        {
        //            throw new ArgumentException($"Stored value type '{value.ValueTypeCase}' does not match type: '{itemType}'");
        //        }
        //    }

        //    return defaultValue;
        //}

        #endregion

        #region --- IDisposable ---

        private bool _disposed = false;

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Cleanup
            if (_disposed) { return; }

            _disposed = true;   // we are handling the disposing in this thread so don't let others do the same thing

            if (disposing)
            {
                // free up objects

                // set up the GCP objects
                this.GcpDataStore = null;
                this.GcpKeyFactory = null;
            }
        }

        ~EntityStore()
        {
            this.Dispose(false);
        }

        #endregion
    }
}