using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ThingsLibrary.Database.Tests.Integration.Cosmos
{
    internal class DataContext : Database.Cosmos.DataContext
    {        
        //public DbSet<TestData.TestClass> TestClasses { get; set; }

        public DbSet<TestData.TestInheritedClass> TestInheritedClasses { get; set; }

        public DataContext(DbContextOptions options) : base(options)
        {
            //nothing            
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
                        

            //modelBuilder.Entity<TestData.TestClass>(builder => {
            //    builder.HasKey(entity => entity.Id);
            //    builder.HasIndex(x => x.PartitionKey);
            //});

            modelBuilder.Entity<TestData.TestInheritedClass>(builder => {
                builder.HasKey(entity => entity.Id);
                builder.HasIndex(x => x.PartitionKey);
                //builder.Property(x => x.Timestamp);
            });

        }

        public static DataContext Create(string connectionString, string databaseName)
        {
            //options.UseCosmos(
            //        "https://homiostorage.table.core.windows.net/",
            //        "{Account Key}",
            //        databaseName: "{name of storage account}"));

            return new(new DbContextOptionsBuilder<DataContext>()
                .UseCosmos("https://iqtechdev.table.core.windows.net/", "KOMr3Bt/ZCuj3O3PAyL1ZVHTGFb5OUPAFcW6x7JIcQCCJZsvLFYNit1uwLF2QxAq4T4F+WvkrWJK+AStk7reeA==", databaseName)
                .Options);
        }
    }
}
