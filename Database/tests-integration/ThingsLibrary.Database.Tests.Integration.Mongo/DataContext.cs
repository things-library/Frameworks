using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace ThingsLibrary.Database.Tests.Integration.Mongo
{
    internal class DataContext : Database.Mongo.DataContext
    {        
        public DbSet<TestData.TestClass> TestClasses { get; set; }

        public DbSet<TestData.TestInheritedClass> TestInheritedClasses { get; set; }

        public DataContext(DbContextOptions options) : base(options)
        {
            //nothing
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TestData.TestClass>();
            modelBuilder.Entity<TestData.TestInheritedClass>();
        }

        public static DataContext Create(IMongoDatabase database)
        {
            return new(new DbContextOptionsBuilder<DataContext>()
                .UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName)
                .Options);
        }
    }
}
