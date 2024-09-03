using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ThingsLibrary.Database.Tests.Integration.AzureTable
{
    internal class DataContext : Database.AzureTable.DataContext
    {
        public ILogger Logger { get; set; }

        public DbSet<TestData.TestClass> TestClasses { get; set; }

        public DbSet<TestData.TestInheritedClass> TestInheritedClasses { get; set; }

        public DataContext(DbContextOptions options, ILoggerFactory loggerFactory) : base(options, loggerFactory)
        {
            this.Logger = loggerFactory.CreateLogger<DataContext>();

        }

        //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //    {
        //        base.OnConfiguring(optionsBuilder);

        //        optionsBuilder.UseAzureTableStorage("UseDevelopmentStorage=true;");
        //    }

        //    protected override void OnModelCreating(ModelBuilder modelBuilder)
        //    {
        //        base.OnModelCreating(modelBuilder);

        //        modelBuilder.Entity<Item>()
        //        .ForAzureTableStorage()
        //        .PartitionAndRowKey(b => b.Partition, b => b.Id);

        //}

    }
}
