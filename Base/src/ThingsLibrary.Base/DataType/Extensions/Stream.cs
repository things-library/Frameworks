// ================================================================================
// <copyright file="Stream.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Base.DataType.Extensions
{
    public static class StreamExtensions
    {
        public static string ToStringData(this Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();                
            }
        }
    }
}
