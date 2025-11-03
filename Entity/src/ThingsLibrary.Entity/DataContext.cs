// ================================================================================
// <copyright file="DataContext.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using Serilog;

namespace ThingsLibrary.Entity
{
    public class DataContext : DbContext
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        public DataContext(DbContextOptions options) : base(options)
        {
            // turn off change tracking as we want to have a light weight processing system
            this.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

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

            Log.Information("= Applying {AssemblyName} ({AssemblyVersion}) Configurations...", baseAssembly.GetName().Name, baseAssembly.GetName().Version);
            modelBuilder.ApplyConfigurationsFromAssembly(baseAssembly);

            var assembly = this.GetType().Assembly;
            if (assembly != baseAssembly)
            {
                Log.Information("= Applying {AssemblyName} ({AssemblyVersion}) Configurations...", assembly.GetName().Name, assembly.GetName().Version);
                modelBuilder.ApplyConfigurationsFromAssembly(assembly);
            }

            //turn off default OnDelete:cascade
            Log.Information($"= Restricting Delete behaviors...");
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
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
                
        /// <summary>
        /// Do any database prechecks and data seeding
        /// </summary>
        public virtual void Prechecks()
        {
            this.Database.EnsureCreated();
        }        
    }
}
