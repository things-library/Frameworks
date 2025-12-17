// ================================================================================
// <copyright file="Revision.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using Json.Patch;

namespace ThingsLibrary.ItemStore.Extensions
{
    public static class DataContextExtensions
    {
        public static Revision AddRevision(this ItemEnvelope current, ItemEnvelope original, RootItemDto currentUser)
        {
            // increment if we haven't incremented it yet
            if (current.Revision == original.Revision) { current.IncrementRevision(); }
            
            // only compare the item (data payload)
            var changes = original.Data.CreatePatch(current.Data, SchemaBase.JsonSerializerOptions);

            var revision = new Revision
            {
                PartitionKey = current.partition,
                
                EntityType = typeof(Item).Name,
                EntityId = current.ResourceKey,
                                
                RevisionNumber = current.Revision,
                Changes = changes
            };

            // add current user details            
            revision.SystemMeta[LibraryKeys.Meta.Editor] = currentUser.Name;
            revision.SystemMeta[LibraryKeys.Meta.EditorEmail] = currentUser[LibraryKeys.User.Tag.EmailAddress] ?? "";            
            revision.SystemMeta[LibraryKeys.Meta.EditorIP] = currentUser[LibraryKeys.User.Tag.IpAddress] ?? "";

            return revision;
        }

        public static Revision AddRevision(this ItemTypeEnvelope current, ItemTypeEnvelope original, RootItemDto currentUser)
        {
            // increment if we haven't incremented it yet
            if (current.Revision == original.Revision) { current.IncrementRevision(); }

            var changes = original.Data.CreatePatch(current.Data, SchemaBase.JsonSerializerOptions);

            var revision = new Revision
            {
                PartitionKey = current.PartitionKey,

                EntityType = typeof(ItemType).Name,
                EntityId = current.Id,
                
                RevisionNumber = current.Revision,
                Changes = changes
            };

            // add current user details            
            revision.SystemMeta[LibraryKeys.Meta.Editor] = currentUser.Name;
            revision.SystemMeta[LibraryKeys.Meta.EditorEmail] = currentUser[LibraryKeys.User.Tag.EmailAddress] ?? "";            
            revision.SystemMeta[LibraryKeys.Meta.EditorIP] = currentUser[LibraryKeys.User.Tag.IpAddress] ?? "";
            
            return revision;
        }
    }
}
