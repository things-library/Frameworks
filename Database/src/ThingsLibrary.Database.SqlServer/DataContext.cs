using Microsoft.EntityFrameworkCore;

namespace ThingsLibrary.Database.SqlServer
{
    public class DataContext : Database.DataContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
            //nothing
        }
    }
}
