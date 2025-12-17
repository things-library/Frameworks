// ================================================================================
// <copyright file="ItemStoreType.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.ItemStore.Types
{
    /// <summary>
    /// Different types of entities stores that are suppored
    /// </summary>
    public enum ItemStoreType : short
    {
        Azure_Table = 1,
        GCP_DataStore = 2,
        GCP_BigTable = 3,
        Local = 4,
        LocalFiles = 5,  // File support
        MongoDb = 6,
        Azure_Cosmos = 7
    }
}