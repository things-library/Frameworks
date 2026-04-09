// ================================================================================
// <copyright file="DataContext.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace ThingsLibrary.Database.Tests.Integration.SqlServer
{
    [ExcludeFromCodeCoverage]
    public class DataContext : Database.DataContext
    {
        public DbSet<TestData.TestInheritedClass> TestInheritedClasses { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            //nothing   
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<TestData.TestClass>(builder => {
            //    builder.HasKey(entity => entity.Id);
            //    builder.HasIndex(x => x.PartitionKey);
            //});

            modelBuilder.Entity<TestData.TestInheritedClass>(builder =>
            {
                builder.HasKey(entity => entity.Id);
                builder.HasIndex(x => x.PartitionKey);
                //builder.Property(x => x.Timestamp);
            });

        }

        public static DataContext Create(string connectionString)
        {
            return new(new DbContextOptionsBuilder<DataContext>()
                .UseSqlServer(connectionString)
                .Options);
        }
    }
}
