// ================================================================================
// <copyright file="FileStoreType.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Storage
{
    /// <summary>
    /// Storage System Type
    /// </summary>
    public enum FileStoreType : byte
    {
        Azure_Blob = 1,
        AWS_S3 = 2,
        GCP_Storage = 3,
        Wasabi = 4,
        Local = 5
    }
}
