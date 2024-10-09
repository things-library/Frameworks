// ================================================================================
// <copyright file="DataContext.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Database
{
    // ====================================================================================
    // RELATIONAL DATABASE MIGRATIONS:
    // Good Info: https://code-maze.com/migrations-and-seed-data-efcore/
    // Good Info: https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/applying?tabs=dotnet-core-cli
    // ====================================================================================
    // PM>  Add-Migration -Project {{SERVICE_NAME}}.Infrastructure -Startup {{SERVICE_NAME}}.Infrastructure {{NAME}}
    // ====================================================================================
    // PM>  Remove-Migration  -Project {{SERVICE_NAME}}.Infrastructure -Startup {{SERVICE_NAME}}.Infrastructure
    // ====================================================================================
    // PM>  Update-Database -Project {{SERVICE_NAME}}.Infrastructure -Startup {{SERVICE_NAME}}.Infrastructure
    // -=OR Specific Migration=-
    // PM>  Update-Database -Project {{SERVICE_NAME}}.Infrastructure -Startup {{SERVICE_NAME}}.Infrastructure -Migration 20220125012319_InitDB
    // ====================================================================================
    // PM>  SqlLocalDb info MSSqlLocalDB
    // ==================================================================================== 
    // dotnet ef migrations script --idempotent

    /// <summary>
    /// This is the main database context
    /// </summary>
    public abstract class DataContext : DbContext
    {
        // Resources:
        //  https://www.learnentityframeworkcore.com/configuration/fluent-api     
        //  https://entityframeworkcore.com/knowledge-base/36354127/ef-core-implementing-table-per-concrete-type-with-fluent-mapping-of-abstract-base-class

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        public DataContext(DbContextOptions options) : base(options)
        {
            //nothing
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder options)
        //{
        //    //TODO:            
        //    Console.WriteLine("OnConfiguring()");
        //    //optionsBuilder.EnableSensitiveDataLogging();
        //}


        /// <summary>
        /// On Model Creating
        /// </summary>
        /// <param name="modelBuilder"><see cref="ModelBuilder"/></param>
        /// <exception cref="ArgumentException"></exception>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // create our models
            base.OnModelCreating(modelBuilder);
                        
            // include all of the services fluent API configurations
            var baseAssembly = typeof(DataContext).Assembly;

            Log.Information($"= Applying {baseAssembly.GetName().Name} ({baseAssembly.GetName().Version}) Configurations...");
            modelBuilder.ApplyConfigurationsFromAssembly(baseAssembly);

            var assembly = this.GetType().Assembly;
            if (assembly != baseAssembly)
            {
                Log.Information($"= Applying {assembly.GetName().Name} ({assembly.GetName().Version}) Configurations...");
                modelBuilder.ApplyConfigurationsFromAssembly(assembly);
            }

            //turn off default OnDelete:cascade
            Log.Information($"= Restricting Delete behaviors...");
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            this.SeedBaseData(modelBuilder);
        }

        /// <summary>
        /// Create Indexes for anything that has a PartitionKey Attribute or a index attribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        public void CreateIndexes<T>(EntityTypeBuilder<T> builder) where T : class
        {
            var type = typeof(T);

            // Partition index.. how the data is partitioned
            var partitionKey = type.GetProperties().FirstOrDefault(x => x.GetCustomAttributes(typeof(Attributes.PartitionKeyAttribute), false).Any());
            if (partitionKey != null)
            {
                builder.HasIndex(partitionKey.Name);
            }

            // create indexes for the Indexes tagged on the entity
            //var indexAttributes = (Attributes.IndexAttribute[])Attribute.GetCustomAttributes(type, typeof(Attributes.IndexAttribute));
            //foreach (var indexAttribute in indexAttributes)
            //{
            //    builder.HasIndex(indexAttribute.PropertyNames.ToArray());
            //}
        }


        #region --- Seed Base Data ---

        /// <summary>
        /// Add base data records that are fundimental to the database 
        /// </summary>
        /// <param name="modelBuilder"></param>
        public void SeedBaseData(ModelBuilder modelBuilder)
        {
            Console.WriteLine("= Seeding / Validating Base Database Data...");

            // EXAMPLE:
            //modelBuilder.Entity<EventType>().HasData(
            //    new EventType { Id = 1, Name = "Created", Key = "created" },
            //    new EventType { Id = 2, Name = "Updated", Key = "updated" },            
            //);
        }

        #endregion

        #region --- Save Changes ---

        //[Obsolete("Audit Tracking requires additional parameters.")]
        //public override int SaveChanges()
        //{
        //    throw new NotImplementedException();
        //}

        //[Obsolete("Audit Tracking requires additional parameters.")]
        //public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        //{
        //    throw new NotImplementedException();
        //}


        ///// <summary>
        ///// Save Changes that the current user is causing
        ///// </summary>
        ///// <param name="currentUser">Current User <see cref="ClaimsPrincipal"/></param>        
        ///// <param name="cancellationToken">Cancellation Token</param>
        ///// <returns></returns>
        //public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        //{   
        //    // don't forget to save
        //    return await base.SaveChangesAsync(cancellationToken);
        //}

        #endregion

        /// <summary>
        /// Output some meaningful connection information for logging
        /// </summary>
        /// <param name="connectionString">Database Connection String</param>
        public static void LogDatabaseSettings(ILogger logger, string connectionString)
        {
            var builder = new DbConnectionStringBuilder()
            {
                ConnectionString = connectionString
            };

            LogDatabaseSettings(logger, builder);
        }

        /// <summary>
        /// Output some meaningful connection information for logging
        /// </summary>
        /// <param name="builder">Builder</param>
        public static void LogDatabaseSettings(ILogger logger, DbConnectionStringBuilder builder)
        {
            // EXAMPLES:
            //   server=(localdb)\\mssqllocaldb;database=WeatherService;trusted_connection=true;
            //   server=localhost;database=WeatherService;User ID=sa;Password=P@ssw0rd!;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False
            //   DefaultEndpointsProtocol=https;AccountName=testaccount;AccountKey=KOMr==;EndpointSuffix=core.windows.net",
            //   Server=localhost;Port=5432;Database=test;User Id=user;Password=Test123!;

            // SQL Server Variables            
            if (builder.ContainsKey("server")) { logger.LogInformation("== Server: {DatabaseServer}", builder["server"]); }
            if (builder.ContainsKey("database")) { logger.LogInformation("== Database: {DatabaseName}", builder["database"]); }
            if (builder.ContainsKey("User ID")) { logger.LogInformation("== User: {UserId}", builder["User ID"]); }

            // SQL Server Variables
            if (builder.ContainsKey("Data Source")) { logger.LogInformation("== Data Source: {DataSource}", builder["Data Source"]); }
            if (builder.ContainsKey("Initial Catalog")) { logger.LogInformation("== Database: {DatabaseCatelog}", builder["Initial Catalog"]); }
            if (builder.ContainsKey("Integrated Security")) { logger.LogInformation("== Integrated Security: {IntegratedSecurity}", builder["Integrated Security"]); }
            
            // Cosmos
            if (builder.ContainsKey("AccountName")) { logger.LogInformation("== Account Name: {AccountName}", builder["AccountName"]); }

            // Postgres

        }
}
}
