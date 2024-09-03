namespace ThingsLibrary.Database.AzureTable
{
    public class DataContext : Database.DataContext
    {
        public DataContext(DbContextOptions options, ILoggerFactory loggerFactory) : base(options, loggerFactory)
        {
            //
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            base.OnConfiguring(options);
                        
            //optionsBuilder.AddTableServiceClient()
        }

        /// <summary>
        /// Validates the name to be used 
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <remarks>
        /// Naming Policies:
        /// 1. Not Empty
        /// 2. Must be between 3 and 63 characters
        /// 3. Must be alphanumeric
        /// 4. Cannot begin with a number
        /// </remarks>
        public static void ValidateTableName(string name)
        {
            if (string.IsNullOrEmpty(name)) { throw new ArgumentNullException(nameof(name)); }

            // validate name
            if (!name.All(c => char.IsLetterOrDigit(c))) { throw new ArgumentException($"Table name '{name}' can only be alphanumeric characters.", nameof(name)); }
            if (name.Length < 3 || name.Length > 63) { throw new ArgumentException($"Table name '{name}' must be between 3 and 63 characters long.", nameof(name)); }
            if (!char.IsLetter(name[0])) { throw new ArgumentException($"Table name '{name}' must start with a letter", nameof(name)); }
        }
    }
}
