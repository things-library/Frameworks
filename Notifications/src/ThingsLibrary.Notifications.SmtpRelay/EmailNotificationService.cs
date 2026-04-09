// ================================================================================
// <copyright file="EmailNotificationService.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using System.Net.Mail;
using Microsoft.Extensions.Logging;

namespace ThingsLibrary.Notification.SmtpRelay
{
    public class EmailNotificationService : IEmailNotifications
    {
        public EmailRelayServiceOptions Options { get; set; }

        private ILogger Logger { get; init; }

        public EmailNotificationService(EmailRelayServiceOptions options, ILogger<EmailNotificationService> logger)
        {
            this.Options = options;
            this.Logger = logger;
        }


        public async Task<ActionResponse> SendAsync(string toAddress, string subject, string body, bool isHtmlBody, CancellationToken cancellationToken)
        {
            try
            {
                using var message = new MailMessage(this.Options.AddressFrom, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtmlBody
                };

                // add any headers that should be included
                if (this.Options.Headers.Count > 0)
                {                    
                    foreach (var header in this.Options.Headers)
                    {
                        message.Headers.Add(header.Key, header.Value);                        
                    }
                }

                using (var smtpClient = new SmtpClient(this.Options.SmtpServer, this.Options.SmtpPort))// { EnableSsl = this.Options.EnableSsl })
                {
                    // do we have credentials to use?
                    if (!string.IsNullOrEmpty(this.Options.Username) && !string.IsNullOrEmpty(this.Options.Password))
                    {
                        smtpClient.Credentials = new NetworkCredential(this.Options.Username, this.Options.Password);
                    }

                    await smtpClient.SendMailAsync(message, cancellationToken);
                }                 

                return new("Notification Sent.");
            }
            catch (Exception ex)
            {
                return new ActionResponse(ex, ex.Message);
            }

        }

        public bool IsHealthy()
        {
            if (string.IsNullOrEmpty(this.Options.SmtpServer))
            {
                return false;
            }

            //try
            //{
            //    using var ping = new System.Net.NetworkInformation.Ping();
            //    var reply = ping.Send(this.SmtpServer, timeout: 3000); // 3 second timeout

            //    return reply.Status == System.Net.NetworkInformation.IPStatus.Success;
            //}
            //catch
            //{
            //    return false;
            //}

            return true;
        }
    }
}
