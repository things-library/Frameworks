// ================================================================================
// <copyright file="EmailServiceOptions.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using ThingsLibrary.Schema.Library;
using ThingsLibrary.Schema.Library.Extensions;

namespace ThingsLibrary.Notification.Graph
{
    public class EmailServiceOptions
    {
        public string TenantId { get; init; } = string.Empty;        
        public string ClientId { get; init; } = string.Empty;
        public string ClientSecret { get; init; } = string.Empty;
        public bool SaveToSentItems { get; init; } = false;

        public string[] Scopes { get; init; } = { "https://graph.microsoft.com/.default" }; //, ".default", "Mail.Send" };

        public Dictionary<string, string> Headers { get; set; } = new();


        public string AddressFrom { get; set; }
        
        public EmailServiceOptions(ItemDto options)
        {
            // {
            //   "tags": { 
            //     "tenant_id": "55555555-5555-5555-5555-555555555555",
            //     "client_id": "55555555-5555-5555-5555-555555555555",
            //     "address_from": "test@compant.com",
            //     "address_from_secret": "",
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

            this.TenantId = options["tenant_id"] ?? string.Empty;
            this.ClientId = options["client_id"] ?? string.Empty;
            this.ClientSecret = options["client_secret"] ?? string.Empty;
            this.SaveToSentItems = bool.Parse(options["retain"] ?? "false");    //retain = keep a copy of the sent email in the Sent Items folder of the sender's mailbox

            this.AddressFrom = options["address_from"] ?? string.Empty;
            

            ArgumentException.ThrowIfNullOrEmpty(this.TenantId);
            ArgumentException.ThrowIfNullOrEmpty(this.ClientId);
            ArgumentException.ThrowIfNullOrEmpty(this.ClientSecret);

            ArgumentException.ThrowIfNullOrEmpty(this.AddressFrom);

            if (this.Scopes.Length == 0) { throw new ArgumentException("At least one scope must be provided."); }

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
