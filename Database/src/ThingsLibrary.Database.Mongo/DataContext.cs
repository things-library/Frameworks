namespace ThingsLibrary.Database.Mongo
{
    public class DataContext : Database.DataContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
            //Examples: https://www.mongodb.com/developer/languages/csharp/crud-changetracking-mongodb-provider-for-efcore/?msockid=124e891271136402087098cd706d65af
        }

        //public static DataContext Create(IMongoDatabase database)
        //{
        //    var dataContext = new DataContext(new DbContextOptionsBuilder<DataContext>()
        //        .UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName)
        //        .Options);

        //    dataContext.Database.EnsureCreated();

        //    return dataContext;
        //}

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    //Example: modelBuilder.Entity<Movie>().ToCollection("movies");
        //}
    }
}
