// ================================================================================
// <copyright file="JwtSecurityToken.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Security.OAuth2.Extensions
{
    public static class JwtSecurityTokenExtensions
    {
        /// <summary>
        /// Get the listing of scopes
        /// </summary>
        /// <param name="token"><see cref="JwtSecurityToken"/></param>
        /// <returns>List of string</returns>
        public static IEnumerable<string> Scopes(this JwtSecurityToken token)
        {
            return token.Claims.GetClaims("scp");
        }

        /// <summary>
        /// Gets the first or default claim that matches the key
        /// </summary>
        /// <param name="token"><see cref="JwtSecurityToken"/></param>
        /// <param name="key">Claim Key</param>
        /// <returns>Claim string</returns>
        public static string GetClaim(this JwtSecurityToken token, string key)
        {            
            return token.Claims.GetClaim(key);
        }
                
        /// <summary>
        /// Get and deserialize the claim value
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="claimsPrincipal"><see cref="ClaimsPrincipal"/></param>
        /// <param name="key">Claim Key</param>
        /// <param name="defaultValue">Default value if key is not found.</param>
        /// <returns>Claim as requested data type, or DefaultValue if key not found."/></returns>
        public static T GetClaim<T>(this JwtSecurityToken token, string key, T defaultValue)
        {
            return token.Claims.GetClaim<T>(key, defaultValue);
        }

        /// <summary>
        /// Validates the ValidFrom and ValidTo component of the token
        /// </summary>
        /// <param name="token">JWT Token</param>
        /// <param name="clockSkew">Clock Skew to allow for variations in clocks between systems</param>
        /// <returns></returns>
        public static bool IsLifetimeValid(this JwtSecurityToken token, TimeSpan? clockSkew = null)
        {
            var now = DateTime.UtcNow;

            // set to zero skew
            if (clockSkew == null)
            {
                clockSkew = new TimeSpan();
            }

            // we are before the from?
            if (token.ValidFrom != default && now < token.ValidFrom.Subtract(clockSkew.Value)) { return false; }

            // we are after the TO?
            if (token.ValidTo != default && now > token.ValidTo.Add(clockSkew.Value)) { return false; }

            // no reason to think it is invalid
            return true;
        }        
    }
}
