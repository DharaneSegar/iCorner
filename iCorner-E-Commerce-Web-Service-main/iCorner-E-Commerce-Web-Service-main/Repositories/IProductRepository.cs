namespace EAD.Repositories
{
    using EAD.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for the Product Repository, defining the operations for managing products.
    /// </summary>
    public interface IProductRepository
    {
        /// <summary>
        /// Retrieves all products from the database.
        /// </summary>
        /// <returns>An enumerable collection of all products.</returns>
        Task<IEnumerable<Product>> GetAllProductsAsync();

        /// <summary>
        /// Retrieves a single product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to retrieve.</param>
        /// <returns>The product with the specified ID, or null if not found.</returns>
        Task<Product> GetProductByIdAsync(string id);

        /// <summary>
        /// Adds a new product to the database.
        /// </summary>
        /// <param name="product">The product to add.</param>
        Task AddProductAsync(Product product);

        /// <summary>
        /// Updates an existing product in the database.
        /// </summary>
        /// <param name="product">The product to update.</param>
        Task UpdateProductAsync(Product product);

        /// <summary>
        /// Deletes a product from the database by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to delete.</param>
        Task DeleteProductAsync(string id);

        /// <summary>
        /// Deactivates a product by setting its IsActive status to false.
        /// </summary>
        /// <param name="id">The ID of the product to deactivate.</param>
        Task DeactivateProductAsync(string id);

        /// <summary>
        /// Activates a product by setting its IsActive status to true.
        /// </summary>
        /// <param name="id">The ID of the product to activate.</param>
        Task ActivateProductAsync(string id);

        /// <summary>
        /// Retrieves all products belonging to a specific category.
        /// </summary>
        /// <param name="id">The ID of the category.</param>
        /// <returns>An enumerable collection of products in the specified category.</returns>
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(string id);

        /// <summary>
        /// Retrieves all products with stock quantity greater than 0.
        /// </summary>
        /// <returns>An enumerable collection of available products.</returns>
        Task<IEnumerable<Product>> GetAvailableProductsAsync();

    }
}
