// ================================================================================
// <copyright file="EmailServiceOptions.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using ThingsLibrary.Schema.Library;
using ThingsLibrary.Schema.Library.Extensions;

namespace ThingsLibrary.Notification.SmtpRelay
{
    public class EmailRelayServiceOptions
    {
        public string SmtpServer { get; init; } = string.Empty;
        public int SmtpPort { get; init; } = 465; // Default port for SMTP over SSL
        public bool EnableSsl { get; init; } = true;

        public string Username { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;

        public string AddressFrom { get; set; }

        public Dictionary<string, string> Headers { get; set; } = new();


        public EmailRelayServiceOptions(ItemDto options)
        {
            // {
            //   "tags": { 
            //     "smtp_server": "test@relay.com",            
            //     "username": "TestUsername",
            //     "password": "Test12345",
            //
            //     "address_from": "",
            //     "retain": "true"
            //   }
            //   "items": {
            //     "headers": {
            //       "tags": {
            //         "header1": "value1",
            //         "header2": "value2",
            //       }
            //     }
            //   }
            // }

            ArgumentNullException.ThrowIfNull(options);
            
            this.SmtpServer = options["smtp_server"] ?? string.Empty;
            this.SmtpPort = int.Parse(options["smtp_port"] ?? "25");
            this.EnableSsl = bool.Parse(options["enable_ssl"] ?? "false");

            this.Username = options["username"] ?? string.Empty;
            this.Password = options["password"] ?? string.Empty;

            this.AddressFrom = options["address_from"] ?? string.Empty;

            ArgumentException.ThrowIfNullOrEmpty(this.SmtpServer);            
            ArgumentException.ThrowIfNullOrEmpty(this.AddressFrom);

            if (!string.IsNullOrEmpty(this.Username) && string.IsNullOrEmpty(this.Password))
            {
                throw new ArgumentException("'Password' must be provided when username is specified.");
            }

            // get any header items that should be added to the email             
            if (options.TryGetItem("headers", out var headersItem) && headersItem.Tags != null)
            {
                foreach (var header in headersItem.Tags)
                {
                    this.Headers[header.Key] = header.Value;
                }
            }
        }
    }
}
