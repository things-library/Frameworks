using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace ThingsLibrary.Security.OAuth2.Tests
{
    [TestClass, ExcludeFromCodeCoverage]
    public class OpenIdConfigurationTests
    {
        [TestMethod]
        public void Constructor()
        {
            var oauth2Url = "https://test.com/oauth2";

            var openId = OpenIdConfigurationTests.GetTestOpenIdConnectConfiguration(oauth2Url);

            Assert.AreEqual(oauth2Url, openId.Issuer);
            Assert.AreEqual($"{oauth2Url}/authorize", openId.AuthorizationEndpoint);
            Assert.AreEqual($"{oauth2Url}/token", openId.TokenEndpoint);
            Assert.AreEqual($"{oauth2Url}/.well-known/jwks", openId.JwksUri);
            Assert.AreEqual($"{oauth2Url}/userinfo", openId.UserInfoEndpoint);
            Assert.AreEqual($"{oauth2Url}/clients", openId.RegistrationEndpoint);
            Assert.AreEqual($"{oauth2Url}/logout", openId.EndSessionEndpoint);

            Assert.AreEqual("code", openId.ResponseTypesSupported.First());
            Assert.AreEqual("authorization_code", openId.GrantTypesSupported.First());
            Assert.AreEqual("public", openId.SubjectTypesSupported.First());
            Assert.AreEqual("RS256", openId.IdTokenSigningAlgValuesSupported.First());
            Assert.AreEqual("openid", openId.ScopesSupported.First());
            Assert.AreEqual("iss", openId.ClaimsSupported.First());
        }

        public static OpenIdConnectConfiguration GetTestOpenIdConnectConfiguration(string oauth2Url)
        {
            var config = new OpenIdConnectConfiguration
            {
                Issuer = oauth2Url,
                AuthorizationEndpoint = $"{oauth2Url}/authorize",
                TokenEndpoint = $"{oauth2Url}/token",
                JwksUri = $"{oauth2Url}/.well-known/jwks",
                UserInfoEndpoint = $"{oauth2Url}/userinfo",
                RegistrationEndpoint = $"{oauth2Url}/clients",
                EndSessionEndpoint = $"{oauth2Url}/logout",
            };

            new List<string> { "code", "token", "id_token", "token id_token" }.ForEach(x => config.ResponseTypesSupported.Add(x));
            new List<string> { "authorization_code", "implicit", "refresh_token", "password", "client_credentials", "impersonate" }.ForEach(x => config.GrantTypesSupported.Add(x));
            new List<string> { "openid", "email", "profile", "offline_access" }.ForEach(x => config.ScopesSupported.Add(x));
            new List<string> { "iss", "ver", "sub", "aud", "iat", "exp", "email", "profile" }.ForEach(x => config.ClaimsSupported.Add(x));

            config.SubjectTypesSupported.Add("public");
            config.IdTokenSigningAlgValuesSupported.Add("RS256");

            return config;
        }
    }
}
