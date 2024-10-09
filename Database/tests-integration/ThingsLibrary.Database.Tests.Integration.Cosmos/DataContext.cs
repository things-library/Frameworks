using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace ThingsLibrary.Database.Tests.Integration.Cosmos
{
    [ExcludeFromCodeCoverage]
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
    }
}
