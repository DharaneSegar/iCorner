using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace EAD.Services
{
    /// <summary>
    /// This service is responsible for generating JSON Web Tokens (JWT) for authenticated users.
    /// It creates tokens containing user-specific claims such as email, role, username, and user ID,
    /// and signs them using a symmetric security key.
    /// </summary>
    /// <remarks>
    /// The tokens generated are valid for a duration of 1 hour by default and are signed using HMAC-SHA256.
    /// This service can be integrated into user authentication mechanisms to issue tokens that can be
    /// validated on subsequent API requests.
    /// </remarks>
    public class JwtService
    {
        private readonly string _secretKey;

        /// <summary>
        /// Initializes the JwtService with a secret key used for signing the token.
        /// </summary>
        /// <param name="secretKey">A string containing the secret key for signing the JWT.</param>
        public JwtService(string secretKey) // Accept string directly
        {
            _secretKey = secretKey;
        }

        /// <summary>
        /// Generates a JWT token with claims for the authenticated user.
        /// </summary>
        /// <param name="email">The email of the user.</param>
        /// <param name="role">The role of the user (e.g., Admin, Vendor, CSR).</param>
        /// <param name="username">The username of the user.</param>
        /// <param name="userId">The unique identifier for the user.</param>
        /// <returns>A signed JWT token string.</returns>
        public string GenerateToken(string email, string role, string username, string userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            // Use simple claim names like "email", "role", "username", and "id"
            var claims = new List<Claim>
            {
                new Claim("email", email),
                new Claim("role", role),
                new Claim("username", username),
                new Claim("id", userId)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1), // Token expiration time
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
