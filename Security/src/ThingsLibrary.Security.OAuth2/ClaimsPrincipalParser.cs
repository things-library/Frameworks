// ================================================================================
// <copyright file="ClaimsPrincipalParser.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Security.OAuth2
{
    public static class ClaimsPrincipalParser
    {
        /// <summary>
        /// Parse encoded principal
        /// </summary>
        /// <param name="headerPrincipal">base64 encoded dictionary of key value pair claims</param>
        /// <returns></returns>
        public static ClaimsPrincipal Parse(string headerPrincipal)
        {
            var decoded = Convert.FromBase64String(headerPrincipal);
            var json = Encoding.UTF8.GetString(decoded);

            var principal = JsonSerializer.Deserialize<ClientPrincipal>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var identity = new ClaimsIdentity(principal!.IdentityProvider);
            identity.AddClaims(principal.Claims.Select(c => new Claim(c.Type, c.Value)));

            return new ClaimsPrincipal(identity);
        }
    }

    public class ClientPrincipalClaim
    {
        [JsonPropertyName("typ")]
        public string Type { get; init; } = string.Empty;

        [JsonPropertyName("val")]
        public string Value { get; init; } = string.Empty;
    }

    public class ClientPrincipal
    {
        [JsonPropertyName("auth_typ")]
        public string IdentityProvider { get; init; } = string.Empty;

        [JsonPropertyName("name_typ")]
        public string NameClaimType { get; init; } = string.Empty;

        [JsonPropertyName("role_typ")]
        public string RoleClaimType { get; init; } = string.Empty;

        [JsonPropertyName("claims")]
        public IEnumerable<ClientPrincipalClaim> Claims { get; set; } = Enumerable.Empty<ClientPrincipalClaim>();        
    }
}
