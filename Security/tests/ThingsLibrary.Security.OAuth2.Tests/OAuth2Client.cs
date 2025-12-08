// ================================================================================
// <copyright file="OAuth2Client.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace ThingsLibrary.Security.OAuth2.Tests
{
    [TestClass, ExcludeFromCodeCoverage]
    public class OAuth2Tests
    {        
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            //nothing
        }

        [TestMethod]
        public void Constructor()
        {
            var now = DateTime.UtcNow;

            var oauth2Url = "https://test.com/oauth2/.well-known/openid-configuration";

            var oauth2 = OAuth2Tests.GetTestOauth2(oauth2Url);

            Assert.AreEqual(new Uri(oauth2Url), oauth2.OpenIdConfigUri);
            Assert.AreEqual("1234567890", oauth2.ClientId);
            Assert.AreEqual("0987654321", oauth2.ClientSecret);
            Assert.AreEqual($"{oauth2Url}/login", oauth2.CallbackUrl);

            //Assert.AreEqual(oauth2Url, oauth2.Issuer);
            Assert.AreEqual($"{oauth2Url}/authorize", oauth2.AuthUrl);
            Assert.AreEqual($"{oauth2Url}/token", oauth2.TokenUrl);
            Assert.AreEqual($"{oauth2Url}/userinfo", oauth2.UserInfoUrl);
            Assert.AreEqual($"{oauth2Url}/.well-known/jwks", oauth2.JwksUrl);
            Assert.AreEqual($"{oauth2Url}/logout", oauth2.EndSessionUrl);
                        
            Assert.IsTrue(oauth2.ResponseTypesSupported.Contains("token"));
            Assert.IsTrue(oauth2.GrantTypesSupported.Contains("refresh_token"));
            Assert.IsTrue(oauth2.ScopesSupported.Contains("openid"));
            Assert.IsTrue(oauth2.ClaimsSupported.Contains("aud"));
            
            Assert.AreEqual(null, oauth2.EncryptionKeys);
            Assert.IsTrue(oauth2.UpdatedOn > now);

            //// update a property
            //var issuerUrl = "https://test.com/token";
            //oauth2.SetIssuer(issuerUrl);
            //Assert.AreEqual(issuerUrl, oauth2.Issuer);
        }

        [TestMethod]
        public void Constructor_Scope()
        {
            var oauth2 = new OAuth2Client()
            {
                Scope = "openid email"                
            };

            Assert.IsTrue(oauth2.Scopes.Contains("email"));
            Assert.AreEqual("openid email", oauth2.Scope);

            // since no scopes supported is defined then we can add anything without error
            oauth2.AddScope("something");
            Assert.IsTrue(oauth2.Scopes.Contains("something"));            
        }

        [TestMethod]
        public void AddScope()
        {
            var oauth2Url = "https://test.com/oauth2";

            var oauth2 = OAuth2Tests.GetTestOauth2(oauth2Url);

            var scopeCount = oauth2.Scopes.Count;
            oauth2.AddScope("profile");
            Assert.IsTrue(oauth2.Scopes.Contains("profile"));
            Assert.IsTrue(oauth2.Scopes.Count == scopeCount + 1);

            // add again since that should not do anything
            scopeCount = oauth2.Scopes.Count;
            oauth2.AddScope("profile");
            Assert.IsTrue(oauth2.Scopes.Count == scopeCount);

            // BAD DATA
            //Assert.Throws<ArgumentException>(() => oauth2.AddScope("not_supported"));
        }

        [TestMethod]
        public void FetchDefinitions_Integration()
        {
            var oauth2Url = "https://login.microsoftonline.com/common/v2.0/.well-known/openid-configuration";

            var oauth2 = new OAuth2Client(new Uri(oauth2Url));
            oauth2.FetchOpenIdDefinitionsAsync().Wait();

            Assert.AreEqual("https://login.microsoftonline.com/common/oauth2/v2.0/token", oauth2.TokenUrl);
            Assert.AreEqual("https://login.microsoftonline.com/common/discovery/v2.0/keys", oauth2.JwksUrl);
            Assert.IsTrue(oauth2.ScopesSupported.Contains("openid"));
        }

        [TestMethod]
        public void FetchDefinitions_BadData()
        {            
            var oauth2 = new OAuth2Client();
            Assert.Throws<AggregateException>(() => oauth2.FetchOpenIdDefinitionsAsync().Wait());
            Assert.Throws<ArgumentNullException>(() => oauth2.UpdateServiceDefinitions(null));
        }

        [TestMethod]
        public void GetAuthCodeUrl()
        {
            var oauth2 = new OAuth2Client()
            {
                ClientId = "123456",
                CallbackUrl = "https://test.com/oauth2/login"                
            };
            oauth2.UpdateServiceDefinitions(OpenIdConfigurationTests.GetTestOpenIdConnectConfiguration("https://test.com/oauth2"));
            oauth2.AddScope("openid");

            var authCodeUri = new Uri(oauth2.GetAuthCodeUrl("123", "456"));
            var queryString = System.Web.HttpUtility.ParseQueryString(authCodeUri.Query);

            Assert.AreEqual("code", queryString["response_type"]);
            Assert.AreEqual(oauth2.ClientId, queryString["client_id"]);
            Assert.AreEqual(oauth2.Scope, queryString["scope"]);
            Assert.AreEqual("123", queryString["state"]);
            Assert.AreEqual("456", queryString["nonce"]);

            // BAD DATA          
            Assert.Throws<ArgumentException>(() => new OAuth2Client() { ClientId = "", CallbackUrl = "https://test.com/login" }.GetAuthCodeUrl("123", "456"));
            Assert.Throws<ArgumentException>(() => new OAuth2Client() { ClientId = "123456", CallbackUrl = "" }.GetAuthCodeUrl("123", "456"));
        }

        [TestMethod]
        public void GetIdTokenUrl()
        {
            var oauth2 = new OAuth2Client()
            {
                ClientId = "123456",
                ClientSecret = "SecretSauce",
                CallbackUrl = "https://test.com/oauth2/login"                
            };
            oauth2.UpdateServiceDefinitions(OpenIdConfigurationTests.GetTestOpenIdConnectConfiguration("https://test.com/oauth2"));
            oauth2.AddScope("openid");

            var idTokenUri = new Uri(oauth2.GetIdTokenUrl("123", "456"));
            var queryString = System.Web.HttpUtility.ParseQueryString(idTokenUri.Query);

            Assert.AreEqual("id_token", queryString["response_type"]);
            Assert.AreEqual(oauth2.ClientId, queryString["client_id"]);
            Assert.AreEqual(oauth2.Scope, queryString["scope"]);
            Assert.AreEqual("123", queryString["state"]);
            Assert.AreEqual("456", queryString["nonce"]);

            // BAD DATA          
            Assert.Throws<ArgumentException>(() => new OAuth2Client() { ClientId = "", ClientSecret = "4567", CallbackUrl = "https://test.com/login" }.GetIdTokenUrl("123", "456"));
            Assert.Throws<ArgumentException>(() => new OAuth2Client() { ClientId = "123456", ClientSecret = "", CallbackUrl = "https://test.com/login" }.GetIdTokenUrl("123", "456"));
            Assert.Throws<ArgumentException>(() => new OAuth2Client() { ClientId = "123456", ClientSecret = "4567", CallbackUrl = "" }.GetIdTokenUrl("123", "456"));            
        }

        [TestMethod]
        public void GetValidationParameters()
        {
            var oauth2 = new OAuth2Client(new Uri("https://test.com/oauth2"))
            {
                Audience = "Test1234",
                ClientId = "123456",
                CallbackUrl = "/login"                
            };
            oauth2.UpdateServiceDefinitions(OpenIdConfigurationTests.GetTestOpenIdConnectConfiguration("https://test.com/oauth2"));

            var validationParameters = oauth2.GetTokenValidationParametersAsync().Result;

            Assert.IsTrue(validationParameters.ValidateAudience);
            Assert.AreEqual("Test1234", validationParameters.ValidAudiences.First());

            //// access_token_issuer test
            //oauth2 = new OAuth2Client();           
            //oauth2.UpdateServiceDefinitions(new OpenIdConnectConfiguration(JsonSerializer.Serialize(new OpenIdConfigurationTests
            //{                
            //    AccessTokenIssuer = "https://test.com/o3"
            //})));

            //validationParameters = oauth2.GetTokenValidationParametersAsync().Result;
            //Assert.AreEqual("https://test.com/o3", validationParameters.ValidIssuers.First());
        }

        [TestMethod]
        public void GetUrls_BadData()
        {
            var oauth2 = new OAuth2Client()
            {
                ClientId = "1234567",
                ClientSecret = "7654321",
                CallbackUrl = "callback",
                Scopes = new List<string> { "openid", "email" }                
            };

            var config = new OpenIdConnectConfiguration();

            new List<string> { "token", "token id_token" }.ForEach(x => config.ResponseTypesSupported.Add(x));
            new List<string> { "implicit", "password", "impersonate" }.ForEach(x => config.GrantTypesSupported.Add(x));
            
            oauth2.UpdateServiceDefinitions(config);

            // BAD DATA
            Assert.Throws<InvalidOperationException>(() => oauth2.GetAuthCodeUrl("123", "456"));
            Assert.Throws<InvalidOperationException>(() => oauth2.GetIdTokenUrl("123", "456"));
        }

        [TestMethod]
        public void GetAccessToken_BadData()
        {
            var oauth2 = new OAuth2Client()
            {
                ClientId = "1234567",
                ClientSecret = "7654321",
                CallbackUrl = "callback"                
            };

            var config = new OpenIdConnectConfiguration();
            new List<string> { "implicit", "password", "impersonate" }.ForEach(x => config.GrantTypesSupported.Add(x));

            oauth2.UpdateServiceDefinitions(config);

            //Assert.Throws<AggregateException>(() => oauth2.GetAccessTokenAsync().Wait());
            Assert.Throws<AggregateException>(() => oauth2.GetAccessTokenAsync("12345").Result);

            //BAD DATA
            Assert.Throws<AggregateException>(() => new OAuth2Client() { ClientId = "Something", ClientSecret = "Something", CallbackUrl = "Something" }.GetAccessTokenAsync(null).Result);
            Assert.Throws<AggregateException>(() => new OAuth2Client() { ClientId = "Something", ClientSecret = "Something", CallbackUrl = "Something" }.GetAccessTokenAsync("").Result);

            //Assert.Throws<AggregateException>(() => new OAuth2Client() { ClientId = "", ClientSecret = "Something", CallbackUrl = "Something" }.GetAccessTokenAsync().Wait());
            //Assert.Throws<AggregateException>(() => new OAuth2Client() { ClientId = "Something", ClientSecret = "", CallbackUrl = "Something" }.GetAccessTokenAsync().Wait());
                        
            Assert.Throws<AggregateException>(() => new OAuth2Client() { ClientId = "", ClientSecret = "Something", CallbackUrl = "Something" }.GetAccessTokenAsync("12345").Result);
            Assert.Throws<AggregateException>(() => new OAuth2Client() { ClientId = "Something", ClientSecret = "", CallbackUrl = "Something" }.GetAccessTokenAsync("12345").Result);
            Assert.Throws<AggregateException>(() => new OAuth2Client() { ClientId = "Something", ClientSecret = "Something", CallbackUrl = null }.GetAccessTokenAsync("12345").Result);
        }

        [TestMethod]
        public void RefreshToken_BadData()
        {
            var oauth2 = new OAuth2Client()
            {
                ClientId = "1234567",
                ClientSecret = "7654321",
                CallbackUrl = "callback"
            };

            var config = new OpenIdConnectConfiguration();
            new List<string> { "refresh_token", "password", "impersonate" }.ForEach(x => config.GrantTypesSupported.Add(x));

            oauth2.UpdateServiceDefinitions(config);

            var token = new OAuth2Token();

            Assert.Throws<AggregateException>(() => oauth2.RefreshTokenAsync(null).Result);
            Assert.Throws<AggregateException>(() => oauth2.RefreshTokenAsync(token).Result);

            // BAD DATA
            Assert.Throws<AggregateException>(() => new OAuth2Client() { ClientId = "", ClientSecret = "Something" }.RefreshTokenAsync(token).Result);
            Assert.Throws<AggregateException>(() => new OAuth2Client() { ClientId = "Something", ClientSecret = "" }.RefreshTokenAsync(token).Result);
            Assert.Throws<AggregateException>(() => new OAuth2Client() { ClientId = "", ClientSecret = "Something" }.RefreshTokenAsync(null).Result);
            Assert.Throws<AggregateException>(() => new OAuth2Client() { ClientId = "Something", ClientSecret = "" }.RefreshTokenAsync(token).Result);

            Assert.Throws<AggregateException>(() => new OAuth2Client() { ClientId = "Something", ClientSecret = "Something" }.RefreshTokenAsync(new OAuth2Token() { RefreshToken = "12345"}).Result);
        }



        //[TestMethod]
        //public void ValidateJwt()
        //{
        //    var securityKey = GenerateSecurityKey("asdv234234^&%&^%&^hjsdfb2%%%;asdv234234^&%&^%&^hjsdfb2%%%;asdv234234^&%&^%&^hjsdfb2%%%;asdv234234^&%&^%&^hjsdfb2%%%;asdv234234^&%&^%&^hjsdfb2%%%;"); //I don't know it looks cool.

        //    var issuerUrl = "https://test.com/oauth2";
        //    var audience = "10000000-0000-0000-0000-100000000001";            
        //    var userName = "bsmith";
                        
        //    var openIdConfig = OpenIdConfigurationTests.GetTestOpenIdConfig(issuerUrl);
        //    openIdConfig.SigningKeys.Add(securityKey);

        //    var oauth2 = new OAuth2Client()
        //    {
        //        Audience = audience,
        //        Scopes = new List<string> { "openid", "email" }                
        //    };

        //    oauth2.UpdateServiceDefinitions(openIdConfig);

        //    // valid token test case
        //    ClaimsPrincipal testPrincipal;

        //    var jwt = GenerateJwtToken(issuerUrl, audience, securityKey, userName, DateTime.UtcNow,  DateTime.UtcNow.AddDays(7));
        //    testPrincipal = oauth2.ParseAndValidateAsync(jwt).Result;
        //    Assert.AreEqual(7, testPrincipal.Claims.Count());

        //    // BAD DATA
        //    jwt = GenerateJwtToken(issuerUrl, audience, securityKey, userName, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow.AddDays(-2));
        //    testPrincipal = oauth2.ParseAndValidateAsync(jwt).Result;
        //    Assert.AreEqual(0, testPrincipal.Claims.Count());

        //    jwt = GenerateJwtToken(issuerUrl, "BADAUDIENCE", securityKey, userName, DateTime.UtcNow, DateTime.UtcNow.AddDays(7));
        //    testPrincipal = oauth2.ParseAndValidateAsync(jwt).Result;
        //    Assert.AreEqual(0, testPrincipal.Claims.Count());
        //}

        #region --- Static Methods ---

        private static OAuth2Client GetTestOauth2(string oauth2Url)
        {            
            var clientId = "1234567890";
            var clientSecret = "0987654321";
            var callbackUrl = $"{oauth2Url}/login";

            // test object
            var oauth2Client = new OAuth2Client(new Uri(oauth2Url))
            {                
                ClientId = clientId,
                ClientSecret = clientSecret,
                CallbackUrl = callbackUrl,

                Scopes = new List<string> { "openid", "email" }
            };

            oauth2Client.UpdateServiceDefinitions(OpenIdConfigurationTests.GetTestOpenIdConnectConfiguration(oauth2Url));

            return oauth2Client;
        }

        public static string GenerateJwtToken(string issuerUrl, string audience, SymmetricSecurityKey securityKey, string userId, DateTime startOn, DateTime expiresOn)
        {   
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim("user_id", userId)
                }),
                NotBefore = startOn,
                Expires = expiresOn,
                Issuer = issuerUrl,
                Audience = audience,
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public static SymmetricSecurityKey GenerateSecurityKey(string secret)
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
        }

        #endregion
    }
}
