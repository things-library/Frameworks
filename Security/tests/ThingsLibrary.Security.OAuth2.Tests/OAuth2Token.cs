// ================================================================================
// <copyright file="OAuth2Token.cs" company="Starlight Software Co">
//    Copyright (c) Starlight Software Co. All rights reserved.
//    Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// </copyright>
// ================================================================================

using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace ThingsLibrary.Security.OAuth2.Tests
{
    [TestClass, ExcludeFromCodeCoverage]
    public class OAuth2TokenTests
    {
        [TestMethod]
        public void Constructor()
        {
            var testToken = new OAuth2Token
            {
                TokenType = "Bearer"
            };

            Assert.AreEqual("Bearer", testToken.TokenType);
            Assert.AreEqual(null, testToken.RefreshToken);
            Assert.AreEqual(null, testToken.AccessToken);
            Assert.AreEqual(null, testToken.IdToken);

            Assert.AreEqual("", testToken.ObjectId);
            Assert.AreEqual("", testToken.UserEmail);
        }

        [TestMethod]
        public void Constructor_Details()
        {
            //var handler = new JwtSecurityTokenHandler();

            var testToken = new OAuth2Token
            {
                TokenType = "Bearer",
                JwtIdToken = OAuth2TokenTests.Token3Jwt,
                JwtAccessToken = OAuth2TokenTests.Token3Jwt,
                RefreshToken = "REFRESH TOKEN"
            };

            Assert.AreEqual("Bearer", testToken.TokenType);
            Assert.AreEqual("REFRESH TOKEN", testToken.RefreshToken);
            Assert.AreEqual(OAuth2TokenTests.Token3Jwt.RawData, testToken.AccessToken);
            Assert.AreEqual(OAuth2TokenTests.Token3Jwt.RawData, testToken.IdToken);

            // ID Token
            Assert.AreEqual(13, testToken.JwtIdToken.Claims.Count());
            Assert.AreEqual(637979963600000000, testToken.JwtIdToken.ValidTo.Ticks);

            //Assert.AreEqual("ff563d9c-f278-4fd3-b51f-3636f358593b", testToken.UserId);
            Assert.AreEqual("chikateanagha@gmail.com", testToken.UserEmail);
        }

        [TestMethod]
        public void Parse()
        {
            var refreshToken = "REFRESH_TOKEN";

            var testToken = OAuth2Token.Parse(OAuth2TokenTests.TokenJwt.RawData, null, refreshToken);

            // token parsing should not change the refresh token
            Assert.AreEqual(refreshToken, testToken.RefreshToken);
            Assert.AreEqual(null, testToken.IdToken);
            Assert.AreEqual(16, testToken.JwtAccessToken.Claims.Count());
            Assert.AreEqual(637964561220000000, testToken.JwtAccessToken.ValidTo.Ticks);
            Assert.AreEqual("https://agsync.com", testToken.JwtAccessToken.Issuer);
        }

        [TestMethod]
        public void ParseJsonDocument()
        {
            var refreshToken = "REFRESH_TOKEN";
            var json = $"{{ \"id_token\": \"{OAuth2TokenTests.TokenJwt.RawData}\", \"access_token\": \"{OAuth2TokenTests.Token2Jwt.RawData}\", \"refresh_token\":\"{refreshToken}\" }}";

            var testToken = OAuth2Token.Parse(json);

            // token parsing should not change the refresh token
            Assert.AreEqual(refreshToken, testToken.RefreshToken);
            Assert.AreEqual(16, testToken.JwtIdToken.Claims.Count());
            Assert.AreEqual(637964561220000000, testToken.JwtIdToken.ValidTo.Ticks);
            Assert.AreEqual("https://agsync.com", testToken.JwtIdToken.Issuer);

            //Assert.AreEqual(null, testToken.ClientId);
            Assert.AreEqual("", testToken.ObjectId);
            Assert.AreEqual("", testToken.UserEmail);
        }

        public static JwtSecurityToken TokenJwt => new JwtSecurityToken("eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6IjNNR0xhWFloNmVVa1ZBVUJnTHJKcC1PVWFrSSIsImtpZCI6IjNNR0xhWFloNmVVa1ZBVUJnTHJKcC1PVWFrSSJ9.eyJjbGllbnRfaWQiOiJsYW5kdXNfY29vcGVyYXRpdmUiLCJhY2NvdW50X2ludGVncmF0aW9uX3BhcnRuZXJfaWQiOiI2Iiwic2NvcGUiOlsib3BlbmlkIiwicHJvZmlsZSIsImVtYWlsIiwiYWdzeW5jIiwicm9sZXMiLCJvZmZsaW5lX2FjY2VzcyJdLCJzdWIiOiI5ZWVlMDQ5OS1kNWIwLTRmMDUtOTIxMS1jODc1YTc3OTcwYzIiLCJhbXIiOlsicGFzc3dvcmQiXSwiYXV0aF90aW1lIjoiMTYzNjA2NTM2NyIsImlkcCI6Imlkc3J2IiwiaXNzIjoiaHR0cHM6Ly9hZ3N5bmMuY29tIiwiYXVkIjoiaHR0cHM6Ly9hZ3N5bmMuY29tL3Jlc291cmNlcyIsImV4cCI6MTY2MDg1OTMyMiwibmJmIjoxNjYwODU1NzIyfQ.gnw-8-6cYCuN-LvYLi024Btb41dx3RspDNKaYA_axil7xxPqi6Oy_GE1owZQb5V9M0sIdMd1vBhdB_6Sxi1MzX43fY4qTMU7q4jMCwNFUdbnGkuavg9pWwd5tzN9BMvzNoC2wzHLS4tScJmsjXeo8czjCY6J0r0_AISj8eKYciYRdDal46Bs713C4Ah75ai0bsIAxrk9GrWuwrrI0v57kwuBdKbsZSJlfXc9EHlWuCI8C6m9EwlP_wEydzh-cMXactGzECwguxXXgFmkKgiAI_BOVGNzac3fwmCx6B5NoPGCxYfCxdh6vRz_HJp3MHkI293rYL3ub1o0kbbL5exMLfczCYeJcDid0zoJCluRvRhXOQ-NVXiy5CeyDKQfV10OAOWd2_WssjBZXUrSQKGtxudgK24usHAyU4iw3OhaPF5f_ixT0M3Ke-41c_7AgUvX3OIKUbneRRjslLLrqHoYVIUNCQevx0rBNwqQXVlMwz4fWbMng-GMpKRTaPHPLkqXdNEXYRlXLtQRG64bvvKoVynetE_JDxb5XHHrLfFgDSVOvOl9uBtOdr2M4t0ndxln1Tm7KeEvrI9EzNouxM2iKH-6MPGme5KvuQbkOMODpzl4rJaZdPBrXzB1s06plsJecncEXCTKIEhaQNTtV2DxY-NMKgB_b5PGhmLN4SBYy7Y");

        /// <summary>
        /// Sample Token 2
        /// </summary>
        /// <remarks>
        /// {
        ///   "alg": "HS256",
        ///   "typ": "JWT"
        /// }.{
        ///   "sub": "1234567890",
        ///   "name": "John Doe",
        ///   "admin": true
        /// }.[Signature]
        /// </remarks>
        public static JwtSecurityToken Token2Jwt => new JwtSecurityToken("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiYWRtaW4iOnRydWV9.dyt0CoTl4WoVjAHI9Q_CwSKhl6d_9rhM3NrXuJttkao");

        ///{
        ///  "alg": "RS256",
        ///  "kid": "81DD375130E6AE912F81B69FCE4886710A135D87",
        ///  "x5t": "gd03UTDmrpEvgbafzkiGcQoTXYc",
        ///  "typ": "JWT"
        ///}.{
        ///  "id": "17c1eec1-8e3c-4aea-b147-0c043e8c7d67",
        ///  "ipaddr": "::1",
        ///  "client_id": "10000000-0000-0000-0000-100000000001",
        ///  "user_id": "ff563d9c-f278-4fd3-b51f-3636f358593b",
        ///  "user_email": "chikateanagha@gmail.com",
        ///  "impersonator_id": "61a9a138-d398-4567-ad96-346f9118228d",
        ///  "impersonator_email": "mlanning@gmail.com",
        ///  "impersonator_edit": "true",
        ///  "nbf": 1662395960,
        ///  "exp": 1662399560,
        ///  "iat": 1662395960,
        ///  "iss": "https://localhost:7077/oauth2",
        ///  "aud": "10000000-0000-0000-0000-100000000001"
        ///}.[Signature]
        public static JwtSecurityToken Token3Jwt => new JwtSecurityToken("eyJhbGciOiJSUzI1NiIsImtpZCI6IjgxREQzNzUxMzBFNkFFOTEyRjgxQjY5RkNFNDg4NjcxMEExMzVEODciLCJ4NXQiOiJnZDAzVVREbXJwRXZnYmFmemtpR2NRb1RYWWMiLCJ0eXAiOiJKV1QifQ.eyJpZCI6IjE3YzFlZWMxLThlM2MtNGFlYS1iMTQ3LTBjMDQzZThjN2Q2NyIsImlwYWRkciI6Ijo6MSIsImNsaWVudF9pZCI6IjEwMDAwMDAwLTAwMDAtMDAwMC0wMDAwLTEwMDAwMDAwMDAwMSIsInVzZXJfaWQiOiJmZjU2M2Q5Yy1mMjc4LTRmZDMtYjUxZi0zNjM2ZjM1ODU5M2IiLCJ1c2VyX2VtYWlsIjoiY2hpa2F0ZWFuYWdoYUBnbWFpbC5jb20iLCJpbXBlcnNvbmF0b3JfaWQiOiI2MWE5YTEzOC1kMzk4LTQ1NjctYWQ5Ni0zNDZmOTExODIyOGQiLCJpbXBlcnNvbmF0b3JfZW1haWwiOiJtbGFubmluZ0BnbWFpbC5jb20iLCJpbXBlcnNvbmF0b3JfZWRpdCI6InRydWUiLCJuYmYiOjE2NjIzOTU5NjAsImV4cCI6MTY2MjM5OTU2MCwiaWF0IjoxNjYyMzk1OTYwLCJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo3MDc3L29hdXRoMiIsImF1ZCI6IjEwMDAwMDAwLTAwMDAtMDAwMC0wMDAwLTEwMDAwMDAwMDAwMSJ9.BkQpskdpOsH-4IdRMnFsXF0TxYrDm_ymdStjFaVTiUud7NZ0GSnC-uzEkYONtHVHIMhNR7oy4Jn2K-54viuLsczNKNnczxJMvqK6-veqyAuoHOac7ZQJZoPvlj0tGzjL_LD7Uy4tOGsv1JKqB1cOS1NuhUxJsy2jnif4MQlFLJehCT8n9snMFmEZdJf7FHzobEMuJKFNqwEFsmn6lz2pKgZgKWTBcmUd29hv_d6AGDSpYjyvohsxotJ8uvhdmO2VngWwrERuy9X0PGNdsXvvuvU8_CCVnTuwfCifNT50XiL7iNoLq_zA4heUcTq4rQvEXoMoS_1TV_uNEXwgRr8t4A");


        /// <summary>
        /// Generate Security Token
        /// </summary>
        /// <param name="claims">Claims to Include</param>
        /// <param name="secret">Signing Key</param>
        /// <param name="startOn">Started DateTime</param>
        /// <param name="expiresOn">Expires DateTime</param>
        /// <returns></returns>
        private SecurityToken GenerateToken(IEnumerable<Claim> claims, string secret, DateTime startOn, DateTime expiresOn)
        {              
            var key = Encoding.ASCII.GetBytes(secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = "https://issuer.thingslibrary.io",
                Audience = "audience.thingslibrary.io",
                Subject = new ClaimsIdentity(claims),
                NotBefore = startOn,
                Expires = expiresOn,
                IssuedAt = DateTime.UtcNow,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            //for JWT string call tokenHandler.WriteToken(token)

            var tokenHandler = new JwtSecurityTokenHandler();
            
            return tokenHandler.CreateToken(tokenDescriptor);            
        }

        /// <summary>
        /// Validate the JWT Token
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="secret">Signing Key</param>
        /// <returns></returns>
        private bool ValidateToken(string token, string secret)
        {
            if (token == null) { return false; }

            var key = Encoding.ASCII.GetBytes(secret);
            var tokenHandler = new JwtSecurityTokenHandler();            

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,

                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                //var jwtToken = (JwtSecurityToken)validatedToken;
                
                // return user id from JWT token if validation successful
                return true;
            }
            catch
            {
                // return null if validation fails
                return false;
            }
        }
    }
}
