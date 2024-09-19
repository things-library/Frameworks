using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace ThingsLibrary.Database.Tests.Integration.SqlServer
{
    [ExcludeFromCodeCoverage]
    public class DataContext : Database.DataContext
    {        
        public DbSet<TestData.TestInheritedClass> TestInheritedClasses { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
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
