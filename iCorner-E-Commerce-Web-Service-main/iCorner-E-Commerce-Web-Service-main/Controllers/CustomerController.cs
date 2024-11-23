// -----------------------------------------------------------------------
// <summary>
// This class represents the CustomerController, which manages customer-related operations
// such as registration, retrieval, updates, activation, deletion, and login.
// It provides endpoints to interact with customer data through the API.
// </summary>
// <remarks>
// This controller relies on the ICustomerRepository interface for data access operations.
// </remarks>
// -----------------------------------------------------------------------

using EAD.Models;
using EAD.Repositories;
using EAD.Services;
using Microsoft.AspNetCore.Mvc;
using static EAD.Repositories.CustomerRepository;

namespace EAD.Controller;

/// <summary>
/// Controller for managing customer-related operations.
/// Provides endpoints for customer registration, retrieval, updates, activation, deletion, and login.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class CustomerController : ControllerBase
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICsrNotificationService _csrNotificationService;
    private readonly UserRepository _userRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerController"/> class.
    /// </summary>
    /// <param name="customerRepository">The customer repository instance for accessing customer data.</param>
    public CustomerController(ICustomerRepository customerRepository, ICsrNotificationService csrNotificationService, UserRepository userRepository)
    {
        _customerRepository = customerRepository;
        _csrNotificationService = csrNotificationService;
        _userRepository = userRepository;
    }

    /// <summary>
    /// Retrieves all customers from the database.
    /// </summary>
    /// <returns>An IActionResult containing the list of customers.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllCustomers()
    {
        var customers = await _customerRepository.GetAllCustomersAsync();
        return Ok(customers);
    }

    /// <summary>
    /// Registers a new customer.
    /// Validates the customer information and checks for existing accounts before registering.
    /// </summary>
    /// <param name="customer">The customer object containing registration details.</param>
    /// <returns>An IActionResult indicating the result of the registration attempt.</returns>
    [HttpPost("register")]
    public async Task<IActionResult> RegisterCustomer([FromBody] Customer customer)
    {
        if (customer == null || string.IsNullOrEmpty(customer.Email) || string.IsNullOrEmpty(customer.Password))
        {
            return BadRequest(new { message = "Email and password are required." });
        }

        var existingCustomer = await _customerRepository.GetCustomerByEmailAsync(customer.Email);
        if (existingCustomer != null)
        {
            return Conflict(new { message = "Customer with this email already exists." });
        }

        try
        {
            await _customerRepository.RegisterCustomerAsync(customer);
            // Get all registered CSRs
            List<User> csrs = await _userRepository.GetAllCsrsAsync();
            List<string> csrEmails = csrs.Select(csr => csr.Email).ToList();

            // Notify all CSRs about the new customer registration
            await _csrNotificationService.NotifyCsrsAboutNewCustomerAsync(customer.Email, csrEmails);
            return CreatedAtAction(nameof(GetCustomerByEmail), new { email = customer.Email }, customer);
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, new { message = "An unexpected error occurred during registration." });
        }
    }

    /// <summary>
    /// Retrieves a customer by their email address.
    /// </summary>
    /// <param name="email">The email address of the customer to retrieve.</param>
    /// <returns>An IActionResult containing the customer details or a NotFound result.</returns>
    [HttpGet("{email}")]
    public async Task<IActionResult> GetCustomerByEmail(string email)
    {
        var customer = await _customerRepository.GetCustomerByEmailAsync(email);
        if (customer == null) return NotFound();
        return Ok(customer);
    }

    /// <summary>
    /// Updates an existing customer's information.
    /// Requires the customer ID and the updated customer object.
    /// </summary>
    /// <param name="id">The ID of the customer to update.</param>
    /// <param name="customer">The customer object containing updated details.</param>
    /// <returns>An IActionResult containing the updated customer details.</returns>
    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateCustomer(string id, [FromBody] Customer customer)
    {
        if (customer == null || string.IsNullOrEmpty(id))
        {
            return BadRequest("Customer ID is required.");
        }

        // Ensure the ID is included in the customer object
        customer.Id = id;

        var updatedCustomer = await _customerRepository.UpdateCustomerAsync(id, customer);
        return Ok(updatedCustomer);
    }

    /// <summary>
    /// Activates a deactivated customer account by email.
    /// </summary>
    /// <param name="email">The email address of the customer to activate.</param>
    /// <returns>An IActionResult indicating the activation status.</returns>
    [HttpPut("activate/{email}")]
    public async Task<IActionResult> ActivateCustomer(string email)
    {
        var customer = await _customerRepository.GetCustomerByEmailAsync(email);
        if (customer == null)
        {
            return NotFound();
        }
        if (customer.IsActive)
        {
            return BadRequest(new { Email = email, Status = "Already Activated" }); // Account is already activated
        }

        await _customerRepository.ActivateCustomerAsync(email);
        return Ok(new { Email = email, Status = "Activated" });
    }

    [HttpPut("deactivate/{email}")]
    public async Task<IActionResult> DeactivateCustomer(string email)
    {
        var customer = await _customerRepository.GetCustomerByEmailAsync(email);
        if (customer == null)
        {
            return NotFound();
        }
        if (customer.IsActive == false)
        {
            return BadRequest(new { Email = email, Status = "Already Deactivated" }); // Account is already activated
        }

        await _customerRepository.DeactivateCustomerAsync(email);
        return Ok(new { Email = email, Status = "Deactivated" });
    }

    [HttpDelete("delete/{email}")]
    public async Task<IActionResult> DeleteCustomer(string email)
    {
        // Check if the customer exists
        var customer = await _customerRepository.GetCustomerByEmailAsync(email);
        if (customer == null)
        {
            return NotFound(new { Status = "Error", Message = "Customer not found" });
        }

        // Proceed to delete the customer
        await _customerRepository.DeleteCustomerAsync(email);
        return Ok(new { Status = "Success", Message = "Customer deleted", Email = email });
    }

    /// <summary>
    /// Authenticates a customer using email and password.
    /// Returns customer details upon successful login.
    /// </summary>
    /// <param name="loginRequest">The login request object containing email and password.</param>
    /// <returns>An IActionResult indicating the login result.</returns>
    [HttpPost("login")]
    public async Task<IActionResult> LoginCustomer([FromBody] LoginRequest loginRequest)
    {
        if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Email) || string.IsNullOrEmpty(loginRequest.Password))
        {
            return BadRequest("Email and password are required.");
        }

        var loginResult = await _customerRepository.LoginCustomerAsync(loginRequest.Email, loginRequest.Password);

        switch (loginResult)
        {
            case LoginResult.Success:
                // Retrieve the customer details after successful login
                var customer = await _customerRepository.GetCustomerByEmailAsync(loginRequest.Email);
                return Ok(new
                {
                    Message = "Login successful.",
                    Customer = customer
                });
            case LoginResult.InvalidEmail:
                return NotFound(new { Message = "Invalid email address." });
            case LoginResult.IncorrectPassword:
                return Unauthorized(new { Message = "Incorrect password." });
            case LoginResult.AccountDeactivated:
                return Unauthorized(new { Message = "Account is deactivated." });
            default:
                return StatusCode(500, "An unexpected error occurred.");
        }
    }
}
