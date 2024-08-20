using System.Diagnostics;

using Starlight.Database.Audit.Models;
using Starlight.DataType.Extensions;

namespace Starlight.Entity.MongoAudit
{
    /// <summary>
    /// Mongo Collection Wrapper
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AuditEntityStore<T> : Mongo.EntityStore<T> where T : class
    {
        private EntityStoreOptions Options { get; init; }
                
        private IClaimsPrincipalAccessor ClaimsPrincipalAccessor { get; init; }

        /// <summary>
        /// If the table is under audit records retention
        /// </summary>
        public bool IsAuditTable { get; set; } = false;

        /// <summary>
        /// Collection of Audit Events for the current table
        /// </summary>
        public Mongo.EntityStore<Models.AuditEvent>? AuditEvents { get; set; }

        /// <summary>
        /// Collection of Users that have editing something
        /// </summary>
        public Mongo.EntityStore<Models.AuditUser>? AuditUsers { get; set; }


        /// <summary>
        /// The current audit user
        /// </summary>
        public Models.AuditUser? CurrentUser { get; set; } = null;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString">Storage Connection String</param>
        /// <param name="databaseName">Database Name</param>
        /// <param name="name">Table Name</param>
        /// <exception cref="ArgumentNullException">Storage connection string, table name must not be null.</exception>
        /// <exception cref="ArgumentException">Table name must strat with a latter, be alphanumeric and be 3-63 characters</exception>
        public AuditEntityStore(EntityStoreOptions options, string name, IClaimsPrincipalAccessor claimsPrincipalAccessor) : base(options, name)
        {
            this.Options = options;
            this.ClaimsPrincipalAccessor = claimsPrincipalAccessor;

            this.IsAuditTable = this.Type.BaseType == typeof(Models.AuditTable);           
        }
                   
        /// <inheritdoc />
        public override async Task InsertEntityAsync(T entity, CancellationToken cancellationToken = default)
        {
            // see if we need to keep track of the revisions
            if (this.IsAuditTable)
            {
                // get ready to add audit records
                await this.InitAuditRecordsAsync(cancellationToken);

                await this.InsertAuditEventAsync(entity, AuditType.Created, cancellationToken);
            }

            // now call the base insert
            await base.InsertEntityAsync(entity, cancellationToken);
        }

        /// <inheritdoc />
        public override async Task UpsertEntityAsync(T entity, CancellationToken cancellationToken = default)
        {
            if (this.IsAuditTable)
            {
                // get ready to add audit records
                await this.InitAuditRecordsAsync(cancellationToken);

                await this.InsertAuditEventAsync(entity, AuditType.Updated, cancellationToken);
            }      

            // now call the base upsert
            await base.UpsertEntityAsync(entity, cancellationToken);

        }

        /// <inheritdoc />
        public override async Task UpdateEntityAsync(T entity, CancellationToken cancellationToken = default)
        {
            // see if we need to keep track of the revisions
            if (this.IsAuditTable)
            {
                // get ready to add audit records
                await this.InitAuditRecordsAsync(cancellationToken);

                await this.InsertAuditEventAsync(entity, AuditType.Updated, cancellationToken);
            }
            
            await base.UpdateEntityAsync(entity, cancellationToken);
        }

        /// <inheritdoc />
        public override async Task BulkUpsertAsync(IEnumerable<T> records, CancellationToken cancellationToken = default)
        {
            // see if we need to keep track of the revisions
            if (this.IsAuditTable)
            {
                // get ready to add audit records
                await this.InitAuditRecordsAsync(cancellationToken);

                throw new NotImplementedException();
            }
            await base.BulkUpsertAsync(records, cancellationToken);
        }

        /// <inheritdoc />
        public override Task DeleteEntityAsync(object key, object partitionKey, CancellationToken cancellationToken = default)
        {
            return this.DeleteEntityAsync(key, partitionKey, false, cancellationToken);
        }


        /// <inheritdoc />
        public override async Task DeleteEntityAsync(object key, object partitionKey, bool errorIfMissing, CancellationToken cancellationToken = default)
        {
            if (this.IsAuditTable)
            {
                // get ready to add audit records
                await this.InitAuditRecordsAsync(cancellationToken);

                var oldEntity = await base.GetEntityAsync(key, partitionKey, cancellationToken);
                
                await this.InsertAuditEventAsync(oldEntity, AuditType.Deleted, cancellationToken);

                await base.UpsertEntityAsync(oldEntity, cancellationToken);                
            }
            else
            {
                await base.DeleteEntityAsync(key, partitionKey, errorIfMissing, cancellationToken);
            }            
        }


        private async Task InitAuditRecordsAsync(CancellationToken cancellationToken = default)
        {
            if (!this.IsAuditTable) { throw new ArgumentException("Audit Table inheritence not detected."); }
            if (this.CurrentUser != null) { return; }
            
            // get the companion tables that we will need in the event of any editing           
            this.AuditEvents = new Mongo.EntityStore<Models.AuditEvent>(this.Options, $"{this.Name}_history");
            this.AuditUsers = new Mongo.EntityStore<Models.AuditUser>(this.Options, "audit_users");
            

            var userPrincipal = this.ClaimsPrincipalAccessor.Principal;

            Models.AuditUser? auditUser = null;

            var userId = this.GetUserId(userPrincipal);            
            if(userId == default) { throw new ArgumentException("Unable to determine unique User ID"); }

            var username = this.GetUsername(userPrincipal);
            if (username == default) { throw new ArgumentException("Unable to find username within claims principal."); }

            var name = this.GetName(userPrincipal);


            // see if we can find the user based on the USER ID
            auditUser = await this.AuditUsers.GetEntityAsync(userId, "", cancellationToken);
            if (auditUser == default)
            {
                this.CurrentUser = new Models.AuditUser
                {
                    Id = userId,
                    Username = username,
                    Name = name,
                    Timestamp = DateTime.UtcNow
                };
                await this.AuditUsers.InsertEntityAsync(this.CurrentUser, cancellationToken);

                return;
            }
            else
            {
                this.CurrentUser = auditUser;
            }

            // DO WE NEED TO UPDATE THE CURRENT USER?
            if(string.Compare(this.CurrentUser.Username, username) != 0 || string.Compare(this.CurrentUser.Name, name) != 0)
            {
                this.CurrentUser.Username = username;
                this.CurrentUser.Name = name;

                await this.AuditUsers.UpdateEntityAsync(this.CurrentUser, cancellationToken);
            }            
        }


        private async Task InsertAuditEventAsync(T entity, AuditType auditType, CancellationToken cancellationToken = default)
        {            
            var entityId = this.Key.GetPropertyValue<string>(entity);
            var version = this.RevisionNumber.GetPropertyValue<int>(entity) + 1;    // +1 because we are doing this BEFORE saving the root entity (which auto increments the version number)

            // see if we are wrong on the type of edit we are doing (aka: upsert like action)
            if(auditType == AuditType.Updated && !await this.ExistsAsync(entityId, "", cancellationToken)) { auditType = AuditType.Created; }

            var auditEvent = new AuditEvent
            {
                Id = $"{entityId}:{version}",
                EntityId = $"{entityId}",
                Version = version,      

                Changes = new Dictionary<string, object>(), //TODO: 

                UserId = this.CurrentUser!.Id,
                Username = this.CurrentUser!.Username,
                TraceId = Activity.Current?.Id,
                EventType = auditType
            };

            var type = entity.GetType();
            if (auditType == AuditType.Created)
            {
                type.GetProperty("CreateEventId")?.SetValue(entity, auditEvent.Id);
            }
            type.GetProperty("LastUpdateEventId")?.SetValue(entity, auditEvent.Id);

            if (auditType == AuditType.Deleted)
            {
                type.GetProperty("IsDeleted")?.SetValue(entity, true);
                type.GetProperty("DeleteEventId")?.SetValue(entity, auditEvent.Id);
            }
                   
            // add the audit event to the events table
            await this.AuditEvents!.InsertEntityAsync(auditEvent, cancellationToken);            
        }

        #region --- User Properties ---

        private string? GetUserId(ClaimsPrincipal userPrincipal)
        {
            // try to find the user based on the object id (if exists)
            var userId = userPrincipal.Claims.SingleOrDefault(c => c.Type == "sub")?.Value.ToLower();
            if (userId == null) { userId = userPrincipal.Claims.SingleOrDefault(c => c.Type == "oid")?.Value.ToLower(); }
                        
            return userId;
        }

        private string? GetUsername(ClaimsPrincipal userPrincipal)
        {
            // find a unique username to use
            var username = userPrincipal.Claims.FirstOrDefault(x => x.Type == "preferred_username")?.Value.ToLower();
            if (username == default) { username = userPrincipal.Claims.FirstOrDefault(x => x.Type == "upn")?.Value.ToLower(); }
            if (username == default) { username = userPrincipal.Claims.FirstOrDefault(x => x.Type == "username")?.Value.ToLower(); }
            if (username == default) { username = userPrincipal.Claims.FirstOrDefault(x => x.Type == "email")?.Value.ToLower(); }

            return username;
        }

        private string? GetName(ClaimsPrincipal userPrincipal)
        {
            // find a unique username to use
            var name = userPrincipal.Claims.FirstOrDefault(x => x.Type == "name")?.Value;
            if (name != default) { return name; }

            var givenName = userPrincipal.Claims.FirstOrDefault(x => x.Type == "given_name")?.Value;
            var familyName = userPrincipal.Claims.FirstOrDefault(x => x.Type == "family_name")?.Value;
                
            if(givenName != default || familyName != default)
            {
                return $"{givenName} {familyName}".Trim();
            }
                          
            // must not have found it
            return default;
        }

        #endregion

        #region --- IDisposable ---

        private bool _disposed = false;

        //public void Dispose()
        //{
        //    this.Dispose(true);
        //    GC.SuppressFinalize(this);
        //}

        protected override void Dispose(bool disposing)
        {
            // Cleanup
            if (_disposed) { return; }

            _disposed = true;   // we are handling the disposing in this thread so don't let others do the same thing

            if (disposing)
            {
                // free up objects
                this.AuditEvents.Dispose();
                this.AuditUsers.Dispose();
            }

            base.Dispose(disposing);
        }

        ~AuditEntityStore()
        {
            this.Dispose(false);
        }

        #endregion
    }
}