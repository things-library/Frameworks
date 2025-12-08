// ================================================================================
// <copyright file="EmailService.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Notification.Azure
{
    public class EmailService : IEmailNotifications
    {
        public EmailServiceOptions Options { get; set; }

        public EmailService(EmailServiceOptions options)
        {
            this.Options = options;
        }


        public async Task<ActionResponse> SendAsync(string toAddress, string subject, string body, bool isHtmlBody, CancellationToken cancellationToken)
        {
            try
            {
                var client = new EmailClient(this.Options.ConnectionString);

                // COMPOSE
                var emailContent = new EmailContent(subject);
                if (isHtmlBody)
                {
                    emailContent.Html = body;                    
                }
                else
                {
                    emailContent.PlainText = body;
                }

                var emailMessage = new EmailMessage(this.Options.AddressFrom, toAddress, emailContent);
                    
                // SEND
                var result = await client.SendAsync(WaitUntil.Started, emailMessage, cancellationToken);
                //if (!result.Value.Status) { return new ActionResponse("Unable to send notification."); }

                return new("Notification Sent.");
            }
            catch (Exception ex)
            {
                return new ActionResponse(ex, ex.Message);
            }

        }
    }
}
