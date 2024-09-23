using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using ThingsLibrary.Database.Postgres;

namespace ThingsLibrary.Database.Tests.Integration.Postgres
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

        public static DataContext Create(string connectionString)
        {
            return new DataContext(DataContextUtils.Parse<DataContext>(connectionString));
        }
    }
}
