// ================================================================================
// <copyright file="ClaimsPrincipal.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using System.Security.Claims;
using ThingsLibrary.Schema.Canvas.Base;

namespace ConnectedServices.Portal.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static RootItemDto ToItemDto(this ClaimsPrincipal principal)
        {
            var userId = principal.GetUserId();
            var userEmail = principal.GetEmailAddress() ?? throw new ArgumentException("Email address not found in claims.");

            if (string.IsNullOrEmpty(userId))
            {
                userId = SchemaBase.GenerateKey(userEmail);
            }


            // compose response object
            var item = new RootItemDto
            {
                Type = "user",

                Key = userId,
                Name = principal.GetDisplayName() ?? "Unknown",
                Tags = new Dictionary<string, string>
                {
                    { "id", userId },
                    { "email", userEmail },
                }                
            };

            // for good measure to have all items under 'tags'
            item.SetTagIfNotNull("name", item.Name);
            
            var (givenName, familyName) = principal.GetNames();
            item.SetTagIfNotNull("given_name", givenName);
            item.SetTagIfNotNull("family_name", familyName);

            var roles = principal.GetClaims("roles");
            item.SetTagIfNotNull("roles", string.Join(',', roles));

            return item;
        }
    }
}
