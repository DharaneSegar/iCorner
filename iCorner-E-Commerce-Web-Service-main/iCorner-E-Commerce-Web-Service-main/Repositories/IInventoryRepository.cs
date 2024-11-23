namespace EAD.Repositories
{
    /// <summary>
    /// Interface for the Inventory Repository, defining operations for managing product stock levels.
    /// </summary>
    public interface IInventoryRepository
    {
        /// <summary>
        /// Updates the stock quantity of a specific product by adding the given quantity.
        /// </summary>
        /// <param name="productId">The ID of the product to update.</param>
        /// <param name="quantity">The quantity to add to the stock.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task UpdateStockAsync(string productId, int quantity);

        /// <summary>
        /// Removes a specific quantity from the stock of a given product.
        /// </summary>
        /// <param name="productId">The ID of the product to update.</param>
        /// <param name="quantity">The quantity to remove from the stock.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task RemoveStockAsync(string productId, int quantity);

        /// <summary>
        /// Retrieves the current stock quantity of a specific product.
        /// </summary>
        /// <param name="productId">The ID of the product.</param>
        /// <returns>The current stock quantity of the product.</returns>
        Task<int> GetStockQuantityAsync(string productId);
    }
}
