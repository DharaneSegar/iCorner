// ------------------------------------------------------------------------------------
// File: ICustomerRepository.cs
// Namespace: EAD.Repositories
// Description: This interface defines the contract for the customer repository, 
//              providing methods to manage customer data, including registration, 
//              updating, deletion, activation, reactivation, and login functionality. 
//              The repository is responsible for handling customer-related operations 
//              such as retrieving customers by email and managing login functionality.
// ------------------------------------------------------------------------------------

namespace EAD.Repositories
{
    using EAD.Models;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using static EAD.Repositories.CustomerRepository;

    public interface ICustomerRepository
    {
        // --------------------------------------------------------------------------------
        // Method: GetAllCustomersAsync
        // Purpose: Retrieves all registered customers asynchronously.
        // Returns: A Task<IEnumerable<Customer>> representing the list of customers.
        // --------------------------------------------------------------------------------
        Task<IEnumerable<Customer>> GetAllCustomersAsync();

        // --------------------------------------------------------------------------------
        // Method: GetCustomerByEmailAsync
        // Purpose: Retrieves a customer by their unique email address.
        // Parameters: 
        //      - email: string (The customer's email address).
        // Returns: A Task<Customer> object representing the customer.
        // --------------------------------------------------------------------------------
        Task<Customer> GetCustomerByEmailAsync(string email);

        // --------------------------------------------------------------------------------
        // Method: RegisterCustomerAsync
        // Purpose: Registers a new customer in the system asynchronously.
        // Parameters: 
        //      - customer: Customer (The customer object containing registration details).
        // --------------------------------------------------------------------------------
        Task RegisterCustomerAsync(Customer customer);

        // --------------------------------------------------------------------------------
        // Method: UpdateCustomerAsync
        // Purpose: Updates the information of an existing customer based on their ID.
        // Parameters: 
        //      - id: string (The customer's unique ID).
        //      - customer: Customer (The updated customer details).
        // Returns: A Task<Customer> object representing the updated customer.
        // --------------------------------------------------------------------------------
        Task<Customer> UpdateCustomerAsync(string id, Customer customer);

        // --------------------------------------------------------------------------------
        // Method: DeleteCustomerAsync
        // Purpose: Deletes a customer from the system based on their email address.
        // Parameters: 
        //      - email: string (The customer's email address).
        // --------------------------------------------------------------------------------
        Task DeleteCustomerAsync(string email);

        Task ActivateCustomerAsync(string email);

        Task DeactivateCustomerAsync(string email);

        // --------------------------------------------------------------------------------
        // Method: ReactivateCustomerAsync
        // Purpose: Reactivates a previously deactivated customer account by their email.
        // Parameters: 
        //      - email: string (The customer's email address).
        // --------------------------------------------------------------------------------
        Task ReactivateCustomerAsync(string email);

        // --------------------------------------------------------------------------------
        // Method: LoginCustomerAsync
        // Purpose: Verifies a customer's login credentials and returns the result.
        // Parameters: 
        //      - email: string (The customer's email address).
        //      - password: string (The customer's password).
        // Returns: A Task<LoginResult> representing the result of the login attempt, 
        //          such as success or failure.
        // --------------------------------------------------------------------------------
        Task<LoginResult> LoginCustomerAsync(string email, string password);
    }
}
