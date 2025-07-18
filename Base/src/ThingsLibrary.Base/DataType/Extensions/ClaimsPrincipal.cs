﻿// ================================================================================
// <copyright file="ClaimsPrincipal.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using System.Security.Claims;

namespace ThingsLibrary.DataType.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Gets the first or default claim that matches the key
        /// </summary>
        /// <param name="claimsPrincipal"><see cref="ClaimsPrincipal"/></param>
        /// <param name="key">Claim Key</param>
        /// <returns>Claim string</returns>
        public static string GetClaim(this ClaimsPrincipal claimsPrincipal, string key)
        {
            return claimsPrincipal.Claims.GetClaim(key);
        }

        /// <summary>
        /// Get and deserialize the claim value
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="claimsPrincipal"><see cref="ClaimsPrincipal"/></param>
        /// <param name="key">Claim Key</param>
        /// <param name="defaultValue">Default value if key is not found.</param>
        /// <returns>Claim as requested data type, or DefaultValue if key not found."/></returns>
        public static T GetClaim<T>(this ClaimsPrincipal claimsPrincipal, string key, T defaultValue)
        {
            return claimsPrincipal.Claims.GetClaim<T>(key, defaultValue);
        }

        /// <summary>
        /// Get and deserialize the claim value
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="claims"><see cref="IEnumerable{Claim}"/></param>
        /// <param name="key">Claim Key</param>
        /// <param name="defaultValue">Default value if key is not found.</param>
        /// <returns>Claim as requested data type, or DefaultValue if key not found."/></returns>
        public static T GetClaim<T>(this IEnumerable<Claim> claims, string key, T defaultValue)
        {
            var claim = GetClaim(claims, key);
            if (claim == null) { return defaultValue; }

            return (T)Convert.ChangeType(claim, typeof(T));
        }

        /// <summary>
        /// Gets the first or default claim that matches the key
        /// </summary>
        /// <param name="claims"><see cref="IEnumerable{Claim}"/></param>
        /// <param name="key">Claim Key</param>
        /// <returns>Claim string</returns>
        public static string GetClaim(this IEnumerable<Claim> claims, string key)
        {
            if (string.IsNullOrEmpty(key)) { return string.Empty; }

            return claims.FirstOrDefault(c => c.Type == key)?.Value ?? string.Empty;
        }

        /// <summary>
        /// Gets the first or default claim that matches the keys (found in order of keys)
        /// </summary>
        /// <param name="claimsPrincipal"><see cref="ClaimsPrincipal"/></param>
        /// <param name="keys">Claim Keys (in order of importance)</param>
        /// <returns>Claim string</returns>
        public static string GetClaim(this ClaimsPrincipal claimsPrincipal, params string[] keys)
        {
            return claimsPrincipal.Claims.GetClaim(keys);
        }

        /// <summary>
        /// Gets the first or default claim that matches the keys (found in order of keys)
        /// </summary>
        /// <param name="claims"><see cref="IEnumerable{Claim}"/></param>
        /// <param name="keys">Claim Keys (in order of importance)</param>
        /// <returns>Claim string</returns>
        public static string GetClaim(this IEnumerable<Claim> claims, params string[] keys)
        {
            if (keys == null || !keys.Any()) { return string.Empty; }

            Claim? claim;
            foreach (var key in keys)
            {
                claim = claims.FirstOrDefault(c => keys.Contains(c.Type));
                if (claim != default) { return claim.Value; }
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the collection of claims that has the key type
        /// </summary>
        /// <param name="claimsPrincipal"><see cref="ClaimsPrincipal"/></param>
        /// <param name="key">Claim Key</param>
        /// <returns>Claim string</returns>
        public static IEnumerable<string> GetClaims(this ClaimsPrincipal claimsPrincipal, string key)
        {
            return claimsPrincipal.Claims.GetClaims(key);
        }

        /// <summary>
        /// Gets the collection of claims that has the key type
        /// </summary>
        /// <param name="claims"><see cref="IEnumerable{Claim}"/></param>
        /// <param name="key">Claim Key</param>
        /// <returns>Claim string</returns>
        public static IEnumerable<string> GetClaims(this IEnumerable<Claim> claims, string key)
        {
            if (string.IsNullOrEmpty(key)) { return new List<string>(); }

            return claims.Where(c => c.Type == key).Select(c => c.Value);
        }

        #region --- User Properties ---

        /// <summary>
        /// Get User ID from claims
        /// </summary>
        /// <param name="claimsPrincipal">Claims Principal</param>
        /// <returns></returns>
        public static string GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            // try to find the user based on the object id (if exists)
            var userId = claimsPrincipal.Claims.SingleOrDefault(c => c.Type == "sub")?.Value.ToLower();
            if (userId == null) { userId = claimsPrincipal.Claims.SingleOrDefault(c => c.Type == "oid")?.Value.ToLower(); }

            userId = userId?.Trim().ToLower();

            return userId ?? string.Empty;
        }

        /// <summary>
        /// Get Username from claims
        /// </summary>
        /// <param name="claimsPrincipal">Claims Principal</param>
        /// <returns></returns>
        public static string GetUsername(this ClaimsPrincipal claimsPrincipal)
        {
            // find a unique username to use            
            var username = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == "unique_name")?.Value;
            if (username == default) { username = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == "preferred_username")?.Value; }
            if (username == default) { username = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == "upn")?.Value; }
            if (username == default) { username = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == "username")?.Value; }
            if (username == default) { username = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == "email")?.Value; }

            username = username?.Trim().ToLower();

            return username ?? string.Empty;
        }

        /// <summary>
        /// Get email address from claims
        /// </summary>
        /// <param name="claimsPrincipal">Claims Principal</param>
        /// <returns></returns>
        public static string GetEmailAddress(this ClaimsPrincipal claimsPrincipal)
        {
            // find a unique username to use
            var email = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == "email")?.Value;
            if (email == default)
            {
                var username = claimsPrincipal.GetUsername();
                if (IsValidEmail(username)) { email = username; }
            }

            email = email?.Trim().ToLower();

            return email ?? string.Empty;
        }

        /// <summary>
        /// Get first and last name
        /// </summary>
        /// <param name="claimsPrincipal">Claims Principal</param>
        /// <returns></returns>
        public static (string, string) GetNames(this ClaimsPrincipal claimsPrincipal)
        {
            var givenName = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == "given_name")?.Value;
            var familyName = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == "family_name")?.Value;

            if (string.IsNullOrEmpty(givenName) && string.IsNullOrEmpty(familyName))
            {
                var fullName = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == "name")?.Value;
                if (fullName != default)
                {
                    int spacePos = fullName.IndexOf(' ');
                    if (spacePos > 0)
                    {
                        givenName = fullName.Substring(0, spacePos);
                        familyName = fullName.Substring(spacePos + 1);
                    }
                }
            }

            // we don't want to deal with nulls
            givenName = givenName?.Trim();
            familyName = familyName?.Trim();

            return (
                givenName ?? string.Empty,
                familyName ?? string.Empty
            );
        }

        /// <summary>
        /// Get a display name based on given_name and family_name
        /// </summary>
        /// <param name="claimsPrincipal">Claims Principal</param>
        /// <returns></returns>
        public static string GetDisplayName(this ClaimsPrincipal claimsPrincipal)
        {
            var (givenName, familyName) = claimsPrincipal.GetNames();

            return $"{givenName} {familyName}".Trim();
        }


        /// <summary>
        /// Get Phone Number from claims
        /// </summary>
        /// <param name="claimsPrincipal">Claims Principal</param>
        /// <returns></returns>
        public static string GetPhoneNumber(this ClaimsPrincipal claimsPrincipal)
        {
            var phoneNumber = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == "phoneNumber")?.Value;
            if (phoneNumber == default) { phoneNumber = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == "phone")?.Value; }

            // we don't want to deal with nulls
            phoneNumber = phoneNumber?.Trim();

            return phoneNumber ?? string.Empty;
        }

        /// <summary>
        /// Check if the email address is valid
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private static bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith(".")) { return false; }

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
