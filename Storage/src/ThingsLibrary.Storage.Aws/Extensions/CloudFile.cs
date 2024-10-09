// ================================================================================
// <copyright file="CloudFile.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using Amazon.S3.Model;

namespace ThingsLibrary.Storage.Aws.Extensions
{
    public static class CloudFileExtensions
    {
        public static FileItem ToCloudFile(this GetObjectResponse awsObj)
        {
            throw new NotImplementedException();
        }
    }
}
