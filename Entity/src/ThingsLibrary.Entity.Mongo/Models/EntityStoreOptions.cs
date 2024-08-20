namespace ThingsLibrary.Entity.Mongo.Models
{
    public class EntityStoreOptions
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }

        public EntityStoreOptions(string connectionString, string databaseName)
        {
            this.ConnectionString = connectionString;
            this.DatabaseName = databaseName;
        }
    }
}
