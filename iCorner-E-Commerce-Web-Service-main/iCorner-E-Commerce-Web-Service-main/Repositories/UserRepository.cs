/****************************************************************************************
 * File: UserRepository.cs
 * Description: This file defines the UserRepository class, which provides methods for 
 *              managing user accounts in the application. It includes functionalities to 
 *              create, retrieve, update, and delete user records, as well as to activate 
 *              or deactivate user accounts based on their ID. 
 ****************************************************************************************/

using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using EAD.Models;

namespace EAD.Repositories
{
    public class UserRepository
    {
        private readonly IMongoCollection<User> _users;

        // Constructor that initializes the user collection from the MongoDB database
        public UserRepository(IMongoDatabase database)
        {
            _users = database.GetCollection<User>("Users");
        }

        /// <summary>
        /// Retrieves a user by their email address.
        /// </summary>
        /// <param name="email">The email of the user.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the user if found; otherwise, null.</returns>
        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="user">The user object to create.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task CreateUserAsync(User user)
        {
            await _users.InsertOneAsync(user);
        }

        /// <summary>
        /// Retrieves a user by their ID.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the user if found; otherwise, null.</returns>
        public async Task<User> GetUserByIdAsync(string id)
        {
            return await _users.Find(user => user.Id == id).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves users by their role.
        /// </summary>
        /// <param name="role">The role of the users.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of users with the specified role.</returns>
        public async Task<List<User>> GetUsersByRoleAsync(string role)
        {
            return await _users.Find(user => user.Role == role).ToListAsync();
        }

        /// <summary>
        /// Retrieves all admin users.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of admin users.</returns>
        public async Task<List<User>> GetAllAdminsAsync()
        {
            return await GetUsersByRoleAsync("Admin");
        }

        /// <summary>
        /// Retrieves all vendor users.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of vendor users.</returns>
        public async Task<List<User>> GetAllVendorsAsync()
        {
            return await GetUsersByRoleAsync("Vendor");
        }

        /// <summary>
        /// Retrieves all CSR users.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of CSR users.</returns>
        public async Task<List<User>> GetAllCsrsAsync()
        {
            return await GetUsersByRoleAsync("CSR");
        }

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="user">The user object with updated information.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the update was successful.</returns>
        public async Task<bool> UpdateUserAsync(User user)
        {
            var result = await _users.ReplaceOneAsync(u => u.Id == user.Id, user);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        /// <summary>
        /// Deletes a user by their ID.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the deletion was successful.</returns>
        public async Task<bool> DeleteUserAsync(string id)
        {
            var result = await _users.DeleteOneAsync(u => u.Id == id);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        /// <summary>
        /// Activates a user account.
        /// </summary>
        /// <param name="userId">The ID of the user to activate.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the activation was successful.</returns>
        public async Task<bool> ActivateUserAsync(string userId)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
            var update = Builders<User>.Update.Set(u => u.IsActive, true);
            var result = await _users.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        /// <summary>
        /// Deactivates a user account.
        /// </summary>
        /// <param name="userId">The ID of the user to deactivate.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the deactivation was successful.</returns>
        public async Task<bool> DeactivateUserAsync(string userId)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
            var update = Builders<User>.Update.Set(u => u.IsActive, false);
            var result = await _users.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }
    }
}
