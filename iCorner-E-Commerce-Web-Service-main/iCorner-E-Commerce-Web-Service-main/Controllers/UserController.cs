/****************************************************************************************
 * File: UserController.cs
 * Description: This file contains the implementation of the UserController, which handles
 *              user-related operations like registration, login, fetching user details, 
 *              updating user profiles, and deleting users. It integrates with the UserService 
 *              to perform these operations and ensures proper authorization for certain actions.
 ****************************************************************************************/

using System;
using System.Security.Claims;
using EAD.Models;
using EAD.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EAD.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IUserNotificationService _userEmailService;

        // Constructor to inject the UserService for user-related operations.
        public UserController(UserService userService, IUserNotificationService userEmailService)
        {
            _userService = userService;
            _userEmailService = userEmailService;
        }

        // POST: api/user
        // This method registers a new user. It validates the incoming user data and 
        // checks if the role is either Admin, Vendor, or CSR. If validation passes, 
        // it attempts to register the user using the UserService.
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] User newUser)
        {
            // Validate that the user object is not null and that required fields are not empty
            if (newUser == null ||
                string.IsNullOrEmpty(newUser.Username) ||
                string.IsNullOrEmpty(newUser.Email) ||
                string.IsNullOrEmpty(newUser.Password) ||
                string.IsNullOrEmpty(newUser.MobileNumber) ||
                string.IsNullOrEmpty(newUser.Address) ||
                string.IsNullOrEmpty(newUser.Role)
            )
            {
                return BadRequest("Invalid user data."); // Return bad request if validation fails
            }

            // Validate that the role is either Admin, Vendor, or CSR
            if (newUser.Role != "Admin" && newUser.Role != "Vendor" && newUser.Role != "CSR")
            {
                return BadRequest("Invalid role. Must be Admin, Vendor, or CSR.");
            }

            try
            {
                var originalPassword = newUser.Password;
                // Register the user asynchronously
                await _userService.RegisterAsync(newUser);
                // Create a dummy user object to send in the email
                var dummyUser = new User
                {
                    Username = newUser.Username,
                    Email = newUser.Email,
                    Password = originalPassword,
                    MobileNumber = newUser.MobileNumber,
                    Address = newUser.Address,
                    Role = newUser.Role
                };
                await _userEmailService.SendRegistrationEmailAsync(dummyUser);

                return Ok(new { Message = "Registration successful. Please check your email for login details." });
            }
            catch (Exception ex)
            {
                // Return a bad request with the exception message in case of errors
                return BadRequest(ex.Message);
            }
        }

        // POST: api/user/login
        // This method handles user login by validating the email and password provided 
        // in the request body. It uses the UserService to authenticate the user and returns 
        // a JWT token if successful.
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                // Authenticate the user using the email and password
                var user = await _userService.LoginAsync(model.Email, model.Password);
                return Ok(new { Token = user }); // Return JWT token if authentication is successful
            }
            catch (Exception ex)
            {
                // Return a bad request with the exception message in case of errors
                return BadRequest(new { Message = ex.Message });
            }
        }

        // Helper method to get the logged-in user's email from the claims.
        private string GetLoggedInUserEmail()
        {
            return User.FindFirstValue(ClaimTypes.Email);
        }

        // GET: api/user/{id}
        // This method retrieves a user's details by their ID. It is an authorized action, 
        // meaning only authenticated users can access this endpoint.
        [Authorize]
        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            // Fetch the user by ID asynchronously
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound("User not found"); // Return NotFound if the user doesn't exist

            return Ok(user); // Return the user details if found
        }

        // PUT: api/user/update/{id}
        // This method allows the authenticated user to update their profile details. 
        // It retrieves the logged-in user's email and updates their information.
        [Authorize]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] User updatedUser)
        {
            var userEmail = GetLoggedInUserEmail();
            if (userEmail == null) return Unauthorized("You must be logged in to update your profile."); // Ensure user is logged in

            // Fetch the user by ID to check if they exist
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound("User not found"); // Return NotFound if user doesn't exist

            // Update the user's profile details
            user.Username = updatedUser.Username;
            user.MobileNumber = updatedUser.MobileNumber;
            user.Address = updatedUser.Address;

            // Hash the password if provided
            if (!string.IsNullOrWhiteSpace(updatedUser.Password))
            {
                user.Password = PasswordHasher.HashPassword(updatedUser.Password);
            }

            // Attempt to update the user and check for success
            var result = await _userService.UpdateUserAsync(user);
            if (!result) return BadRequest("Failed to update user");

            return Ok("User updated successfully"); // Return success message
        }

        // DELETE: api/user/delete/{id}
        // This method allows the authenticated user to delete their profile.
        // It checks if the user is logged in and deletes the user account if found.
        [Authorize]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var userEmail = GetLoggedInUserEmail();
            if (userEmail == null) return Unauthorized("You must be logged in to delete your profile."); // Ensure user is logged in

            // Fetch the user by ID to check if they exist
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound("User not found"); // Return NotFound if user doesn't exist

            // Attempt to delete the user and check for success
            var result = await _userService.DeleteUserAsync(id);
            if (!result) return BadRequest("Failed to delete user");

            return Ok("User deleted successfully"); // Return success message
        }
    }
}
