using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;

namespace ThingsLibrary.Database.Cosmos
{
    [ExcludeFromCodeCoverage]
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


        #region --- Static --- 
        
        public static DbContextOptionsBuilder Parse<TContext>(string connectionString, string databaseName) where TContext : DbContext
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(connectionString);

            
            return new DbContextOptionsBuilder<TContext>().UseCosmos(connectionString, databaseName, builder => 
            { 
                
            });
        }

        #endregion

    }
}
