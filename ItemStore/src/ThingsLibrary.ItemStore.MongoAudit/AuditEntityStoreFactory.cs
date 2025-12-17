// ================================================================================
// <copyright file="AuditEntityStoreFactory.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using System.Security.Authentication;
using MongoDB.Driver.Core.Configuration;
using ThingsLibrary.DataType;
using ThingsLibrary.Entity.Interfaces;
using ThingsLibrary.Entity.Mongo.Models;
using ThingsLibrary.Entity.Types;
using ThingsLibrary.Security.OAuth2.Interfaces;

namespace ThingsLibrary.Entity.MongoAudit;

/// <summary>
/// Mongo Entity Store Wrapper
/// </summary>
/// <remarks>
/// Reference: https://www.mongodb.com/blog/post/quick-start-c-sharp-and-mongodb-starting-and-setup
/// </remarks>
public class AuditEntityStoreFactory : Mongo.EntityStoreFactory
{
    private IClaimsPrincipalAccessor ClaimsPrincipalAccessor { get; init; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="connectionString">Connection String</param>
    /// <param name="defaultDatabaseName">Database Name</param>
    public AuditEntityStoreFactory(Mongo.Models.EntityStoreOptions options, IClaimsPrincipalAccessor claimsPrincipalAccessor) : base(options)
    {
        this.ClaimsPrincipalAccessor = claimsPrincipalAccessor;
    }

    /// <inheritdoc />
    public override IEntityStore<T> GetStore<T>(string name) where T : class
    {
        return new AuditEntityStore<T>(this.Options, name, this.ClaimsPrincipalAccessor);
    }

    public override AuditEntityStore<T> GetStoreObj<T>(string name) where T : class
    {
        return new AuditEntityStore<T>(this.Options, name, this.ClaimsPrincipalAccessor);
    }

    /// <summary>
    /// Get store for database name
    /// </summary>
    /// <typeparam name="T">Stored Entity</typeparam>
    /// <param name="databaseName">Database Name</param>
    /// <param name="name">Store Name</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public override IEntityStore<T> GetStore<T>(string databaseName, string name) where T : class
    {
        _ = Guard.Argument(name, nameof(name))
           .NotEmpty()
           .NotNull();

        ValidateDatabaseName(databaseName);

        var options = new EntityStoreOptions(this.Options.ConnectionString, databaseName);

        return new AuditEntityStore<T>(options, name, this.ClaimsPrincipalAccessor);
    }   
}