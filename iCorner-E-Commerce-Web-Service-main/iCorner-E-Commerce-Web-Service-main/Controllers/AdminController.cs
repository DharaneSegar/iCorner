/****************************************************************************************
 * File: AdminController.cs
 * Description: This file contains the implementation of the AdminController, which handles
 *              administrative functionalities such as retrieving user roles (Admins, Vendors,
 *              CSRs), activating and deactivating user accounts. Access is restricted to
 *              users with the "Admin" role.
 ****************************************************************************************/

using System;
using EAD.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EAD.Controllers
{
    // The AdminController is responsible for managing admin-specific operations
    // such as retrieving admins, vendors, and customer service representatives (CSRs),
    // and activating or deactivating user accounts. Only users with the "Admin" role
    // are authorized to access these endpoints.
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Restricts access to users with Admin role
    public class AdminController : ControllerBase
    {
        // Dependency injection for the UserService, which handles user-related data operations
        private readonly UserService _userService;
        private readonly IUserNotificationService _userEmailService;

        // Constructor for AdminController to inject UserService
        public AdminController(UserService userService, IUserNotificationService userEmailService)
        {
            _userService = userService;
            _userEmailService = userEmailService;
        }

        // Inline comments at the beginning of each method.

        // Method to get all admins
        // GET: api/admin/admins
        // This method retrieves all users who have the "Admin" role by calling the GetAllAdminsAsync
        // method of the injected UserService and returns the list of admins as a JSON response.
        [HttpGet("admins")]
        public async Task<IActionResult> GetAllAdmins()
        {
            var admins = await _userService.GetAllAdminsAsync(); // Calls service to get all admins
            return Ok(admins); // Returns the list of admins in the response
        }

        // Method to get all vendors
        // GET: api/admin/vendors
        // This method retrieves all users who have the "Vendor" role by calling the GetAllVendorsAsync
        // method of the injected UserService and returns the list of vendors as a JSON response.
        [HttpGet("vendors")]
        public async Task<IActionResult> GetAllVendors()
        {
            var vendors = await _userService.GetAllVendorsAsync(); // Calls service to get all vendors
            return Ok(vendors); // Returns the list of vendors in the response
        }

        // Method to get all CSRs (Customer Service Representatives)
        // GET: api/admin/csrs
        // This method retrieves all users who have the "CSR" role by calling the GetAllCsrsAsync
        // method of the injected UserService and returns the list of CSRs as a JSON response.
        [HttpGet("csrs")]
        public async Task<IActionResult> GetAllCsrs()
        {
            var csrs = await _userService.GetAllCsrsAsync(); // Calls service to get all CSRs
            return Ok(csrs); // Returns the list of CSRs in the response
        }

        // Method to activate a user account
        // PATCH: api/admin/activate/{userId}
        // This method activates a user account based on the userId provided in the route.
        // It calls the ActivateUserAsync method of the UserService and returns a success
        // or failure message depending on the outcome.
        [HttpPatch("activate/{userId}")]
        public async Task<IActionResult> ActivateUser(string userId)
        {
            var result = await _userService.ActivateUserAsync(userId); // Calls service to activate the user
            if (result)
            {
                // Notify the user via email upon successful activation
                await _userEmailService.SendAccountActivationEmailAsync(userId);
                return Ok(new { message = "User account activated successfully" });
            }
            return NotFound(new { message = "User not found" }); // Error response if user not found
        }

        // Method to deactivate a user account
        // PATCH: api/admin/deactivate/{userId}
        // This method deactivates a user account based on the userId provided in the route.
        // It calls the DeactivateUserAsync method of the UserService and returns a success
        // or failure message depending on the outcome.
        [HttpPatch("deactivate/{userId}")]
        public async Task<IActionResult> DeactivateUser(string userId)
        {
            var result = await _userService.DeactivateUserAsync(userId); // Calls service to deactivate the user
            if (result)
            {
                // Notify the user via email upon successful deactivation
                await _userEmailService.SendAccountDeactivationEmailAsync(userId);
                return Ok(new { message = "User account deactivated successfully" });
            }
            return NotFound(new { message = "User not found" }); // Error response if user not found
        }
    }
}
