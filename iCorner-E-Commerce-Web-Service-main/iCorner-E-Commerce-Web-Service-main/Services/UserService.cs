/****************************************************************************************
 * File: UserService.cs
 * Description: This class provides services for user management, including registration,
 *              login, and user retrieval. It interacts with the UserRepository and 
 *              JwtService for data persistence and JWT token generation.
 ****************************************************************************************/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EAD.Repositories;
using MongoDB.Driver;

namespace EAD.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;
        private readonly JwtService _jwtService;

        public UserService(UserRepository userRepository, JwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        /// <summary>
        /// Registers a new user after checking for existing users with the same email.
        /// </summary>
        /// <param name="newUser">The new user object to register.</param>
        /// <exception cref="Exception">Thrown when the user already exists.</exception>
        public async Task RegisterAsync(User newUser)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(newUser.Email) || string.IsNullOrWhiteSpace(newUser.Password))
            {
                throw new Exception("Email and password are required.");
            }

            var existingUser = await _userRepository.GetUserByEmailAsync(newUser.Email);
            if (existingUser != null) throw new Exception("User already exists");

            newUser.Password = PasswordHasher.HashPassword(newUser.Password);
            await _userRepository.CreateUserAsync(newUser);
        }

        /// <summary>
        /// Authenticates a user by email and password, returning a JWT token if successful.
        /// </summary>
        /// <param name="email">The email of the user attempting to log in.</param>
        /// <param name="password">The password of the user attempting to log in.</param>
        /// <returns>A JWT token if login is successful.</returns>
        /// <exception cref="Exception">Thrown when the credentials are invalid or the account is deactivated.</exception>
        public async Task<string> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null || !PasswordHasher.VerifyPassword(password, user.Password))
                throw new Exception("Invalid credentials");

            if (!user.IsActive)
            {
                throw new Exception("User account is deactivated. Please contact support.");
            }
            return _jwtService.GenerateToken(user.Email, user.Role, user.Username, user.Id);
        }

        /// <summary>
        /// Retrieves a user by their email.
        /// </summary>
        /// <param name="email">The email of the user.</param>
        /// <returns>The user object associated with the email.</returns>
        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetUserByEmailAsync(email);
        }

        /// <summary>
        /// Retrieves a user by their ID.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <returns>The user object associated with the ID.</returns>
        public async Task<User> GetUserByIdAsync(string id)
        {
            return await _userRepository.GetUserByIdAsync(id);
        }

        /// <summary>
        /// Retrieves all admin users.
        /// </summary>
        /// <returns>A list of admin users.</returns>
        public async Task<List<User>> GetAllAdminsAsync()
        {
            return await _userRepository.GetAllAdminsAsync();
        }

        /// <summary>
        /// Retrieves all vendor users.
        /// </summary>
        /// <returns>A list of vendor users.</returns>
        public async Task<List<User>> GetAllVendorsAsync()
        {
            return await _userRepository.GetAllVendorsAsync();
        }

        /// <summary>
        /// Retrieves all CSR users.
        /// </summary>
        /// <returns>A list of CSR users.</returns>
        public async Task<List<User>> GetAllCsrsAsync()
        {
            return await _userRepository.GetAllCsrsAsync();
        }

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="user">The user object containing updated information.</param>
        /// <returns>True if the update was successful, otherwise false.</returns>
        public async Task<bool> UpdateUserAsync(User user)
        {
            return await _userRepository.UpdateUserAsync(user);
        }

        /// <summary>
        /// Deletes a user by their ID.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>True if the deletion was successful, otherwise false.</returns>
        public async Task<bool> DeleteUserAsync(string id)
        {
            return await _userRepository.DeleteUserAsync(id);
        }

        /// <summary>
        /// Activates a user account.
        /// </summary>
        /// <param name="userId">The ID of the user to activate.</param>
        /// <returns>True if the activation was successful, otherwise false.</returns>
        public async Task<bool> ActivateUserAsync(string userId)
        {
            return await _userRepository.ActivateUserAsync(userId);
        }

        /// <summary>
        /// Deactivates a user account.
        /// </summary>
        /// <param name="userId">The ID of the user to deactivate.</param>
        /// <returns>True if the deactivation was successful, otherwise false.</returns>
        public async Task<bool> DeactivateUserAsync(string userId)
        {
            return await _userRepository.DeactivateUserAsync(userId);
        }
    }
}
