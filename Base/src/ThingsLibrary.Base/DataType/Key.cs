// ================================================================================
// <copyright file="Key.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using System.Text.RegularExpressions;

namespace ThingsLibrary.DataType
{
    public static class Key
    {
        public static bool IsValidJsonKey(string key)
        {
            if (string.IsNullOrEmpty(key)) { return false; }

            return Regex.IsMatch(key, "^[a-z0-9_-]+$");
        }

        public static string GetJsonKey(string name)
        {
            if (string.IsNullOrEmpty(name)) { throw new ArgumentNullException(name); }

            return Regex.Replace(name.ToLower(), @"[^a-z0-9_-]+", "_");
        }
    }
}
