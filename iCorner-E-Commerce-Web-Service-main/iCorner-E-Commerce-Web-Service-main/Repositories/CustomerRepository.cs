// ------------------------------------------------------------------------------------
// File: CustomerRepository.cs
// Namespace: EAD.Repositories
// Description: This class handles all CRUD operations related to the "Customer" entity 
//              in the MongoDB database. It also includes functionality for customer 
//              registration, account activation, and login handling.
// ------------------------------------------------------------------------------------

namespace EAD.Repositories
{
    using EAD.Models;
    using EAD.Services;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using BCrypt.Net;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class CustomerRepository : ICustomerRepository
    {
        private readonly IMongoCollection<Customer> _customers;
        private readonly ICustomerNotificationService _customerNotificationService;

        public CustomerRepository(IMongoDatabase database, ICustomerNotificationService customerNotificationService)
        {
            _customers = database.GetCollection<Customer>("Customers");
            _customerNotificationService = customerNotificationService; // Injected service
        }

        // --------------------------------------------------------------------------------
        // Method: GetAllCustomersAsync
        // Purpose: Retrieves all customer records from the database.
        // Returns: A list of all customers as IEnumerable<Customer>.
        // --------------------------------------------------------------------------------
        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            return await _customers.Find(_ => true).ToListAsync();
        }

        // --------------------------------------------------------------------------------
        // Method: GetCustomerByEmailAsync
        // Purpose: Retrieves a single customer record by email.
        // Parameters: 
        //      - email: string (The customer's email).
        // Returns: A Customer object or null if not found.
        // --------------------------------------------------------------------------------
        public async Task<Customer> GetCustomerByEmailAsync(string email)
        {
            return await _customers.Find(c => c.Email == email).FirstOrDefaultAsync();
        }

        // --------------------------------------------------------------------------------
        // Method: RegisterCustomerAsync
        // Purpose: Registers a new customer by inserting a new customer document.
        // Parameters:
        //      - customer: Customer (The customer object to be registered).
        // Actions: Hashes the customer's password and sets the account to inactive.
        // --------------------------------------------------------------------------------
        public async Task RegisterCustomerAsync(Customer customer)
        {
            if (string.IsNullOrEmpty(customer.Id))
            {
                customer.Id = ObjectId.GenerateNewId().ToString(); // Generate new Id if not set
            }

            // Hash the password before saving
            customer.Password = BCrypt.HashPassword(customer.Password);

            // Set IsActive to false by default
            customer.IsActive = false;

            await _customers.InsertOneAsync(customer);
        }

        // --------------------------------------------------------------------------------
        // Method: UpdateCustomerAsync
        // Purpose: Updates an existing customer record in the database.
        // Parameters: 
        //      - id: string (Customer's Id).
        //      - customer: Customer (Updated customer object).
        // Returns: The updated Customer object.
        // Throws: Exception if no matching customer is found.
        // --------------------------------------------------------------------------------
        public async Task<Customer> UpdateCustomerAsync(string id, Customer customer)
        {
            // Ensure that the ID in the customer object is not null or altered
            if (customer.Id != id)
            {
                customer.Id = id; // Set the Id to ensure it remains the same
            }

            // Update customer in the database
            var result = await _customers.ReplaceOneAsync(c => c.Id == id, customer);
            if (result.MatchedCount == 0)
            {
                throw new Exception("Customer not found for update.");
            }

            return customer;
        }

        // --------------------------------------------------------------------------------
        // Method: DeleteCustomerAsync
        // Purpose: Deactivates the customer account by removing the customer record.
        // Parameters: 
        //      - email: string (The customer's email to be deleted).
        // --------------------------------------------------------------------------------
        public async Task DeleteCustomerAsync(string email)
        {
            // Deletes the customer from the database using the email
            await _customers.DeleteOneAsync(c => c.Email == email);
        }

        // --------------------------------------------------------------------------------
        // Method: ActivateCustomerAsync
        // Purpose: Activates a customer account. 
        // Parameters: 
        //      - email: string (The customer's email whose account is to be activated).
        // --------------------------------------------------------------------------------
        public async Task ActivateCustomerAsync(string email)
        {
            var update = Builders<Customer>.Update.Set(c => c.IsActive, true);
            await _customers.UpdateOneAsync(c => c.Email == email, update);

            // After activating the customer, send an activation email
            await _customerNotificationService.NotifyCustomerActivationAsync(email);
        }

        // Deactivate customer account (only CSR/Admin)
        public async Task DeactivateCustomerAsync(string email)
        {
            var update = Builders<Customer>.Update.Set(c => c.IsActive, false);
            await _customers.UpdateOneAsync(c => c.Email == email, update);

            // After activating the customer, send an activation email
            await _customerNotificationService.NotifyCustomerDeactivationAsync(email);
        }


        // --------------------------------------------------------------------------------
        // Method: ReactivateCustomerAsync
        // Purpose: Reactivates a customer account that was previously deactivated.
        // Parameters: 
        //      - email: string (The customer's email whose account is to be reactivated).
        // --------------------------------------------------------------------------------
        public async Task ReactivateCustomerAsync(string email)
        {
            var update = Builders<Customer>.Update.Set(c => c.IsActive, true);
            await _customers.UpdateOneAsync(c => c.Email == email, update);
        }

        // --------------------------------------------------------------------------------
        // Enum: LoginResult
        // Purpose: Defines the possible outcomes of the login process.
        // --------------------------------------------------------------------------------
        public enum LoginResult
        {
            Success,
            InvalidEmail,
            IncorrectPassword,
            AccountDeactivated
        }

        // --------------------------------------------------------------------------------
        // Method: LoginCustomerAsync
        // Purpose: Handles customer login, verifying email and password, and checking account status.
        // Parameters: 
        //      - email: string (The customer's email).
        //      - password: string (The provided password).
        // Returns: LoginResult indicating the result of the login attempt.
        // --------------------------------------------------------------------------------
        public async Task<LoginResult> LoginCustomerAsync(string email, string password)
        {
            var customer = await _customers.Find(c => c.Email == email).FirstOrDefaultAsync();

            if (customer == null)
            {
                return LoginResult.InvalidEmail; // Email does not exist
            }

            if (!customer.IsActive)
            {
                return LoginResult.AccountDeactivated; // Account is deactivated
            }

            if (!BCrypt.Verify(password, customer.Password))
            {
                return LoginResult.IncorrectPassword; // Incorrect password
            }

            return LoginResult.Success; // Login successful
        }

    }
}
