// ================================================================================
// <copyright file="EmailServiceOptions.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Notification.Azure
{
    public class EmailServiceOptions
    {
        public string ConnectionString { get; set; }

        public string AddressFrom { get; set; }

        public EmailServiceOptions(string connectionString, string addressFrom)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(connectionString);
            ArgumentNullException.ThrowIfNullOrEmpty(addressFrom);

            this.ConnectionString = connectionString;
            this.AddressFrom = addressFrom;
        }
    }
}
