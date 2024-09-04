namespace ThingsLibrary.Database.Cosmos
{
    public class DataContext : Database.DataContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
            //nothing
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            //nothing yet
        }
    
    }
}
