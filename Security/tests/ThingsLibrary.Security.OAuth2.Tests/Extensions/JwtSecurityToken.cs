// ================================================================================
// <copyright file="JwtSecurityToken.cs" company="Starlight Software Co">
//    Copyright (c) 2025 Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

namespace ThingsLibrary.Security.OAuth2.Tests.Extensions
{
    [TestClass, ExcludeFromCodeCoverage]
    public class JwtSecurityTokenTests
    {
        public static JwtSecurityToken TOKEN;
                
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            TOKEN = GenerateTestToken(DateTime.UtcNow, DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(-1));    // Valid > -1 hr, < +1 hr                    
        }

        [TestMethod]
        public void Scopes()
        {
            var scopes = TOKEN.Scopes();

            Assert.IsTrue(scopes.Count() == 3);
        }

        [DataTestMethod]
        [DataRow("sub", "1234567890")]
        [DataRow("unique_name", "DOMAIN\\UserName")]
        [DataRow("email", "testuser@test.com")]
        [DataRow("BAD", "")]
        public void GetClaim(string testValue, string expectedValue)
        {
            Assert.AreEqual(expectedValue, TOKEN.GetClaim(testValue));
        }

        [DataTestMethod]
        [DataRow(null, null, null, 0, true)]       //defaults to: UtcNow, UtcNow, UtcNow + 1hr, 
        [DataRow(0, -10, 10, 0, true)]
        [DataRow(0, -10, -1, 0, false)]
        [DataRow(0, -10, 1, 0, true)]
        
        [DataRow(0, -10, -1, 0, false)]         // almost expired
        [DataRow(0, -10, -1, 2, true)]          // almost expired
        [DataRow(0, -10, -2, 1, false)]         // clock skew but still not valid

        [DataRow(0, 1, 10, 0, false)]           // starting to be valid
        [DataRow(0, 1, 10, 2, true)]            // starting to be valid
        [DataRow(0, 3, 10, 1, false)]           // clock skew but still not valid
        public void IsLifetimeValid_Skew(int? issuedAddMinutes, int? notBeforeAddMinutes, int? expiresAtAddMinutes, int clockSkew, bool expectedValue)
        {
            DateTime? issuedAt = (issuedAddMinutes != null ? DateTime.UtcNow.AddMinutes(issuedAddMinutes.Value) : null);
            
            DateTime? notBeforeAt = (notBeforeAddMinutes != null ? DateTime.UtcNow.AddMinutes(notBeforeAddMinutes.Value) : null);
            DateTime? expiresAt = (expiresAtAddMinutes != null ? DateTime.UtcNow.AddMinutes(expiresAtAddMinutes.Value) : null);

            var token = GenerateTestToken(issuedAt, expiresAt, notBeforeAt);

            if(clockSkew == 0)
            {
                Assert.AreEqual(expectedValue, token.IsLifetimeValid(null));
            }
            else
            {
                Assert.AreEqual(expectedValue, token.IsLifetimeValid(TimeSpan.FromMinutes(clockSkew)));
            }
        }


        private static JwtSecurityToken GenerateTestToken(DateTime? issuedAt, DateTime? expiresAt, DateTime? notBeforeAt = null)
        {
            // Generates Structure Like this:
            //{
            //  "alg": "HS256",
            //  "typ": "Bearer"
            //}.{
            //  "sub": "1234567890",
            //  "unique_name": "DOMAIN\\UserName",
            //  "email": "testuser@test.com",
            //  "given_name": "John",
            //  "family_name": "Doe",
            //  "sid": "S-12345-6789",
            //  "nbf": 1658592315,
            //  "exp": 1658595915,
            //  "iat": 1658592315,
            //  "iss": "https://www.testsite.com",
            //  "aud": "http://myaudience.com"
            //}.[Signature]

            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("asdv234234^&%&^%&^hjsdfb2%%%;asdv234234^&%&^%&^hjsdfb2%%%;asdv234234^&%&^%&^hjsdfb2%%%;asdv234234^&%&^%&^hjsdfb2%%%;asdv234234^&%&^%&^hjsdfb2%%%;"));
            var signingCreds = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature);

            //create a identity and add claims to the user which we want to log in
            var claimsIdentity = new ClaimsIdentity(new[]
            {                
                new Claim(JwtRegisteredClaimNames.Sub, "1234567890"),                
                new Claim(JwtRegisteredClaimNames.UniqueName, "DOMAIN\\UserName"),
                new Claim(JwtRegisteredClaimNames.Email, "testuser@test.com"),
                new Claim(JwtRegisteredClaimNames.GivenName, "John"),
                new Claim(JwtRegisteredClaimNames.FamilyName, "Doe"),
                new Claim(JwtRegisteredClaimNames.Sid, "S-12345-6789"),

                new Claim("scp", "Role1"),
                new Claim("scp", "Role2"),
                new Claim("scp", "Role3")
            });

            var tokenHandler = new JwtSecurityTokenHandler();

            return tokenHandler.CreateJwtSecurityToken(new SecurityTokenDescriptor
            {
                TokenType = "Bearer",
                Subject = claimsIdentity,                
                Issuer = "https://www.testsite.com",
                Audience = "http://myaudience.com",
                NotBefore = notBeforeAt,
                IssuedAt = issuedAt,
                Expires = expiresAt,                
                SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature)
            });
        }
    }
}
