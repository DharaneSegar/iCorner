/****************************************************************************************
 * File: PasswordHasher.cs
 * Description: This static class provides methods for securely hashing and verifying
 *              passwords. It utilizes PBKDF2 with HMACSHA256 to generate hashed passwords
 *              with a salt for added security.
 ****************************************************************************************/

using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace EAD.Services
{
    public static class PasswordHasher
    {
        /// <summary>
        /// Hashes a password using PBKDF2 with HMACSHA256 and a randomly generated salt.
        /// </summary>
        /// <param name="password">The plain text password to hash.</param>
        /// <returns>A string containing the base64 encoded salt and the hashed password.</returns>
        public static string HashPassword(string password)
        {
            // Generate a 16-byte salt
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt); // Fill the salt array with random bytes
            }

            // Hash the password with PBKDF2
            var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000, // Use 10,000 iterations for hashing
                numBytesRequested: 32)); // Request a 32-byte hash

            // Combine salt and hashed password in a single string
            return $"{Convert.ToBase64String(salt)}.{hashed}";
        }

        /// <summary>
        /// Verifies a password against a previously hashed password.
        /// </summary>
        /// <param name="password">The plain text password to verify.</param>
        /// <param name="hashedPassword">The previously hashed password to verify against.</param>
        /// <returns>True if the password matches, otherwise false.</returns>
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            // Split the stored hash to get the salt and the stored hash value
            var parts = hashedPassword.Split('.');
            var salt = Convert.FromBase64String(parts[0]); // Decode the salt
            var storedHash = parts[1]; // The stored hash value

            // Hash the provided password with the extracted salt
            var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 32));

            // Compare the computed hash with the stored hash
            return hashed == storedHash;
        }
    }
}
