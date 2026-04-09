// ================================================================================
// <copyright file="EmailNotificationService.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using System.Net.Mail;
using Azure.Identity;
using Microsoft.Extensions.Logging;

namespace ThingsLibrary.Notification.Graph
{
    public class EmailNotificationService : IEmailNotifications
    {
        public EmailServiceOptions Options { get; set; }
        public GraphServiceClient GraphClient { get; init; }

        private ILogger Logger { get; init; }

        public EmailNotificationService(EmailServiceOptions options, ILogger<EmailNotificationService> logger)
        {
            this.Options = options;
            this.Logger = logger;

            // Use client credentials flow instead of UsernamePasswordCredential (deprecated and MFA-incompatible)
            var credential = new ClientSecretCredential(
                this.Options.TenantId,
                this.Options.ClientId,
                this.Options.ClientSecret,
                new TokenCredentialOptions { AuthorityHost = AzureAuthorityHosts.AzurePublicCloud }
            );

            this.GraphClient = new GraphServiceClient(credential, this.Options.Scopes);
        }


        public async Task<ActionResponse> SendAsync(string toAddress, string subject, string body, bool isHtmlBody, CancellationToken cancellationToken)
        {
            try
            {
                var message = new Message
                {
                    Subject = subject,
                    Body = new ItemBody
                    {
                        ContentType = (isHtmlBody ? BodyType.Html : BodyType.Text),
                        Content = body,
                    },

                    ToRecipients = new List<Recipient> { new Recipient { EmailAddress = new EmailAddress { Address = toAddress } } }                    
                };

                // add any headers that should be included
                if (this.Options.Headers.Count > 0)
                {
                    message.InternetMessageHeaders = new List<InternetMessageHeader>();
                    foreach (var header in this.Options.Headers)
                    {
                        message.InternetMessageHeaders.Add(new InternetMessageHeader
                        {
                            Name = header.Key,
                            Value = header.Value
                        });
                    }
                }

                await this.GraphClient.Users[this.Options.AddressFrom].SendMail.PostAsync(new () { Message = message, SaveToSentItems = this.Options.SaveToSentItems }, null, cancellationToken);

                return new("Notification Sent.");
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "Failed to send email notification via Graph.");

                return new ActionResponse(ex, ex.Message);
            }

        }

        public bool IsHealthy()
        {
            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                                
                var users = this.GraphClient.Users.GetAsync((request) =>
                {
                    request.QueryParameters.Top = 1;
                }, cancellationToken: cts.Token).GetAwaiter().GetResult();

                return (users != null);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "Graph Email health check failed.");

                return false;
            }
        }
    }
}
