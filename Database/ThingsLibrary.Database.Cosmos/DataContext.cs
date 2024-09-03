namespace ThingsLibrary.Database.Cosmos
{
    public class DataContext : Database.DataContext
    {
        public DataContext(DbContextOptions options, ILoggerFactory loggerFactory) : base(options, loggerFactory)
        {
            //nothing
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseCosmos("https://localhost:8081", "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==", databaseName: "OrdersDB");

            //await context.Database.EnsureCreatedAsync();
        }
    
    }
}
