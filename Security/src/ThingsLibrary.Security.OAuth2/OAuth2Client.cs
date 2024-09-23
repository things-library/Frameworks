// ================================================================================
// <copyright file="OAuth2Client.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using ThingsLibrary.DataType;
using ThingsLibrary.DataType.Json;
using ThingsLibrary.Security.OAuth2.Extensions;
using System.Text.Json.Serialization.Metadata;

namespace ThingsLibrary.Security.OAuth2
{
    /// <summary>
    /// OAuth2 wrapper class
    /// </summary>
    public class OAuth2Client
    {
        /// <summary>        
        /// Open ID Provider Uri 
        /// </summary>
        /// <example>https://login.microsoftonline.com/{{tenant_id}}/v2.0</example>
        [JsonPropertyName("authority")]
        public Uri? Authority { get; init; }

        /// <summary>
        /// Open ID Config Uri (Typically: {authProviderUri}/.well-known/openid-configuration)
        /// </summary>
        [JsonPropertyName("oidc_url")]
        public Uri? OpenIdConfigUri => (this.Authority != null ? new Uri($"{this.Authority.AbsoluteUri}/.well-known/openid-configuration") : null);

        #region --- OpenID Settings ---

        /// <summary>
        /// Client ID
        /// </summary>
        [JsonPropertyName("clientId")]
        public string? ClientId { get; init; }

        /// <summary>
        /// Client Secret
        /// </summary>
        [JsonIgnore]
        public string? ClientSecret { get; init; }

        /// <summary>
        /// Callback Url
        /// </summary>
        [JsonPropertyName("callbackUri")]
        public string? CallbackUrl { get; init; }

        #endregion

        #region --- JWT Token ---

        /// <summary>
        /// Issuer (or delimited list)
        /// </summary>
        [JsonPropertyName("issuer")]
        public string? Issuer { get; set; }

        /// <summary>
        /// Audience (or delimited list)
        /// </summary>
        [JsonPropertyName("audience")]
        public string? Audience { get; init; }

        [JsonIgnore]
        public string NameClaimType { get; init; } = "name";

        [JsonIgnore]
        public string RoleClaimType { get; init; } = "roles";

        #endregion

        /// <summary>
        /// Signing Keys
        /// </summary>
        [JsonIgnore]
        public IEnumerable<SecurityKey> SigningKeys { get; set; } = new List<SecurityKey>();

        [JsonIgnore]
        public bool ShowValidationErrors { get; init; }

        [JsonIgnore]
        public bool DisableValidation { get; init; }

        #region --- Scopes ---

        /// <summary>
        /// Scope (space delimited scopes listing)
        /// </summary>
        [JsonPropertyName("scope")]
        public string Scope
        {
            get => string.Join(' ', this.Scopes);
            init => this.Scopes = value.Split(' ').ToList();
        }

        /// <summary>
        /// List of scopes
        /// </summary>
        [JsonPropertyName("scopes")]
        public List<string> Scopes { get; init; } = new List<string>();
       
        /// <summary>
        /// Add Scope to scope list
        /// </summary>
        /// <param name="scope">Scope to add</param>
        /// <remarks>NOTE: This does not check if scope is in the 'ScopesSuppored' listing</remarks>
        public void AddScope(string scope)
        {
            if(this.Scopes.Contains(scope)) { return; }

            //if (this.ScopesSupported.Any() && !this.ScopesSupported.Contains(scope))
            //{
            //    throw new ArgumentException($"Scope '{scope}' is not supported.");
            //}

            this.Scopes.Add(scope);            
        }

        #endregion

        #region --- OpenId Service Definitions ---

        [JsonIgnore]
        public OpenIdConnectConfiguration OpenId { get; private set; } = new OpenIdConnectConfiguration();
                
        /// <summary>
        /// Authorization Endpoint
        /// </summary>
        [JsonPropertyName("auth_uri")]
        public string AuthUrl => this.OpenId.AuthorizationEndpoint;

        /// <summary>
        /// Token Endpoint
        /// </summary>
        [JsonPropertyName("token_uri")]
        public string TokenUrl => this.OpenId.TokenEndpoint;

        /// <summary>
        /// User Info Endpoint
        /// </summary>
        [JsonPropertyName("userinfo_url")]
        public string UserInfoUrl  => this.OpenId.UserInfoEndpoint;

        //[JsonPropertyName("device_authorization_endpoint")]
        //public string DeviceCodeUrl => this.OpenId.DeviceCodeEndPoint;      //device_authorization_endpoint

        /// <summary>
        /// JWKs Endpoint
        /// </summary>
        [JsonPropertyName("jwks_uri")]
        public string JwksUrl => this.OpenId.JwksUri;

        /// <summary>
        /// End Session Endpoint
        /// </summary>
        [JsonPropertyName("end_session_uri")]
        public string EndSessionUrl => this.OpenId.EndSessionEndpoint;
    
        /// <summary>
        /// Grant Types Supported
        /// </summary>
        [JsonPropertyName("grant_types_supported")]
        public IEnumerable<string> GrantTypesSupported => this.OpenId.GrantTypesSupported;

        /// <summary>
        /// Response Types Supported
        /// </summary>
        [JsonPropertyName("response_types_supported")]
        public IEnumerable<string> ResponseTypesSupported => this.OpenId.ResponseTypesSupported;

        /// <summary>
        /// Scopes Supported
        /// </summary>
        [JsonPropertyName("scopes_supported")]
        public IEnumerable<string> ScopesSupported => this.OpenId.ScopesSupported;

        /// <summary>
        /// Claims Supported
        /// </summary>
        [JsonPropertyName("claims_supported")]
        public IEnumerable<string> ClaimsSupported => this.OpenId.ClaimsSupported;

        /// <summary>
        /// Encryption Keys
        /// </summary>
        [JsonIgnore]
        public JsonWebKeySet EncryptionKeys => this.OpenId.JsonWebKeySet;

        /// <summary>
        /// Last Definitions Refresh
        /// </summary>
        [JsonIgnore]
        public DateTime? UpdatedOn { get; set; } = null;

        #endregion

        public OAuth2Client() 
        { 
            //nothing
        }

        /// <summary>
        /// Creates a new OAuth2 object
        /// </summary>
        /// <param name="authorityUri">OAuth2 authority url</param>        
        /// <exception cref="ArgumentNullException"></exception>
        public OAuth2Client(Uri authorityUri)
        {
            // just in case its a OIDC config url.. convert it to just the authority url
            if (authorityUri.OriginalString.EndsWith("/.well-known/openid-configuration", StringComparison.OrdinalIgnoreCase)) 
            {
                this.Authority = new Uri(authorityUri.OriginalString.Replace("/.well-known/openid-configuration", ""));                
            }
            else
            {
                this.Authority = authorityUri;
            }
        }

        /// <summary>
        /// Check that the OpenId Definitions are current
        /// </summary>        
        /// <returns></returns>
        public async Task CheckOpenIdDefinitionsAsync()
        {
            if (this.OpenIdConfigUri == default) { return; }
            if (this.UpdatedOn != null) { return; }
                        
            await this.FetchOpenIdDefinitionsAsync();                     
        }
                
        /// <summary>
        /// Get updated service definitions
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public async Task FetchOpenIdDefinitionsAsync()
        {
            if (this.OpenIdConfigUri == default) { throw new ArgumentException("No Open ID Connect Config Uri defined."); }

            var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(this.OpenIdConfigUri.ToString(), new OpenIdConnectConfigurationRetriever());
            OpenIdConnectConfiguration config;// = await configurationManager.GetConfigurationAsync(CancellationToken.None);

            try
            {
                config = await configurationManager.GetConfigurationAsync(CancellationToken.None);
                //this.SaveLoadOpenIdConnectConfiguration(config);
            }
            catch
            {
                //Log.Warning("UNABLE TO FETCH OIDC CONFIGURATION!!!");
                config = this.LoadOpenIdConnectConfiguration();
            }

            // go get the data
            this.UpdateServiceDefinitions(config);
        }

        #region --- Backup OIDC Settings ---

        private OpenIdConnectConfiguration LoadOpenIdConnectConfiguration()
        {
            //Log.Warning("Loading backup copy of OIDC...");

            string filePath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "static", "openid-configuration.json");
            if (!Path.Exists(filePath)) { throw new ArgumentException($"Unable to find OIDC backup copy at '{filePath}'"); }
                        
            var fileData = System.IO.File.ReadAllText(filePath);
            if (string.IsNullOrEmpty(fileData)) { throw new ArgumentException("Unable to load backup copy of OIDC Settings"); }
            
            return JsonSerializer.Deserialize<OpenIdConnectConfiguration>(fileData) ?? throw new ArgumentException($"Unable to deserialize OIDC backup copy."); 
        }

        private void SaveLoadOpenIdConnectConfiguration(OpenIdConnectConfiguration config)
        {
            try
            {
                //Log.Debug("Saving backup copy of OIDC...");
                string dirPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "static");
                Directory.CreateDirectory(dirPath);

                string filePath = Path.Combine(dirPath, "openid-configuration.json");
                
                // if the file isn't that old then we don't need to save it every time... this might be a shared file mount
                if(File.Exists(filePath))
                {
                    var fileAge = DateTime.Now - System.IO.File.GetLastWriteTime(filePath);
                    if(fileAge < TimeSpan.FromHours(6)) { return; } // less then 6 hours old?  close enough as this is just a backup in case the identity server is offline
                }

                var fileData = JsonSerializer.Serialize(config, new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                    WriteIndented = true
                });
                System.IO.File.WriteAllText(filePath, fileData);
            }
            catch 
            { 
                //nothing.. if we can't save we can't save.. not the end of the world
            }            
        }

        #endregion

        /// <summary>
        /// Update service definitions based on provide configuration
        /// </summary>
        /// <param name="openIdConfig"></param>
        public void UpdateServiceDefinitions(OpenIdConnectConfiguration openIdConfig)
        {
            ArgumentNullException.ThrowIfNull(openIdConfig);

            // go get the data
            this.OpenId = openIdConfig;
            this.UpdatedOn = DateTime.UtcNow;

            if (string.IsNullOrEmpty(this.Issuer))
            {
                if (this.OpenId.AdditionalData.ContainsKey("access_token_issuer"))
                {
                    this.Issuer = this.OpenId.AdditionalData["access_token_issuer"]?.ToString();
                }
                else
                {
                    this.Issuer = this.OpenId.Issuer;
                }
            }
            else if(string.Compare(this.Issuer, openIdConfig.Issuer) != 0)
            {
                throw new ArgumentException($"AppSettings Issuer {this.Issuer} does not match OIDC Issuer {openIdConfig.Issuer}");
            }

            if(!this.SigningKeys.Any()) { this.SigningKeys = this.OpenId.SigningKeys; }
        }   

        /// <summary>
        /// Get the url for getting the Auth Code
        /// </summary>
        /// <param name="state">Unique code representing current user state, should not be reused</param>
        /// <param name="nonce">Used to associate client session with an id_token, embedded into ID Token</param>
        /// <returns>Url for getting Auth Code</returns>
        public string GetAuthCodeUrl(string state, string nonce)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(this.ClientId);
            ArgumentNullException.ThrowIfNullOrEmpty(this.CallbackUrl);

            // make sure we have current service definitions            
            this.CheckOpenIdDefinitionsAsync().Wait();

            if (!this.ResponseTypesSupported.Contains("code"))
            {
                throw new InvalidOperationException($"'code' response type not supported. Supported Types: {string.Join("; ", this.ResponseTypesSupported)}");
            }
                        
            var parameters = new Dictionary<string, string>
            {
                ["response_type"] = "code",
                ["scope"] = WebUtility.UrlEncode(this.Scope),
                ["state"] = state,
                ["nonce"] = nonce,

                ["client_id"] = this.ClientId,
                ["redirect_uri"] = WebUtility.UrlEncode(this.CallbackUrl.ToString())
            };

            var queryString = string.Join("&", parameters.Select((x) => $"{x.Key}={x.Value}"));

            return $"{this.AuthUrl}?{queryString}";
        }

        /// <summary>
        /// Get the url for getting the id token
        /// </summary>
        /// <param name="state">Unique code representing current user state, should not be reused</param>
        /// <param name="nonce">Used to associate client session with an id_token, embedded into ID Token</param>
        /// <returns>Url for getting ID Token</returns>
        public string GetIdTokenUrl(string state, string nonce)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(this.ClientId);
            ArgumentNullException.ThrowIfNullOrEmpty(this.ClientSecret);
            ArgumentNullException.ThrowIfNullOrEmpty(this.CallbackUrl);

            // make sure we have current service definitions            
            this.CheckOpenIdDefinitionsAsync().Wait();

            if (!this.ResponseTypesSupported.Contains("id_token"))
            {
                throw new InvalidOperationException($"'id_token' response type not supported. Supported Types: {string.Join("; ", this.ResponseTypesSupported)}");
            }

            var parameters = new Dictionary<string, string>
            {
                ["response_type"] = "id_token",
                ["scope"] = WebUtility.UrlEncode(this.Scope),
                ["state"] = state,
                ["nonce"] = nonce,

                ["client_id"] = this.ClientId,
                ["redirect_uri"] = WebUtility.UrlEncode(this.CallbackUrl)
            };

            var queryString = string.Join("&", parameters.Select((x) => $"{x.Key}={x.Value}"));

            return $"{this.AuthUrl}?{queryString}";
        }

        /// <summary>
        /// Get access token by client credentials
        /// </summary>        
        /// <returns>JWT Token Object</returns>
        public async Task<OAuth2Token> GetServiceAccountTokenAsync()
        {
            ArgumentNullException.ThrowIfNullOrEmpty(this.ClientId);
            ArgumentNullException.ThrowIfNullOrEmpty(this.ClientSecret);

            return await this.GetServiceAccountTokenAsync(this.ClientId, this.ClientSecret);
        }

        /// <summary>
        /// Get access token by client credentials
        /// </summary>        
        /// <returns>JWT Token Object</returns>
        public async Task<OAuth2Token> GetServiceAccountTokenAsync(string clientId, string clientSecret)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(clientId);
            ArgumentNullException.ThrowIfNullOrEmpty(clientSecret);

            // make sure we have current service definitions            
            await this.CheckOpenIdDefinitionsAsync();

            // make sure this grant type is supported
            if (!this.GrantTypesSupported.Contains("client_credentials"))
            {
                throw new InvalidOperationException($"'client_credentials' grant type not supported. Supported Types: {string.Join("; ", this.GrantTypesSupported)}");
            }

            var request = new HttpRequestMessage(HttpMethod.Post, this.TokenUrl);

            var parameters = new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",

                ["client_id"] = clientId,
                ["client_secret"] = clientSecret
            };

            // if no scope then leave it off
            if(this.Scopes.Count > 0)
            {
                parameters.Add("scope", WebUtility.UrlEncode(this.Scope));
            }

            request.Content = new FormUrlEncodedContent(parameters);
            
            using (var client = new HttpClient())
            {
                var response = client.Send(request);

                var json = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    //Log.Error($"AUTH ERROR: {json}");

                    response.EnsureSuccessStatusCode();
                }

                return OAuth2Token.Parse(json);
            }
        }

        /// <summary>
        /// Get the user info details for the logged in user
        /// </summary>
        /// <param name="accessToken">JWT Access Token</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public async Task<JsonResponse<Dictionary<string, string>>> GetUserInfo(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken)) { throw new ArgumentNullException(accessToken); }

            try
            {
                // make sure we have the service definitions
                if (this.OpenIdConfigUri != default)
                {
                    await this.CheckOpenIdDefinitionsAsync();
                }

                if (string.IsNullOrEmpty(this.UserInfoUrl))
                {
                    throw new InvalidOperationException("'userinfo_endpoint' is not specified.");
                }

                var request = new HttpRequestMessage(HttpMethod.Get, this.UserInfoUrl);

                using (var client = new HttpClient())
                {
                    var response = client.Send(request);

                    var json = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"AUTH ERROR: {json}");

                        response.EnsureSuccessStatusCode();
                    }

                    //"{\"error\":\"invalid_request\",\"error_description\":\"MSIS9609: The \\u0027redirect_uri\\u0027 parameter is invalid. No redirect uri with the specified value is registered for the received \\u0027client_id\\u0027. \"}"
                    var userData = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                    if(userData == null) { throw new ArgumentException("Unable to parse user info response from authentication provider."); }

                    return new(userData);
                }
            }
            catch(Exception ex)
            {
                return new JsonResponse<Dictionary<string, string>>(ex);
            }
        }

        /// <summary>
        /// Get the access token from a username and password
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<OAuth2Token> GetAccessTokenAsync(string username, string password)
        {
            if (string.IsNullOrEmpty(username)) { throw new ArgumentNullException(username); }
            if (string.IsNullOrEmpty(password)) { throw new ArgumentNullException(password); }

            if (string.IsNullOrEmpty(this.ClientId)) { throw new ArgumentException("No Client Id defined."); }
            if (string.IsNullOrEmpty(this.ClientSecret)) { throw new ArgumentException("No Client Secret defined"); }
            
            // make sure we have the service definitions
            if (this.OpenIdConfigUri != default)
            {
                await this.CheckOpenIdDefinitionsAsync();
            }

            // make sure this grant type is supported
            if (this.GrantTypesSupported.Any() && !this.GrantTypesSupported.Contains("password"))
            {
                throw new InvalidOperationException("'password' grant type not supported.");
            }

            var request = new HttpRequestMessage(HttpMethod.Post, this.TokenUrl);
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "password",                
                ["client_id"] = this.ClientId,
                ["client_secret"] = this.ClientSecret,

                ["username"] = username,
                ["password"] = password
            });

            using (var client = new HttpClient())
            {
                var response = client.Send(request);

                var json = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"AUTH ERROR: {json}");

                    response.EnsureSuccessStatusCode();
                }

                //"{\"error\":\"invalid_request\",\"error_description\":\"MSIS9609: The \\u0027redirect_uri\\u0027 parameter is invalid. No redirect uri with the specified value is registered for the received \\u0027client_id\\u0027. \"}"

                return OAuth2Token.Parse(json);
            }
        }
        /// <summary>
        /// Get the access token from a authorization code
        /// </summary>
        /// <param name="code">Authorization Code</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<OAuth2Token> GetAccessTokenAsync(string code)
        {
            if (string.IsNullOrEmpty(code)) { throw new ArgumentNullException(code); }

            if (string.IsNullOrEmpty(this.ClientId)) { throw new ArgumentException("No Client Id defined."); }
            if (string.IsNullOrEmpty(this.ClientSecret)) { throw new ArgumentException("No Client Secret defined"); }
            if (string.IsNullOrEmpty(this.CallbackUrl)) { throw new ArgumentException("No Callback Url defined."); }

            // make sure we have the service definitions
            if (this.OpenIdConfigUri != default)
            {
                await this.CheckOpenIdDefinitionsAsync();
            }

            // make sure this grant type is supported
            if (this.GrantTypesSupported.Any() && !this.GrantTypesSupported.Contains("authorization_code"))
            {
                throw new InvalidOperationException("'authorization_code' grant type not supported.");
            }

            var request = new HttpRequestMessage(HttpMethod.Post, this.TokenUrl);
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "authorization_code",
                ["code"] = code,

                ["client_id"] = this.ClientId,
                ["client_secret"] = this.ClientSecret,
                ["redirect_uri"] = this.CallbackUrl.ToString()
            });

            using (var client = new HttpClient())
            {
                var response = client.Send(request);

                var json = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"AUTH ERROR: {json}");

                    response.EnsureSuccessStatusCode();
                }

                //"{\"error\":\"invalid_request\",\"error_description\":\"MSIS9609: The \\u0027redirect_uri\\u0027 parameter is invalid. No redirect uri with the specified value is registered for the received \\u0027client_id\\u0027. \"}"

                return OAuth2Token.Parse(json);
            }
        }

        /// <summary>
        /// Refreshs the provided token 
        /// </summary>
        /// <param name="refreshToken">OAuth2 Token</param>
        /// <returns>Refreshed <see cref="OAuth2Token"/></returns>
        /// <exception cref="InvalidOperationException">If grant type is not supported.</exception>
        /// <exception cref="ArgumentException">Missing or bad arguments</exception>
        public async Task<OAuth2Token> RefreshTokenAsync(OAuth2Token refreshToken)
        {
            if (string.IsNullOrEmpty(this.ClientId)) { throw new ArgumentException("No Client Id defined."); }
            if (string.IsNullOrEmpty(this.ClientSecret)) { throw new ArgumentException("No Client Secret defined"); }

            if (refreshToken == null) { throw new ArgumentNullException(nameof(refreshToken)); }
            if (string.IsNullOrEmpty(refreshToken.RefreshToken)) { throw new ArgumentException("Refresh token not specified"); }

            // make sure we have the service definitions
            await this.CheckOpenIdDefinitionsAsync();

            // make sure this grant type is supported
            if (!this.GrantTypesSupported.Contains("refresh_token"))
            {
                throw new InvalidOperationException($"'refresh_token' grant type not supported. Supported Types: {string.Join("; ", this.GrantTypesSupported)}");
            }
                       
            // set up the request
            var request = new HttpRequestMessage(HttpMethod.Post, this.TokenUrl);
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                [ "grant_type"] = "refresh_token",
                [ "refresh_token"] = refreshToken.RefreshToken,

                [ "client_id" ] = this.ClientId,
                [ "client_secret"] = this.ClientSecret
            });

            using (var client = new HttpClient())
            {
                var response = client.Send(request);

                var json = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"AUTH ERROR: {json}");

                    response.EnsureSuccessStatusCode();
                }

                return OAuth2Token.Parse(json);
            }
        }

        #region --- Validation ---

        /// <summary>
        /// Gets the token validation parameters object based on OAuth2 client settings
        /// </summary>
        /// <returns><see cref="TokenValidationParameters"/></returns>
        /// <remarks>Clock Skew is 5 minutes to account for various server time drift</remarks>
        public async Task<TokenValidationParameters> GetTokenValidationParametersAsync()
        {
            // make sure we have service definitions
            await this.CheckOpenIdDefinitionsAsync();

            return new TokenValidationParameters()
            {
                // Ensure the token was issued by a trusted authorization server (default true):
                ValidateIssuer = !string.IsNullOrEmpty(this.Issuer),
                ValidIssuers = this.Issuer?.Split(" ,;".ToCharArray(), StringSplitOptions.RemoveEmptyEntries),

                // make sure the token isn't expired 
                RequireExpirationTime = true,       // tokens must expire to be valid
                ValidateLifetime = true,            // tokens can't already be expired

                // Ensure the token audience matches our audience value (default true):                
                ValidateAudience = !string.IsNullOrEmpty(this.Audience),
                ValidAudiences = this.Audience?.Split(" ,;".ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
                RequireAudience = !string.IsNullOrEmpty(this.Audience),

                // Specify the key used to sign the token:
                ValidateIssuerSigningKey = this.SigningKeys.Any(),
                RequireSignedTokens = this.SigningKeys.Any(),
                IssuerSigningKeys = this.SigningKeys,
                
                // set the core claim types
                NameClaimType = this.NameClaimType ?? ClaimTypes.Name,
                RoleClaimType = this.RoleClaimType ?? ClaimTypes.Role, 

                // Clock skew compensates for server time drift.                
                ClockSkew = TimeSpan.FromMinutes(5)
            };
        }

        /// <summary>
        /// Validate the jwt token string
        /// </summary>
        /// <param name="jwt">JWT string</param>
        /// <returns>List of validation issues</returns>
        public async Task<Dictionary<string, string>> ValidateJwtAsync(string jwt)
        {
            if (string.IsNullOrEmpty(jwt))
            {
                return new() { { "JwtToken", "No JWT token information provided." } };
            }

            // incase it is a Bearer token
            if (jwt.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)) { jwt = jwt.Substring("Bearer ".Length); }

            // PARSE THE TOKEN
            JwtSecurityToken jwtToken;
            try
            {
                jwtToken = new JwtSecurityToken(jwt);
            }
            catch
            {
                return new() { { "JwtToken", "Unable to parse security token." } };
            }

            return await this.ValidateJwtAsync(jwtToken);
        }

        public async Task<Dictionary<string, string>> ValidateJwtAsync(JwtSecurityToken jwtToken)
        {
            if (jwtToken == null)
            {
                return new() { { "JwtToken", "No JWT token provided." } };
            }

            var results = new Dictionary<string, string>();
            var validationParameters = await this.GetTokenValidationParametersAsync();
            
            if (this.DisableValidation)
            {
                //Log.Warning("JWT Validation Disabled!");
                results.Add("ValidationDisabled", "JWT token validation is currently disabled!");
            }

            // ================================================================================
            // Lifetime
            // ================================================================================
            //// make sure the token isn't expired 
            //RequireExpirationTime = true,       // tokens must expire to be valid
            //ValidateLifetime = true,            // tokens can't already be expired

            if (validationParameters.ValidateLifetime)
            {
                if (jwtToken.ValidTo.Add(validationParameters.ClockSkew) < DateTime.UtcNow)
                {
                    results.Add("ValidTo", $"Token expired on '{jwtToken.ValidTo}'.");
                }
                                
                if (jwtToken.ValidFrom != default && jwtToken.ValidFrom.Subtract(validationParameters.ClockSkew) > DateTime.UtcNow)
                {
                    results.Add("ValidFrom", $"Token not yet valid '{jwtToken.ValidFrom}'.");
                }
            }

            // ================================================================================
            // Issuer
            // ================================================================================
            // Ensure the token was issued by a trusted authorization server (default true):
            //ValidateIssuer = !string.IsNullOrEmpty(this.Issuer),
            //ValidIssuers = this.Issuer?.Split(" ,;".ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
            if (validationParameters.ValidateIssuer && !validationParameters.ValidIssuers.Any(x => string.Compare(x, jwtToken.Issuer, true) == 0))
            {
                results.Add("Issuer", $"Issuer '{jwtToken.Issuer}' not listed as valid issuer.");
            }

            // ================================================================================
            // Audience Validation
            // ================================================================================            
            //ValidateAudience = !string.IsNullOrEmpty(this.Audience),
            //ValidAudiences = this.Audience?.Split(" ,;".ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
            //RequireAudience = !string.IsNullOrEmpty(this.Audience),

            if (validationParameters.ValidateAudience)
            {
                if (validationParameters.RequireAudience && !jwtToken.Audiences.Any())
                {
                    results.Add("Audience", $"Audience required but none provided.");
                }
                else if (!validationParameters.ValidAudiences.Any(x => jwtToken.Audiences.Contains(x)))
                {
                    results.Add("Audience", $"Audience '{string.Join(", ", jwtToken.Audiences)}' not listed as a valid audiences.");
                }
            }

            // ================================================================================
            // Signature
            // ================================================================================
            //// Specify the key used to sign the token:
            //ValidateIssuerSigningKey = this.SigningKeys.Any(),
            //RequireSignedTokens = this.SigningKeys.Any(),
            //IssuerSigningKeys = this.SigningKeys,

            var tokenHandler = new JwtSecurityTokenHandler();
            foreach (var securityKey in this.SigningKeys)
            {
                //jwtToken..SigningKey = securityKey;

            }
            

            return results;
        }

        /// <summary>
        /// Parse the jwt token and validate it against our validation rules
        /// </summary>
        /// <param name="jwt"></param>
        /// <returns><see cref="ClaimsPrincipal"/></returns>
        public async Task<ClaimsPrincipal?> ParseAndValidateAsync(string jwt)
        {
            if (jwt.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)) { jwt = jwt.Substring("Bearer ".Length); }
                        
            TokenValidationResult? result = null;
            var tries = 0;

            while (result == null && tries <= 1)
            {
                try
                {
                    var validationParameters = await this.GetTokenValidationParametersAsync();

                    var tokenHandler = new JwtSecurityTokenHandler();                    
                    tokenHandler.InboundClaimTypeMap.Clear();   //STOP MAPPING TO F-ING XML SCHEMAS!

                    result = await tokenHandler.ValidateTokenAsync(jwt, validationParameters);                    
                }
                catch (SecurityTokenSignatureKeyNotFoundException)
                {
                    // This exception is thrown if the signature key of the JWT could not be found.
                    // This could be the case when the issuer changed its signing keys, so we trigger a refresh and retry validation.

                    //Log.Error("Security Token Signature Key Not Found, did the key change?");
                    //Log.Error(ex, ex.Message);

                    await this.FetchOpenIdDefinitionsAsync();
                    tries++;
                }
            }

            if (result?.IsValid == true)
            {
                return new ClaimsPrincipal(result.ClaimsIdentity);
            }
            else if (this.DisableValidation)
            {
                return this.ParseJwt(jwt);
            }
            else
            {
                // no logged in user
                return null;
            }
        }

        /// <summary>
        /// Parse the jwt token based on claim types and parser
        /// </summary>
        /// <param name="jwt"></param>
        /// <returns><see cref="ClaimsPrincipal"/></returns>
        public ClaimsPrincipal? ParseJwt(string jwt)
        {
            if (jwt.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)) { jwt = jwt.Substring("Bearer ".Length); }

            try
            {
                var token = new JwtSecurityToken(jwt);
                
                var claimsIdentity = new ClaimsIdentity(token.Claims, "Bearer", this.NameClaimType, this.RoleClaimType);
                
                return new ClaimsPrincipal(claimsIdentity);
            }
            catch//(Exception ex)
            {
                //Log.Warning("Unable to parse JWT token.");
                //Log.Warning(ex, ex.Message);

                return null;
            }                         
        }

        //private TokenValidationParameters GetTokenValidationParameters()
        //{
        //    var jwt = App.Service.Canvas!.Auth!.Jwt!;

        //    if (!jwt.DisableValidation)
        //    {
        //        return new TokenValidationParameters
        //        {
        //            // issuer
        //            ValidateIssuer = !(string.IsNullOrEmpty(jwt.Issuer)),
        //            ValidIssuer = jwt.Issuer,

        //            // audience
        //            ValidateAudience = !string.IsNullOrEmpty(jwt.Audience),
        //            ValidAudience = jwt.Audience?.ToString(),
        //            RequireAudience = !string.IsNullOrEmpty(jwt.Audience),

        //            // signature
        //            ValidateIssuerSigningKey = true,
        //            IssuerSigningKeys = this.OpenIdConnectConfiguration?.SigningKeys,
        //            RequireSignedTokens = true,

        //            // lifetime
        //            ValidateLifetime = true
        //        };
        //    }
        //    else
        //    {
        //        return new TokenValidationParameters
        //        {
        //            ValidateIssuer = false,
        //            ValidateAudience = false,
        //            RequireAudience = false,
        //            ValidateIssuerSigningKey = false,
        //            RequireSignedTokens = false,

        //            ValidateLifetime = false,

        //            // fool the validation logic since we aren't validating tokens
        //            SignatureValidator = delegate (string token, TokenValidationParameters parameters)
        //            {
        //                return new JwtSecurityToken(token);
        //            }
        //        };
        //    }
        //}

        //private bool IsScopeValid(ClaimsPrincipal claimsPrincipal, string scopeName)
        //{
        //    if (claimsPrincipal == null)
        //    {
        //        Log.Warning("Scope invalid {ScopeName}", scopeName);
        //        return false;
        //    }

        //    var scopeClaim = claimsPrincipal.Claims.FirstOrDefault(x => x.Type.StartsWith("http://schemas.microsoft.com/identity/claims/scope"))?.Value;
        //    if (string.IsNullOrEmpty(scopeClaim))
        //    {
        //        Log.Warning("Scope invalid {ScopeName}", scopeName);
        //        return false;
        //    }

        //    if (!scopeClaim.EndsWith(scopeName, StringComparison.OrdinalIgnoreCase))
        //    {
        //        Log.Warning("Scope invalid {ScopeName}", scopeName);
        //        return false;
        //    }

        //    Log.Debug("Scope valid {ScopeName}", scopeName);
        //    return true;
        //}

        /// <summary>
        /// Get the username from the possible user name fields
        /// </summary>
        /// <returns></returns>
        public string GetPreferredUsername(ClaimsPrincipal claimsPrincipal)
        {
            // find a unique username to use
            return claimsPrincipal.GetClaim("preferred_username", "upn", "username", "email", "unique_name");
        }

        ///// <summary>
        ///// Get the auth object ID from known object ID fields
        ///// </summary>
        ///// <returns></returns>
        //public string GetObjectId()
        //{
        //    // try to find the user based on the object id (if exists)
        //    var oid = this.ClaimsPrincipal.Claims.SingleOrDefault(c => c.Type == "oid")?.Value.ToLower();
        //    if (oid == null) { oid = this.ClaimsPrincipal.Claims.SingleOrDefault(c => c.Type == "jti")?.Value.ToLower(); } //JTI is a JWT ID 

        //    return oid;
        //}

        ///// <summary>
        ///// Parse the jwt token and validate it against our validation rules
        ///// </summary>
        ///// <param name="jwt"></param>
        ///// <returns><see cref="ClaimsPrincipal"/></returns>
        //public async Task<ClaimsPrincipal> ParseAndValidateNew(string jwt)
        //{
        //    var validationParameters = this.GetTokenValidationParameters();

        //    TokenValidationResult result = null;
        //    var tries = 0;

        //    while (result == null && tries <= 1)
        //    {
        //        try
        //        {
        //            var tokenHandler = new JwtSecurityTokenHandler();

        //            result = await tokenHandler.ValidateTokenAsync(jwt, validationParameters);
        //        }
        //        catch (SecurityTokenSignatureKeyNotFoundException ex)
        //        {
        //            // This exception is thrown if the signature key of the JWT could not be found.
        //            // This could be the case when the issuer changed its signing keys, so we trigger a refresh and retry validation.

        //            Log.Error("Security Token Signature Key Not Found, did the key change?");
        //            Log.Error(ex, ex.Message);

        //            await this.FetchOpenIdDefinitionsAsync();
        //            tries++;
        //        }
        //    }

        //    if (result.IsValid)
        //    {
        //        return new ClaimsPrincipal(result.ClaimsIdentity);
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        #endregion
    }

    file sealed class JsonHelper
    {
        private static readonly Func<object, object?, bool> AlwaysSerialize = (_, __) => true;

        /// <summary>
        /// NSwag generates JsonIgnoreCondition.WhenWritingDefault for value type properties (enum, int, bool, etc.) which is wrong.
        /// </summary>
        public static void AlwaysSerializeValueTypeProperties(JsonTypeInfo ti)
        {
            // https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/custom-contracts

            foreach (var prop in ti.Properties)
            {
                if (prop.PropertyType.IsValueType && prop.AttributeProvider is not null)
                {
                    prop.ShouldSerialize = AlwaysSerialize;
                }
            }
        }
    }
}
