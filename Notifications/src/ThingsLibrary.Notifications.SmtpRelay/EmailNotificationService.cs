// ================================================================================
// <copyright file="EmailNotificationService.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using System.Data.Common;
using System.Net.Mail;

namespace ThingsLibrary.Notification.SmtpRelay
{
    public class EmailNotificationService : IEmailNotifications
    {
        public EmailServiceOptions Options { get; set; }

        public string SmtpServer { get; init; }
        public int SmtpPort { get; init; }
        public bool EnableSsl { get; init; }


        public string? Username { get; init; }
        public string? Password { get; init; }


        public EmailNotificationService(EmailServiceOptions options)
        {
            this.Options = options;

            DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
            builder.ConnectionString = this.Options.ConnectionString;

            if (builder.TryGetValue("endpoint", out var endpoint))
            {
                this.SmtpServer = $"{endpoint}";
            }
            else
            {
                throw new ArgumentException("'endpoint' is required for Email Relay Service.");
            }

            if (builder.TryGetValue("port", out var port))
            {
                this.SmtpPort = int.Parse($"{port}");
            }
            else
            {
                this.SmtpPort = 465; //default port
            }

            if (builder.TryGetValue("ssl", out var ssl))
            {
                this.EnableSsl = bool.Parse($"{ssl}");
            }

            if (builder.TryGetValue("user", out var username))
            {
                this.Username = $"{username}";
            }

            if (builder.TryGetValue("password", out var password))
            {
                this.Password = $"{password}";
            }
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

                using var smtpClient = new SmtpClient(this.SmtpServer, this.SmtpPort)
                {
                    EnableSsl = this.EnableSsl                    
                };

                // do we have credentials to use?
                if(!string.IsNullOrEmpty(this.Username) && !string.IsNullOrEmpty(this.Password))
                {
                    smtpClient.Credentials = new NetworkCredential(this.Username, this.Password);
                }               

                await smtpClient.SendMailAsync(message, cancellationToken);

                return new("Notification Sent.");
            }
            catch (Exception ex)
            {
                return new ActionResponse(ex, ex.Message);
            }

        }

        public bool IsHealthy()
        {
            if (string.IsNullOrEmpty(this.SmtpServer))
            {
                return false;
            }

            try
            {
                using var ping = new System.Net.NetworkInformation.Ping();
                var reply = ping.Send(this.SmtpServer, timeout: 3000); // 3 second timeout
                
                return reply.Status == System.Net.NetworkInformation.IPStatus.Success;
            }
            catch
            {
                return false;
            }
        }
    }
}
