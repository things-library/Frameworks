// ================================================================================
// <copyright file="ItemTypeTag.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.ItemStore.Extensions
{
    public static class ItemTpeTagExtensions
    {
        public static ItemTypeTagDto ToTypeTagDto(this ItemTypeTag itemTypeTag, bool includeMetadata = false)
        {
            var itemTypeTagDto = new ItemTypeTagDto
            {
                Name = itemTypeTag.Name,
                Type = itemTypeTag.Type,

                Units = itemTypeTag.Units,
                Sequence = itemTypeTag.Sequence,

                Values = itemTypeTag.Values               
            };

            if (includeMetadata)            
            {
                itemTypeTagDto.Meta = itemTypeTag.Meta;                
            }

            // always include the system flags
            if (itemTypeTag.IsListView) { itemTypeTagDto.Meta.Add(LibraryKeys.Meta.ListView, LibraryKeys.True); }
            if (itemTypeTag.IsRequired) { itemTypeTagDto.Meta.Add(LibraryKeys.Meta.Required, LibraryKeys.True); }

            return itemTypeTagDto;
        }

        /// <summary>
        /// Is the field required?
        /// </summary>
        /// <param name="itemTypeTag"></param>
        /// <returns></returns>
        public static bool IsRequired(this ItemTypeTagDto itemTypeTag)
        {
            return (string.Compare(itemTypeTag[LibraryKeys.Meta.Required, true], LibraryKeys.True, true) == 0);
        }

        /// <summary>
        /// Is this tag a list view tag?
        /// </summary>
        /// <param name="itemTypeTag"></param>
        /// <returns></returns>
        public static bool IsListView(this ItemTypeTagDto itemTypeTag)
        {
            return (string.Compare(itemTypeTag[LibraryKeys.Meta.ListView, true], LibraryKeys.True, true) == 0);
        }                
    }
}
