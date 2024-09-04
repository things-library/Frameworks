﻿using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ThingsLibrary.Database.Tests.Integration.Mongo
{
    internal class DataContext : Database.Mongo.DataContext
    {        
        //public DbSet<TestData.TestClass> TestClasses { get; set; }

        public DbSet<TestData.TestInheritedClass> TestInheritedClasses { get; set; }

        public DataContext(DbContextOptions options) : base(options)
        {
            //nothing
            BsonDefaults.GuidRepresentationMode = GuidRepresentationMode.V3;   
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder options)
        //{
        //    base.OnConfiguring(options);
        //}

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