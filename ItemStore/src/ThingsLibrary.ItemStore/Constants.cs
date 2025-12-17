// ================================================================================
// <copyright file="Constants.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using System.Data;

namespace ThingsLibrary.Constants
{
    public static class EventCodes
    {
        public static readonly string KEY_INVALID = "KEY_INVALID";
        public static readonly string TYPE_INVALID = "TYPE_INVALID";

        public static readonly string UNAUTHORIZED = "UNAUTHORIZED";
        
        public static readonly string RECORD_INVALID = "RECORD_INVALID";
        public static readonly string RECORD_FETCH_ERROR = "RECORD_FETCH_ERROR";
        public static readonly string RECORD_NOT_FOUND = "RECORD_NOT_FOUND";
        public static readonly string RECORD_ALREADY_EXISTS = "RECORD_ALREADY_EXISTS";
        public static readonly string RECORD_CREATED = "RECORD_CREATED";
        public static readonly string RECORD_CREATE_ERROR = "RECORD_CREATE_ERROR";
        public static readonly string RECORD_UPDATED = "RECORD_UPDATED";
        public static readonly string RECORD_UPDATE_ERROR = "RECORD_UPDATE_ERROR";
        public static readonly string RECORD_DELETED = "RECORD_DELETED";
    }

    public static class EventMessages
    {        
        public static readonly string NOTHING_CHANGED = "Nothing Changed";
        public static readonly string UNAUTHORIZED =    "Invalid Permissions";

        public static readonly string RECORD_EXISTS = "Record Already Exists";
        public static readonly string RECORD_NOT_FOUND = "Record Not Found";
        public static readonly string RECORD_DELETED = "Record Deleted";
        public static readonly string RECORD_CREATED = "Record Created";
        public static readonly string RECORD_UPDATED = "Record Updated";

        public static readonly string ITEM_TYPE_NOT_FOUND = "Item Type '{0}' Not Found";

        public static readonly string LIBRARY_NOT_FOUND = "Library '{0}' Not Found";

    }

    //Examples:   Keys.Item.Type.Library

    public static class LibraryTypes
    {
        public static readonly string Library = "$library";
        public static readonly string Users = "$users";
        public static readonly string User = "$user";
        public static readonly string Roles = "$roles";
        public static readonly string Role = "$role";

        public static readonly string Session = "$session";
        public static readonly string File = "$file";
        public static readonly string Meta = "$meta";
    }


    public static class LibraryKeys
    {
        public static readonly string True = "true";
        public static readonly string False = "false";

        public static class Resource
        {
            public static string Key(string libraryKey, string itemKey) => (!string.IsNullOrEmpty(itemKey) ? $"{libraryKey}/{itemKey}" : libraryKey);

            public static string? Parent(string itemKey)
            {
                if(string.IsNullOrEmpty(itemKey)) { return null; }

                var paths = itemKey.Split('/');
                if(paths.Length <= 1) { return null; }  // there is no parent

                return string.Join('/', paths.Take(paths.Length - 1));
            }
        }
        
        public static class App
        {
            /// <summary>
            /// System tags that a app can have
            /// </summary>
            public static class Tag
            {
                public static readonly string Id = "$app_id";
                public static readonly string Name = "$app_name";
            }
        }

        /// <summary>
        /// System types that a library item can have
        /// </summary>
        public static class Item
        {
            public static readonly string Library = "$library";
            public static readonly string Users = "$users";
            public static readonly string Roles = "$roles";
            public static readonly string Results = "$results";
        }

        public static class User
        {   
            /// <summary>
            /// System Tags that a User item can have
            /// </summary>
            public static class Tag
            {
                public static readonly string Id = "$id";
                public static readonly string Name = "$name";
                public static readonly string GivenName = "$given_name";
                public static readonly string FamilyName = "$family_name";

                public static readonly string JobTitle = "$title";
                public static readonly string Company = "$company";

                public static readonly string PostalCode = "$postal_code";
                public static readonly string City = "$city";
                public static readonly string State = "$state";
                public static readonly string Country = "$country";

                public static readonly string EmailAddress = "$email";
                public static readonly string Phone = "$phone";
                public static readonly string PreferredLanguage = "$language";
                public static readonly string TimeZone = "$tz";

                public static readonly string Roles = "$roles";

                public static readonly string IpAddress = "$ip_address";
            }

            public static class Roles
            {
                // LIBRARY LEVEL
                public static readonly string Owner = "owner";
                public static readonly string Manager = "manager";
                public static readonly string Borrower = "borrower";
                public static readonly string Viewer = "viewer";

                public static Dictionary<string, string> Items { get; private set; } = new Dictionary<string, string>
                {
                    { Roles.Owner, "Owner" },
                    { Roles.Manager, "Manager" },
                    { Roles.Borrower, "Borrower" },
                    { Roles.Viewer, "Viewer" }
                };

                public static string ToDisplayNames(string roleKeys)
                {
                    return string.Join(", ", roleKeys
                        .Split(',')
                        .Select(role => Roles.Items.FirstOrDefault(x => x.Key == role).Value));
                }
            }
        }

        public static class Cache
        {
            public static string Library(string libraryKey) => $"libraries/{libraryKey}";
            public static string User(string userKey) => $"users/{userKey}";
        }

        public static class Meta
        {
            public static readonly string RootType = "$root";
            public static readonly string IsDeleted = "$deleted";
            public static readonly string IsSystem = "$system";
            public static readonly string ListView = "$list";
            public static readonly string Required = "$required";
            public static readonly string ItemCount = "$count";
            public static readonly string Warning = "$warning";

            public static readonly string Created = "$created";
            public static readonly string Creator = "$creator";
            public static readonly string CreatorIP = "$creator_ip";
            public static readonly string CreatorEmail = "$creator_email";

            public static readonly string Editor = "$editor";
            public static readonly string EditorIP = "$editor_ip";
            public static readonly string EditorEmail = "$editor_email";

            public static readonly string Id = "$id";
            public static readonly string Revision = "$rev";
            public static readonly string Updated = "$updated";

            public static readonly string FileName = "$file_name";
            public static readonly string FileVersion = "$file_version";
            public static readonly string FileType = "$file_type";
            public static readonly string FileMD5 = "$file_md5";
            public static readonly string FileURL = "$file_url";
            public static readonly string FileSize = "$file_size";
            public static readonly string FileContents = "$file_contents";

            public static readonly string Description = "description";

            public static Dictionary<string, string> Items { get; private set; } = new Dictionary<string, string>
            {
                { LibraryKeys.Meta.RootType, "Root Type" },
                { Meta.IsSystem, "Is System" },
                { Meta.IsDeleted, "Is Deleted" },

                { LibraryKeys.Meta.Created, "Created Date" },
                { LibraryKeys.Meta.Creator, "Creator" },
                { LibraryKeys.Meta.CreatorIP, "Creator IP" },
                { LibraryKeys.Meta.Editor, "Editor" },
                { LibraryKeys.Meta.EditorIP, "Editor IP" },
                { LibraryKeys.Meta.Id, "ID" },
                { LibraryKeys.Meta.Revision, "Revision" },
                { LibraryKeys.Meta.Updated, "Updated On" },

                { LibraryKeys.Meta.Description, "Description" }
            };

            public static string? ToDisplayName(string metaKey)
            {
                return LibraryKeys.Meta.Items.FirstOrDefault(x => x.Key == metaKey).Value ?? metaKey;
            }
        }        
    }

    //public static class LibraryItemTypes
    //{
    //    public static readonly string BaseLibrary = "base";
    //    public static readonly string Library = "$library";

    //    public static readonly string Users = "$users";
    //    public static readonly string User = "$user";
    //    public static readonly string Role = "$role";
    //    public static readonly string Roles = "$roles";

    //    public static readonly string Session = "$session";
    //    public static readonly string File = "$file";
    //}
}
