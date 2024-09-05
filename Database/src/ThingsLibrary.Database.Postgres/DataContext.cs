using Microsoft.EntityFrameworkCore;

namespace ThingsLibrary.Database.Postgres
{
    public class DataContext : Database.DataContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
            //nothing
        }

    }
}
