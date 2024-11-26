// ================================================================================
// <copyright file="DataContext.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Diagnostics.CodeAnalysis;

namespace ThingsLibrary.Database.Tests.Integration.Mongo
{
    [ExcludeFromCodeCoverage]
    internal class DataContext : Database.DataContext
    {   
        public DbSet<TestData.TestInheritedClass> TestInheritedClasses { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            //BsonDefaults.GuidRepresentationMode = GuidRepresentationMode.V3;
        }
                
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);                        

            //modelBuilder.Entity<TestData.TestClass>(builder => {
            //    builder.HasKey(entity => entity.Id);
            //    builder.HasIndex(x => x.PartitionKey);
            //});

            modelBuilder.Entity<TestData.TestInheritedClass>(builder => {
                builder.HasKey(entity => entity.Id);
                builder.HasIndex(x => x.PartitionKey);
                builder.Property(x => x.UpdatedOn)                    
                    .ValueGeneratedOnAddOrUpdate();
            });
        }

        public static DataContext Create(IMongoDatabase database)
        {
            return new(new DbContextOptionsBuilder<DataContext>()
                .UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName)
                .Options);
        }
    }
}
