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
        public static T GetClaim<T>(this JwtSecurityToken token, string key, T defaultValue)
        {
            return token.Claims.GetClaim<T>(key, defaultValue);
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
            foreach(var key in keys)
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


        public static IEnumerable<string> GetRoles(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.GetRoles();
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
