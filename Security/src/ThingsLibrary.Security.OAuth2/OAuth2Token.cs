// ================================================================================
// <copyright file="OAuth2Token.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using ThingsLibrary.DataType.Extensions;

namespace ThingsLibrary.Security.OAuth2
{
    /// <summary>
    /// General JWT Token
    /// </summary>
    public class OAuth2Token
    {
        [JsonIgnore]
        public JwtSecurityToken JwtAccessToken { get; init; } = new JwtSecurityToken();

        [JsonIgnore]
        public JwtSecurityToken? JwtIdToken { get; init; } = null;
        
        #region --- Token Root Properties ---

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = "Bearer";
        
        /// <summary>
        /// Access Token provides authorization based on requested scopes
        /// </summary>
        [JsonPropertyName("access_token")]
        public string AccessToken => this.JwtAccessToken.RawData;

        /// <summary>
        /// ID Token provides authentication details that the user actually logged in.
        /// </summary>
        [JsonPropertyName("id_token")]
        public string? IdToken => this.JwtIdToken?.RawData;

        /// <summary>
        /// Refresh Token used to refresh the access token
        /// </summary>
        [JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; init; }

        // ================================================================================================================
        // EXTENDED PROPERTIES
        // ================================================================================================================
        /// <summary>
        /// Object Id (Claim: oid)
        /// </summary>
        [JsonPropertyName("oid")]
        public string ObjectId => this.JwtAccessToken.Claims.FirstOrDefault(c => c.Type == "oid")?.Value.ToLower() ?? string.Empty;

        /// <summary>
        /// User Display Name (Claim: name)
        /// </summary>
        [JsonPropertyName("name"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string DisplayName => this.JwtAccessToken?.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? string.Empty;


        /// <summary>
        /// Given (First) Name (Claim: given_name)
        /// </summary>
        [JsonPropertyName("given_name"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string GivenName => this.JwtAccessToken?.Claims.FirstOrDefault(c => c.Type == "given_name")?.Value ?? string.Empty;

        /// <summary>
        /// First Name (Claim: family_name)
        /// </summary>
        [JsonPropertyName("family_name"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string FamilyName => this.JwtAccessToken?.Claims.FirstOrDefault(c => c.Type == "family_name")?.Value ?? string.Empty;

        /// <summary>
        /// User Principal Name (Claim: upn)
        /// </summary>
        [JsonPropertyName("upn"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Username => this.JwtAccessToken?.Claims.FirstOrDefault(c => c.Type == "upn")?.Value.ToLower() ?? string.Empty;

        /// <summary>
        /// User Email (Claim: email)
        /// </summary>
        [JsonPropertyName("email")]
        public string UserEmail => this.JwtAccessToken.Claims.FirstOrDefault(c => c.Type == "user_email")?.Value.ToLower() ?? string.Empty;

        [JsonPropertyName("expries_on")]
        public DateTime? ExpiresOn => this.JwtAccessToken?.ValidTo;

        [JsonPropertyName("created_on")]
        public DateTime? CreatedOn => this.JwtAccessToken?.IssuedAt;

        #endregion

        /// <summary>
        /// App Roles (Claim: roles)
        /// </summary>
        [JsonPropertyName("roles")]
        public List<string> Roles
        {
            get
            {
                if (_roles == null)
                {
                    // figure out where the roles should come from 
                    _roles = this.JwtAccessToken.Claims.Where(c => c.Type == "roles").Select(x => x.Value).ToList();
                    if (_roles.Count == 0 && this.JwtIdToken != null)
                    {
                        _roles = this.JwtIdToken.Claims.Where(c => c.Type == "roles").Select(x => x.Value).ToList();
                    }
                }

                return _roles;
            }
        }
        private List<string>? _roles = null;



        public OAuth2Token() { }

        #region --- Static Methods ---

        /// <summary>
        /// Parse out the jwt tokens into a OAuth2Token object
        /// </summary>
        /// <param name="accessToken">Access Token (or null)</param>
        /// <param name="idTokenJwt">JWT Id Token (or null)</param>
        /// <param name="refreshToken">Refresh Token</param>
        /// <returns><see cref="OAuth2Token"/></returns>
        public static OAuth2Token Parse(string accessToken, string? idTokenJwt, string? refreshToken)
        {
            if (string.IsNullOrEmpty(accessToken)) { throw new ArgumentNullException(nameof(accessToken)); }

            var handler = new JwtSecurityTokenHandler();

            var jwtaccessToken = handler.ReadJwtToken(accessToken);
            
            
            JwtSecurityToken? jwtIdToken = null;
            if (!string.IsNullOrEmpty(idTokenJwt))
            {
                jwtIdToken = handler.ReadJwtToken(idTokenJwt);
            }

            return new OAuth2Token()
            {
                JwtAccessToken = jwtaccessToken,
                JwtIdToken = jwtIdToken,  

                RefreshToken = refreshToken
            };
        }

        /// <summary>
        /// Parses a OAuth2 json response
        /// </summary>
        /// <param name="json">OAuth2 Json Response</param>
        /// <returns><see cref="OAuth2Token"/></returns>
        /// <remarks>Assumes properties: "access_token", "id_Token", and optionally "refresh_token"</remarks>
        public static OAuth2Token Parse(string json)
        {
            return Parse(JsonDocument.Parse(json));
        }

        /// <summary>
        /// Parses a OAuth2 json response { "id": "", "access_token": "", "refresh_token": "" } 
        /// </summary>
        /// <param name="json">OAuth2 Json Response</param>
        /// <returns><see cref="OAuth2Token"/></returns>
        /// <remarks>Assumes properties: "access_token", "id_Token", and optionally "refresh_token"</remarks>
        public static OAuth2Token Parse(JsonDocument json)
        {
            var accessToken = json.RootElement.GetProperty<string>("access_token", string.Empty);
            if (string.IsNullOrEmpty(accessToken)) { throw new ArgumentException("'access_token' not found in json data."); }

            var jwtIdToken = json.RootElement.GetProperty<string>("id_token", string.Empty);
            var refresh_token = json.RootElement.GetProperty<string>("refresh_token", string.Empty);

            return OAuth2Token.Parse(accessToken, jwtIdToken, refresh_token);
        }

        /// <summary>
        /// Parse out common cliams used for username
        /// </summary>
        public static string GetUsername(JwtSecurityToken token)
        {
            if (token == null) { throw new ArgumentNullException(nameof(token)); }

            var name = token.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            if (!string.IsNullOrEmpty(name)) { return name; }

            // Try UPN which is principal name often username
            var username = token.Claims.FirstOrDefault(c => c.Type == "upn")?.Value;
            if (!string.IsNullOrEmpty(username))
            {
                return username;
            }

            // try again? 
            var firstName = token.Claims.FirstOrDefault(c => c.Type == "given_name")?.Value;
            var lastName = token.Claims.FirstOrDefault(c => c.Type == "family_name")?.Value;

            if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
            {
                return $"{firstName} {lastName}";
            }
            else
            {
                return string.Empty;
            }
        }

        #endregion
    }

}
