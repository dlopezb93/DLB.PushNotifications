using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace PushNotifications.APNS.Helpers
{
    public class JwtHelper
    {
        private Dictionary<string, string> _deviceTokens = new Dictionary<string, string>();

        public string GetToken(ECDsa key, string keyID, string teamID, string deviceToken)
        {
            if (!_deviceTokens.TryGetValue(deviceToken, out var token))
            {
                token = CreateToken(key, keyID, teamID);

                _deviceTokens.Add(deviceToken, token);
            }

            return token;
        }

        private string CreateToken(ECDsa key, string keyID, string teamID)
        {
            var securityKey = new ECDsaSecurityKey(key) { KeyId = keyID };
            var credentials = new SigningCredentials(securityKey, "ES256");

            var descriptor = new SecurityTokenDescriptor
            {
                IssuedAt = DateTime.UtcNow,
                Issuer = teamID,
                SigningCredentials = credentials
            };

            var handler = new JwtSecurityTokenHandler();
            var encodedToken = handler.CreateEncodedJwt(descriptor);
            return encodedToken;
        }
    }
}
