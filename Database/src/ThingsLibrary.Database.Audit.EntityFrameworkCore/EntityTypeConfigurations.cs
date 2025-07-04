// ================================================================================
// <copyright file="EntityTypeConfigurations.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ThingsLibrary.Database.Audit.EntityFrameworkCore
{
    /// <summary>
    /// Configure Fluent API 
    /// </summary>
    public class AuditEventConfig : IEntityTypeConfiguration<AuditEvent>
    {
        /// <summary>
        /// Configure Database
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<AuditEvent> builder)
        {
            Log.Information("== Configuring '{EntityType}' entity types...", nameof(AuditEvent));

            // secondary key
            builder.Property(x => x.SequenceId).ValueGeneratedOnAdd();

            // fields
            builder.Property(x => x.EventType)
                .HasConversion<short>(x => (short)x, x => (AuditType)x);

            builder.Property(x => x.NewValues).HasJsonConversion();
            builder.Property(x => x.OldValues).HasJsonConversion();
        }        
    }

    /// <summary>
    /// Configuration Details
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AuditTableConfig<T> : IEntityTypeConfiguration<T> where T : AuditTable
    {
        /// <summary>
        /// Configure
        /// </summary>
        /// <param name="builder">EntityTypeBuilder</param>
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            Log.Information("== Configuring Generic Audit Table '{EntityType}' entity types...", typeof(T).Name);

            builder.HasOne(x => x.CreateEvent)
              .WithOne()
              .HasForeignKey<T>(x => x.CreateEventId)
              .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.LastUpdateEvent)
              .WithOne()
              .HasForeignKey<T>(x => x.LastUpdateEventId)
              .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.DeleteEvent)
              .WithOne()
              .HasForeignKey<T>(x => x.DeleteEventId)
              .OnDelete(DeleteBehavior.Restrict);
        }
    }


}
