// ================================================================================
// <copyright file="ItemType.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.ItemStore.Extensions
{
    public static class ItemTypeExtensions
    {
        public static RootItemDto ToItemTypesDto(this IEnumerable<ItemTypeEnvelope> results, bool includeSystemMetadata)
        {
            return new RootItemDto
            {
                Type = LibraryTypes.Library,

                Key = LibraryKeys.Item.Results,
                Name = "Types",
                Types = results.OrderBy(x => x.Data.Name).ToDictionary(x => x.Id.Split('/')[^1], x => x.ToItemTypeDto(includeSystemMetadata))                
            };
        }

        public static ItemTypeDto ToItemTypeDto(this ItemTypeEnvelope envelope, bool includeSystemMetadata)
        {
            var itemTypeDto = new ItemTypeDto(envelope.Data.Name)
            {
                Description = envelope.Data.Description,

                Tags = envelope.Data.Tags.ToDictionary(k => k.Key, v => v.ToTypeTagDto()),
                Items = envelope.Data.Items.ToDictionary(k => k.Key, v => v.ToTypeAttachmentDto(includeSystemMetadata)),
                Meta = envelope.Data.Meta
            };

            if (includeSystemMetadata)
            {
                itemTypeDto.Meta.Add(LibraryKeys.Meta.Id, envelope.Id.ToString());
                itemTypeDto.Meta.Add(LibraryKeys.Meta.Revision, envelope.Revision.ToString());
                
                itemTypeDto.Meta.Add(LibraryKeys.Meta.Created, envelope.CreatedOn.ToString("O"));
                itemTypeDto.Meta[LibraryKeys.Meta.Creator] = envelope[LibraryKeys.Meta.Creator] ?? "";

                itemTypeDto.Meta.Add(LibraryKeys.Meta.Updated, envelope.UpdatedOn.ToString("O"));
                itemTypeDto.Meta[LibraryKeys.Meta.Editor] = envelope[LibraryKeys.Meta.Editor] ?? "";
            }

            // always include the system flags
            if (envelope.IsRootType) { itemTypeDto.Meta.Add(LibraryKeys.Meta.RootType, LibraryKeys.True); }
            if (envelope.IsDeleted) { itemTypeDto.Meta.Add(LibraryKeys.Meta.IsDeleted, LibraryKeys.True); }
            if (envelope.IsSystem) { itemTypeDto.Meta.Add(LibraryKeys.Meta.IsSystem, LibraryKeys.True); }            

            return itemTypeDto;
        }

        public static void Merge(this ItemTypeDto rootType, ItemTypeDto importType)
        {            
            foreach (var typeTag in importType.Tags)
            {
                if (rootType.Tags.ContainsKey(typeTag.Key)) { continue; }

                rootType.Tags[typeTag.Key] = typeTag.Value;
            }

            foreach (var typeItem in importType.Items)
            {
                if (rootType.Items.ContainsKey(typeItem.Key)) { continue; }

                rootType.Items[typeItem.Key] = typeItem.Value;
            }
        }

        public static bool IsRootType(this ItemTypeDto itemTypeDto)
        {
            if (itemTypeDto.Meta.TryGetValue(LibraryKeys.Meta.RootType, out var isRoot))
            {
                return isRoot == LibraryKeys.True;
            }
            else
            {
                return false;
            }
        }

        public static bool IsSystem(this ItemTypeDto itemTypeDto)
        {
            return itemTypeDto.Name.StartsWith('$');            
        }

        public static string ToDisplayValue(this ItemTypeDto itemTypeDto, KeyValuePair<string, string> pair)
        {
            return itemTypeDto.ToDisplayValue(pair.Key, pair.Value);
        }    

        public static string ToDisplayValue(this ItemTypeDto itemTypeDto, string tagKey, string tagValue)
        {
            if (!itemTypeDto.Tags.ContainsKey(tagKey))
            {
                return tagValue;    //TODO: convert to data type and present properly
            }

            var typeTag = itemTypeDto.Tags[tagKey];
            if (!string.IsNullOrEmpty(typeTag.Units))
            {
                return $"{tagValue} ({typeTag.Units})";
            }
            else
            {
                return tagValue;    //TODO: convert to data type and present properly
            }
        }
    }
}
