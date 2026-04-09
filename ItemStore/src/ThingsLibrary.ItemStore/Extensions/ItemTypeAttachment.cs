// ================================================================================
// <copyright file="ItemTypeAttachment.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.ItemStore.Extensions
{
    public static class ItemTpeAttachmentExtensions
    {
        public static ItemTypeAttachmentDto ToTypeAttachmentDto(this ItemTypeAttachment itemAttachment, bool includeMetadata)
        {
            var itemDto = new ItemTypeAttachmentDto
            {
                Type = itemAttachment.Type,
                Name = itemAttachment.Name,
                Description = itemAttachment.Description,

                Sequence = itemAttachment.Sequence,

                Meta = itemAttachment.Meta
            };

            return itemDto;
        }
    }
}
