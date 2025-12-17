// ================================================================================
// <copyright file="Item.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.ItemStore.Extensions
{
    public static class ItemDtoExtensions
    {
        public static RootItemDto ToItemsDto(this IEnumerable<ItemEnvelope> results, bool includeMetadata)
        {
            return new RootItemDto
            {
                Type = LibraryTypes.Library,

                Key = LibraryKeys.Item.Results,
                Name = "Results",
                Items = results.OrderBy(x => x.Data.Name).ToDictionary(x => x.ResourceKey.Split('/')[^1], x => (ItemDto)x.ToItemDto(includeMetadata))
            };
        }


        public static RootItemDto ToItemDto(this ItemEnvelope envelope, bool includeMetadata)
        {
            var key = envelope.ResourceKey.Split('/')[^1];
            var item = envelope.Data;

            var itemDto = new RootItemDto(envelope.Type, item.Name, key)
            {
                Date = item.Date,
                Tags = item.Tags
            };

            if (includeMetadata)
            {
                itemDto.Meta = item.Meta;
                itemDto.Meta.AddRange(envelope.SystemMeta, true);

                itemDto.Meta.Add(LibraryKeys.Meta.Id, envelope.ResourceKey.ToString());
                itemDto.Meta.Add(LibraryKeys.Meta.Revision, envelope.Revision.ToString());

                itemDto.Meta.Add(LibraryKeys.Meta.Created, envelope.CreatedOn.ToString("O"));
                itemDto.Meta.Add(LibraryKeys.Meta.Updated, envelope.UpdatedOn.ToString("O"));
            }

            // always include the deleted flag
            if (envelope.IsRootItem) { itemDto.Meta.Add(LibraryKeys.Meta.RootType, LibraryKeys.True); }
            if (envelope.IsDeleted) { itemDto.Meta.Add(LibraryKeys.Meta.IsDeleted, LibraryKeys.True); }
            if (envelope.IsSystem) { itemDto.Meta.Add(LibraryKeys.Meta.IsSystem, LibraryKeys.True); }

            return itemDto;
        }

        public static List<ItemEnvelope> ToEnvelopes(this ItemDto itemDto, string partitionKey, string resourceKey)
        {
            var envelopes = new List<ItemEnvelope>()
            {
                itemDto.ToEnvelope(partitionKey, resourceKey)
            };
            
            foreach(var childItem in itemDto.Items) 
            {
                var childResourceKey = $"{resourceKey}/{childItem.Key}";
                envelopes.AddRange(childItem.Value.ToEnvelopes(partitionKey, childResourceKey));
            }

            return envelopes;
        }

        public static ItemEnvelope ToEnvelope(this ItemDto itemDto, string partitionKey, string resourceKey)
        {
            return new ItemEnvelope
            {                
                partition = partitionKey,
                ResourceKey = resourceKey,
                Type = itemDto.Type,

                Data = new Item(itemDto.Name, itemDto.Date)
                {
                    Tags = itemDto.Tags,
                    Meta = itemDto.Meta.Where(x => !x.Key.StartsWith('$')).ToDictionary()    // keep whatever metadata was provided minus system keys
                },

                // pull out all the system metadata
                SystemMeta = itemDto.Meta.Where(x => x.Key.StartsWith('$')).ToDictionary()
            };
        }
    

        /// <summary>
        /// Takes a listing of items and puts them back into the tree hierarchy of parent/child relationships
        /// </summary>
        /// <param name="items"></param>
        /// <param name="rootKey"></param>
        /// <param name="includeMetadata"></param>
        /// <returns></returns>
        public static RootItemDto ToTree(this List<ItemEnvelope> items, string rootKey, bool includeMetadata)
        {
            var itemDtos = items.ToDictionary(x => x.ResourceKey, x => x.ToItemDto(includeMetadata));

            if (!itemDtos.ContainsKey(rootKey)) { return new RootItemDto(); }

            var rootItem = itemDtos[rootKey];

            foreach (var pair in itemDtos)
            {
                var pathParts = pair.Key.Split('/');

                var parentPath = string.Join('/', pathParts[..^1]);
                var key = pathParts[pathParts.Length - 1];

                // ROOT CHECK (aka: already attached)
                if (pair.Key == rootKey)
                {
                    rootItem.Key = key;
                    continue;
                }

                if (itemDtos.TryGetValue(parentPath, out var parentItem))
                {
                    parentItem.Items[key] = pair.Value;
                }
                else
                {
                    Debug.WriteLine("ERROR: unable to find parent!!!");
                }
            }

            return rootItem;
        }

        public static string ToTagName(this RootItemDto libraryDto, string typeKey, string tagKey, string defaultValue)
        {
            if (libraryDto.Types.TryGetValue(typeKey, out var typeDto))
            {
                if (typeDto.Tags.TryGetValue(tagKey, out var tagDto))
                {
                    return tagDto.Name;
                }
                else
                {
                    return defaultValue;
                }
            }
            else
            {
                return defaultValue;
            }
        }

        public static string ToTagValue(this RootItemDto libraryDto, string typeKey, string tagKey, string tagRawValue)
        {
            return tagRawValue;
        }


        /// <summary>
        /// Convert the item to a root item by adding the key
        /// </summary>
        /// <param name="itemDto">Item</param>
        /// <param name="key">Item Key</param>
        /// <returns></returns>
        public static RootItemDto ToRootItem(this ItemDto itemDto, string itemKey)
        {
            if (itemKey.Contains('/'))
            {
                // only take the last part of the key
                itemKey = itemKey.Split('/')[^1];
            }

            return new RootItemDto(itemKey, itemDto);
        }
        
        /// <summary>
        /// Check the library for valid permissions
        /// </summary>                
        /// <param name="libraryDto">Library</param>
        /// <param name="username">Username</param>
        /// <param name="allowedRoles">Allowed roles</param>
        /// <returns></returns>
        public static bool IsUnauthorized(this RootItemDto libraryDto, string username, params List<string>? allowedRoles)        
        {
            // the OPPOSITE!!
            return !libraryDto.IsAuthorized(username, allowedRoles);
        }

        /// <summary>
        /// Check the library for valid permissions
        /// </summary>
        /// <param name="libraryDto">Library</param>
        /// <param name="username">Username</param>
        /// <param name="allowedRoles">Allowed roles</param>
        /// <returns></returns>
        public static bool IsAuthorized(this RootItemDto libraryDto, string username, params List<string>? allowedRoles)
        {
            // if we don't yet have a current library then we are authorized to mess with it until we do
            if (libraryDto.IsInvalid()) { return true; }

            // even the right type?
            if (libraryDto.Type != LibraryTypes.Library) { return false; }
            

            var userKey = SchemaBase.GenerateKey(username);

            if (libraryDto.TryGetItem($"{LibraryKeys.Item.Users}/{userKey}", out var libraryUser))
            {
                // are we just looking that they have access???
                if(allowedRoles == null || allowedRoles.Count == 0) { return true; }

                // ROLES are encoded as ENUM so   "roles": "|owner|manager|"
                var roles = libraryUser[LibraryKeys.User.Tag.Roles] ?? "";

                // see if any of the allowed roles are found in our current library roles listing
                if (allowedRoles.Any(x => roles.Contains($"|{x}|"))) { return true; }
                            
                // unauthorized
                return false;
            }
            else
            {
                // unauthorized.. not even found in the list of users
                return false;
            }            
        }

        ///// <summary>
        ///// Flattens a DTO tree hierarcyy into individual DTO items with a resource key 
        ///// </summary>
        ///// <param name="parentItem"></param>
        ///// <param name="parentKey"></param>
        ///// <returns></returns>
        ///// <remarks>(WARNING: KEYS ARE REFERENCE KEYS NOT CHILD KEYS)</remarks>
        //public static Dictionary<string, ItemDto> Flatten(this ItemDto parentItem, string parentKey)
        //{
        //    var items = new Dictionary<string, ItemDto>(parentItem.Items.Count);

        //    if (parentItem.Type != LibraryTypes.Library)
        //    {
        //        items.Add(parentKey, parentItem);
        //    }

        //    // add all the children
        //    foreach (var childItem in parentItem.Items)
        //    {
        //        items.AddRange(Flatten(childItem.Value, $"{parentKey}/{childItem.Key}"));
        //    }

        //    return items;
        //}


        /// <summary>
        /// Get all the items grouped by type key
        /// </summary>
        /// <param name="rootItem">Item to get all type items (including all children)
        /// </param>
        /// <returns></returns>
        public static Dictionary<string, List<ItemDto>> GetTypeItems(this ItemDto rootItem)
        {
            var items = new Dictionary<string, List<ItemDto>>();

            if (rootItem.Type != LibraryTypes.Library)
            {
                if (!items.ContainsKey(rootItem.Type)) { items[rootItem.Type] = new List<ItemDto>(); }

                items[rootItem.Type].Add(rootItem);
            }

            // add all the children
            foreach (var childItem in rootItem.Items)
            {
                var children = childItem.Value.GetTypeItems();
                foreach(var pair in children)
                {
                    if (!items.ContainsKey(pair.Key)) { items[pair.Key] = new List<ItemDto>(); }

                    items[pair.Key].AddRange(pair.Value);                   
                }
            }

            return items;
        }


        /// <summary>
        /// Add system root flags for root types
        /// </summary>
        /// <param name="rootItem"></param>
        public static void UpdateRootTypes(this RootItemDto rootItem)
        {            
            if (rootItem.Type != LibraryTypes.Library)
            {
                rootItem.Types[rootItem.Type].Meta[LibraryKeys.Meta.RootType] = "true";               
            }
            else
            {
                foreach (var item in rootItem.Items)
                {
                    rootItem.Types[item.Value.Type].Meta[LibraryKeys.Meta.RootType] = "true";
                }
            }
        }

        public static void Add(this RootItemDto rootItem, RootItemDto importItem)
        {
            if(rootItem.Type != LibraryTypes.Library) { throw new ArgumentException("Root item must be a library."); }

            // only add if missing
            foreach(var type in importItem.Types)
            {
                if (!rootItem.Types.ContainsKey(type.Key))
                {
                    rootItem.Types[type.Key] = type.Value;
                }
                else
                {
                    rootItem.Types[type.Key].Merge(type.Value);                    
                }
            }

            foreach(var item in importItem.Items)
            {
                if (rootItem.Items.ContainsKey(item.Key)) { throw new ArgumentException($"Item already exists for key '{item.Key}'."); }

                rootItem.Items[item.Key] = item.Value;
            }
        }

        public static ItemDto AttachUser(this RootItemDto libraryDto, string emailAddress, string name, List<string> roles)
        {
            ArgumentNullException.ThrowIfNull(emailAddress);
            ArgumentNullException.ThrowIfNull(name);
            if(roles.Count == 0) { throw new ArgumentException("No roles specified"); }

            // GET OR CREATE USERS
            ItemDto usersDto;
            if (!libraryDto.Items.ContainsKey(LibraryKeys.Item.Users))
            {
                usersDto = new ItemDto(LibraryTypes.Users, "Users", null);
                libraryDto.Items[LibraryKeys.Item.Users] = usersDto;
            }
            else
            {
                usersDto = libraryDto.Items[LibraryKeys.Item.Users];
            }

            var key = SchemaBase.GenerateKey(emailAddress);
            var userDto = new ItemDto(LibraryTypes.User, name)
            {
                Tags = new Dictionary<string, string>
                {
                    { LibraryKeys.User.Tag.EmailAddress, emailAddress },
                    { LibraryKeys.User.Tag.Roles, $"|{string.Join('|', roles)}|" }
                }
            };
            usersDto.Items[key] = userDto;

            return userDto;
        }
    }
}
