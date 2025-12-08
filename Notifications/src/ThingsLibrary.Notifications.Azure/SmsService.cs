// ================================================================================
// <copyright file="SmsService.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Notification.Azure
{
    public class SmsService : ISmsNotifications
    {
        public SmsServiceOptions Options { get; set; }

        public SmsService(SmsServiceOptions options)
        {
            this.Options = options;
        }
        
        public async Task<ActionResponse> SendAsync(string toAddress, string message, CancellationToken cancellationToken)
        {
            try
            {
                var client = new SmsClient(this.Options.ConnectionString);

                var result = await client.SendAsync(this.Options.AddressFrom, toAddress, message, null, cancellationToken);
                if (!result.Value.Successful) { return new ActionResponse("Unable to send notification."); }

                return new("Notification Sent.");
            }
            catch (Exception ex)
            {
                return new ActionResponse(ex, ex.Message);
            }

        }
    }
}
