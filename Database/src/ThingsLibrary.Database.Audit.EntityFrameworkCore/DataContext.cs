// ================================================================================
// <copyright file="DataContext.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics;

namespace ThingsLibrary.Database.Audit.EntityFrameworkCore
{   
    // ====================================================================================
    // DATABASE MIGRATIONS:
    // Good Info: https://code-maze.com/migrations-and-seed-data-efcore/
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

    /// <summary>
    /// This is the main database context
    /// </summary>
    public class DataContext : DbContext
    {
        // Resources:
        //  https://www.learnentityframeworkcore.com/configuration/fluent-api     
        //  https://entityframeworkcore.com/knowledge-base/36354127/ef-core-implementing-table-per-concrete-type-with-fluent-mapping-of-abstract-base-class

        /// <summary>
        /// Audit Users
        /// </summary>
        public DbSet<AuditUser> AuditUsers { get; set; }

        /// <summary>
        /// Audit Event Log
        /// </summary>
        public DbSet<AuditEvent> AuditEvents { get; set; }

        ///// <summary>
        ///// Constructor
        ///// </summary>
        //public DataContext() : base()
        //{
        //    // turn off change tracking as we want to have a light weight processing system
        //    this.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        //}

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
            
            this.CreateIndexes<AuditUser>(modelBuilder.Entity<AuditUser>());
            this.CreateIndexes<AuditEvent>(modelBuilder.Entity<AuditEvent>());
            
            //var assembly = this.GetType().Assembly;
            //if(assembly != baseAssembly)
            //{
            //    Log.Information("= Applying {AssemblyName} ({AssemblyVersion}) Configurations...", assembly.GetName().Name, assembly.GetName().Version);
            //    modelBuilder.ApplyConfigurationsFromAssembly(assembly);
            //}

            //turn off default OnDelete:cascade
            Log.Information($"= Restricting Delete behaviors...");
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            this.SeedBaseData(modelBuilder);
        }
           
        public void CreateIndexes<T>(EntityTypeBuilder<T> builder) where T : class
        {
            var type = typeof(T);
            
            // Partition index.. how the data is partitioned
            var partitionKey = type.GetProperties().FirstOrDefault(x => x.GetCustomAttributes(typeof(Attributes.PartitionKeyAttribute), false).Any());
            if(partitionKey != null)
            {
                builder.HasIndex(partitionKey.Name);
            }

            // create indexes for the Indexes tagged on the entity
            var indexAttributes = (Attributes.IndexAttribute[])Attribute.GetCustomAttributes(type, typeof(Attributes.IndexAttribute));
            foreach (var indexAttribute in indexAttributes)
            {
                builder.HasIndex(indexAttribute.PropertyNames.ToArray());
            }
        }


        #region --- Seed Base Data ---

        /// <summary>
        /// Add base data records that are fundimental to the database 
        /// </summary>
        /// <param name="modelBuilder"></param>
        public void SeedBaseData(ModelBuilder modelBuilder)
        {
            Log.Information("= Seeding / Validating Base Database Data...");

            // EXAMPLE:
            //modelBuilder.Entity<AuditUser>().HasData(               
            ////    new EventType { Id = 2, Name = "Updated", Key = "updated" },            
            //);
        }

        #endregion

        #region --- Save Changes ---

        [Obsolete("Audit Tracking requires additional parameters.")]
        public override int SaveChanges()
        {
            throw new NotImplementedException();
        }

        [Obsolete("Audit Tracking requires additional parameters.")]
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Save Changes that the current user is causing
        /// </summary>
        /// <param name="currentUser">Current User <see cref="ClaimsPrincipal"/></param>        
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns></returns>
        public async Task<int> SaveChangesAsync(Guid currentUserId, CancellationToken cancellationToken = default)
        {   
            // add all the audit events based on what is being saved out
            this.AddAuditEvents(currentUserId);

            // don't forget to save
            return await base.SaveChangesAsync(cancellationToken);
        }

        #endregion

        #region --- Audit ---

        /// <summary>
        /// Parse out the elements we need to uniquely identify who edited the data
        /// </summary>        
        /// <exception cref="ArgumentException">When User Not Logged in but table with audit tracking has been changed.</exception>
        public async Task<AuditUser> GetOrCreateAuditUserAsync(ClaimsPrincipal userPrincipal, CancellationToken cancellationToken = default)
        {
            if (userPrincipal.Identity?.IsAuthenticated != true) { throw new ArgumentException("User Not Authenticated."); }

            // Note:  if the email address changes that is just considered a new account at this point since OID and UPN can often be missing depending on auth provider configuration.
            // 
            
            var objectId = this.GetUserId(userPrincipal);
            if (objectId == default) { throw new ArgumentException("Unable to determine unique User ID from claims principal"); }

            var username = this.GetUsername(userPrincipal);
            if (username == default) { throw new ArgumentException("Unable to find username within claims principal."); }

            var (givenName, familyName) = this.GetName(userPrincipal);
            var emailAddress = this.GetEmail(userPrincipal);

            // try to find the user based on the object id (if exists)
            var auditUser = await this.AuditUsers.SingleOrDefaultAsync(x => x.ObjectId == objectId, cancellationToken);
            if(auditUser == null)
            {
                auditUser = await this.AuditUsers.SingleOrDefaultAsync(x => x.Username == username, cancellationToken);
            }

            if (auditUser == null)
            {
                // FINE... didn't find the record so we need to create it
                auditUser = new AuditUser
                {
                    Id = DataType.SequentialGuid.NewGuid(),
                    ObjectId = objectId,
                    Username = username,

                    GivenName = givenName,
                    FamilyName = familyName,
                    EmailAddress = emailAddress                    
                };
                
                this.AuditUsers.Add(auditUser);
                await base.SaveChangesAsync(cancellationToken);
            }
            else
            {
                //bool isChanged = false;
                this.Attach(auditUser);

                auditUser.ObjectId = objectId;
                auditUser.Username = username;
                auditUser.GivenName = givenName;
                auditUser.FamilyName = familyName;
                auditUser.EmailAddress = emailAddress;
              
                if (this.ChangeTracker.HasChanges())
                {
                    await base.SaveChangesAsync(cancellationToken);
                }
            }

            return auditUser;            
        }

        #region --- User Properties ---

        private string? GetUserId(ClaimsPrincipal userPrincipal)
        {
            // try to find the user based on the object id (if exists)
            var userId = userPrincipal.Claims.SingleOrDefault(c => c.Type == "sub")?.Value.ToLower();
            if (userId == null) { userId = userPrincipal.Claims.SingleOrDefault(c => c.Type == "oid")?.Value.ToLower(); }

            return userId;
        }

        public string? GetUsername(ClaimsPrincipal userPrincipal)
        {
            // find a unique username to use
            var username = userPrincipal.Claims.FirstOrDefault(x => x.Type == "preferred_username")?.Value.ToLower();
            if (username == default) { username = userPrincipal.Claims.FirstOrDefault(x => x.Type == "upn")?.Value.ToLower(); }
            if (username == default) { username = userPrincipal.Claims.FirstOrDefault(x => x.Type == "username")?.Value.ToLower(); }
            if (username == default) { username = userPrincipal.Claims.FirstOrDefault(x => x.Type == "email")?.Value.ToLower(); }

            return username;
        }

        private string GetEmail(ClaimsPrincipal userPrincipal)
        {
            // find a unique username to use
            var email = userPrincipal.Claims.FirstOrDefault(x => x.Type == "email")?.Value.ToLower();

            if (string.IsNullOrEmpty(email)) { email = string.Empty; }

            return email;
        }

        private (string, string) GetName(ClaimsPrincipal userPrincipal)
        {
            var givenName = userPrincipal.Claims.FirstOrDefault(x => x.Type == "given_name")?.Value;
            var familyName = userPrincipal.Claims.FirstOrDefault(x => x.Type == "family_name")?.Value;

            if (string.IsNullOrEmpty(givenName) && string.IsNullOrEmpty(familyName))
            {
                var fullName = userPrincipal.Claims.FirstOrDefault(x => x.Type == "name")?.Value;
                if (fullName != default) 
                {
                    int spacePos = fullName.IndexOf(' ');
                    if (spacePos > 0)
                    {
                        givenName = fullName.Substring(0, spacePos);
                        familyName = fullName.Substring(spacePos + 1);
                    }                   
                }
            }

            // we don't want to deal with nulls
            if (givenName == null) { givenName = string.Empty; }
            if (familyName == null) { familyName = string.Empty; }
                                    
            return (givenName, familyName);
        }

        #endregion

        /// <summary>
        /// Figure out the event type based on various potiential outcomes and parameters
        /// </summary>
        /// <param name="editedEntity"></param>
        /// <returns><see cref="AuditType"/></returns>
        /// <exception cref="ArgumentException"></exception>
        private AuditType GetEventType(EntityEntry editedEntity)
        {
            // add is easy as that is the type
            if (editedEntity.State == EntityState.Added)
            {
                return AuditType.Create;
            }
            else if (editedEntity.State == EntityState.Deleted)
            {
                return AuditType.Delete;
            }
            else if (editedEntity.State == EntityState.Modified)
            {
                //check to see if IsDeleted state is changing 
                PropertyEntry isDeletedProperty = editedEntity.Property("IsDeleted");
                if((bool)isDeletedProperty.CurrentValue! != (bool)isDeletedProperty.OriginalValue!)
                {
                    if((bool)isDeletedProperty.CurrentValue) 
                    { 
                        return AuditType.Delete; 
                    }
                    else
                    {
                        return AuditType.UnDelete;
                    }
                }

                return AuditType.Update;
            }
            else
            {
                throw new ArgumentException($"Invalid audit type state: '{editedEntity.State}'");
            }
        }

        /// <summary>
        /// Add audit events to all the records that have audit tracking
        /// </summary>
        /// <exception cref="ArgumentException" ></ exception >
        private void AddAuditEvents(Guid currentUserId)
        {
            var auditEvents = new List<AuditEvent>();

            // build lookup table (don't want to include fields we are inheriting)            
            var inheritedProps = typeof(AuditTable).GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public).ToDictionary(x => x.Name, x => x.Name);

            var entities = this.ChangeTracker.Entries().Where(x => x.State == EntityState.Added || x.State == EntityState.Deleted || x.State == EntityState.Modified).ToList();
            foreach (var entity in entities)
            {
                if (entity.Entity is not AuditTable || entity.State == EntityState.Detached || entity.State == EntityState.Unchanged) { continue; }

                var trackedEntity = (AuditTable)entity.Entity;

                // start a new audit entry
                var auditEvent = new AuditEvent
                {
                    Id = DataType.SequentialGuid.NewGuid(),
                    EntityId = trackedEntity.Id,
                    EntityName = entity.Entity.GetType().Name,                    
                    AuditUserId = currentUserId,
                    TraceId = Activity.Current?.Id,
                    EventType = this.GetEventType(entity)
                };

                if (entity.State == EntityState.Added)
                {
                    foreach (var property in entity.Properties)
                    {
                        if (property.Metadata.IsPrimaryKey() || property.IsTemporary) { continue; }
                        if (inheritedProps.ContainsKey(property.Metadata.Name)) { continue; }       //everything is modified when adding

                        //auditEvent.Changes[$"+{property.Metadata.Name}"] = $"{property.CurrentValue}";
                        auditEvent.NewValues[property.Metadata.Name] = property.CurrentValue;
                    }

                    trackedEntity.CreateEvent = auditEvent;
                    trackedEntity.CreateEventId = auditEvent.Id;
                    entity.Property("CreateEventId").IsModified = true;

                    trackedEntity.LastUpdateEvent = auditEvent;
                    trackedEntity.LastUpdateEventId = auditEvent.Id;
                    entity.Property("LastUpdateEventId").IsModified = true;
                }
                else if (auditEvent.EventType == AuditType.Update || auditEvent.EventType == AuditType.UnDelete)
                {
                    foreach (var property in entity.Properties)
                    {
                        if (!property.IsModified || property.Metadata.IsPrimaryKey() || property.IsTemporary) { continue; }
                        if (inheritedProps.ContainsKey(property.Metadata.Name))
                        {
                            property.IsModified = false;    // do not allow outside modifying of inherited Audit tracking properties
                            continue;
                        }

                        // if it is the same then it isn't a edit
                        if(Object.Equals(property.OriginalValue, property.CurrentValue))
                        {
                            property.IsModified = false;
                            continue;
                        }
                                                
                        auditEvent.OldValues[property.Metadata.Name] = property.OriginalValue;
                        auditEvent.NewValues[property.Metadata.Name] = property.CurrentValue;
                    }

                    trackedEntity.LastUpdateEvent = auditEvent;
                    trackedEntity.LastUpdateEventId = auditEvent.Id;
                    entity.Property("LastUpdateEventId").IsModified = true;
                }
                else if (auditEvent.EventType == AuditType.Delete)
                {
                    // don't let the record be deleted.. just flag it as deleted
                    entity.State = EntityState.Modified;
                    entity.Property("IsDeleted").CurrentValue = true;

                    foreach (var property in entity.Properties)
                    {
                        if (property.Metadata.IsPrimaryKey() || property.IsTemporary) { continue; }
                        if (inheritedProps.ContainsKey(property.Metadata.Name))
                        {
                            property.IsModified = false;    // do not allow outside modifying of inherited Audit tracking properties
                            continue;
                        }
                                                
                        auditEvent.OldValues[property.Metadata.Name] = property.OriginalValue;
                    }

                    trackedEntity.LastUpdateEvent = auditEvent;
                    trackedEntity.LastUpdateEventId = auditEvent.Id;
                    entity.Property("LastUpdateEventId").IsModified = true;

                    trackedEntity.DeleteEvent = auditEvent;
                    trackedEntity.DeleteEventId = auditEvent.Id;
                    entity.Property("DeleteEventId").IsModified = true;
                }
                else if (auditEvent.EventType == AuditType.UnDelete)
                {
                    foreach (var property in entity.Properties)
                    {
                        if (!property.IsModified || property.Metadata.IsPrimaryKey() || property.IsTemporary) { continue; }
                        if (inheritedProps.ContainsKey(property.Metadata.Name))
                        {
                            property.IsModified = false;    // do not allow outside modifying of inherited Audit tracking properties
                            continue;
                        }

                        // ARE THEY NEW AGAIN?
                        auditEvent.NewValues[property.Metadata.Name] = $"{property.CurrentValue}";
                    }

                    trackedEntity.LastUpdateEvent = auditEvent;
                    trackedEntity.LastUpdateEventId = auditEvent.Id;
                    entity.Property("LastUpdateEventId").IsModified = true;

                    trackedEntity.DeleteEvent = null;
                    trackedEntity.DeleteEventId = null; //since it is undeleted we don't really have a end of road delete anymore
                    entity.Property("DeleteEventId").IsModified = true;
                }
                else
                {
                    throw new ArgumentException($"Invalid audit type state: '{entity.State}'");
                }

                // NO CHANGE DETECTED TO SAVE OUT
                if (!auditEvent.NewValues.Any() && !auditEvent.OldValues.Any())
                {
                    //nothing to see here folks
                    entity.State = EntityState.Unchanged;
                    continue;
                }

                // make the dates match                
                trackedEntity.LastUpdatedOn = auditEvent.EventOn;
                entity.Property("LastUpdatedOn").IsModified = true;

                // increment the version number since we are saving it out
                trackedEntity.Version = trackedEntity.Version + 1;
                auditEvent.Version = trackedEntity.Version; // make the event match 
                entity.Property("Version").IsModified = true;                
                
                // Add the audit events to the system
                auditEvents.Add(auditEvent);
            }

            this.AuditEvents.AddRange(auditEvents);
        }

        #endregion

        /// <summary>
        /// Output some meaningful connection information for logging
        /// </summary>
        /// <param name="connectionString"></param>
        public static void ConsoleLogDbSettings(string connectionString)
        {
            var dbConn = new DbConnectionStringBuilder() 
            { 
                ConnectionString = connectionString 
            };

            // EXAMPLES:
            //   server=(localdb)\\mssqllocaldb;database=WeatherService;trusted_connection=true;
            //   server=localhost;database=WeatherService;User ID=sa;Password=P@ssw0rd!;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False

            if (dbConn.ContainsKey("Data Source")) { Log.Information("== Data Source: {DataSource}", dbConn["Data Source"]); }
            if (dbConn.ContainsKey("Initial Catalog")) { Log.Information("== Database: {DatabaseCatelog}", dbConn["Initial Catalog"]); }
            if (dbConn.ContainsKey("Integrated Security")) { Log.Information("== Integrated Security: {IntegratedSecurity}", dbConn["Integrated Security"]); }

            if (dbConn.ContainsKey("server")) { Log.Information("== Server: {DatabaseServer}", dbConn["server"]); }
            if (dbConn.ContainsKey("database")) { Log.Information("== Database: {DatabaseName}", dbConn["database"]); }   
            if (dbConn.ContainsKey("User ID")) { Log.Information("== User: {UserId}", dbConn["User ID"]); }            
        }


    }
}
