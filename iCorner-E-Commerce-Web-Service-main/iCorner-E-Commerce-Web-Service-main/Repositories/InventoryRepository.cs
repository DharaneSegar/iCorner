namespace EAD.Repositories
{
    using EAD.Models;
    using EAD.Services;
    using MongoDB.Driver;
    using System.Threading.Tasks;

    /// <summary>
    /// Repository class for handling inventory-related operations, including stock management and vendor notifications.
    /// </summary>
    public class InventoryRepository : IInventoryRepository
    {
        private readonly IMongoCollection<Product> _products;
        private readonly IVendorNotificationService _notificationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryRepository"/> class.
        /// </summary>
        /// <param name="database">The MongoDB database instance.</param>
        /// <param name="notificationService">The vendor notification service instance.</param>
        public InventoryRepository(IMongoDatabase database, IVendorNotificationService notificationService)
        {
            _products = database.GetCollection<Product>("Products");
            _notificationService = notificationService;
        }

        /// <summary>
        /// Updates the stock quantity of a specific product by adding the given quantity.
        /// </summary>
        /// <param name="productId">The ID of the product to update.</param>
        /// <param name="quantity">The quantity to add to the stock.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UpdateStockAsync(string productId, int quantity)
        {
            var product = await _products.Find(p => p.Id == productId).FirstOrDefaultAsync();
            if (product != null)
            {
                product.StockQuantity += quantity;
                await _products.ReplaceOneAsync(p => p.Id == product.Id, product);
            }
        }

        /// <summary>
        /// Removes a specific quantity from the stock of a given product.
        /// </summary>
        /// <param name="productId">The ID of the product to update.</param>
        /// <param name="quantity">The quantity to remove from the stock.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task RemoveStockAsync(string productId, int quantity)
        {
            var product = await _products.Find(p => p.Id == productId).FirstOrDefaultAsync();

            if (product != null && product.StockQuantity >= quantity)
            {
                bool hasPendingOrders = await CheckPendingOrdersAsync(productId);
                if (hasPendingOrders)
                {
                    throw new InvalidOperationException("Cannot remove stock for products with pending orders.");
                }

                product.StockQuantity -= quantity;
                await _products.ReplaceOneAsync(p => p.Id == product.Id, product);
                await NotifyVendorIfLowStockAsync(productId);
            }
        }

        /// <summary>
        /// Checks if there are any pending orders for the given product.
        /// </summary>
        /// <param name="productId">The ID of the product to check.</param>
        /// <returns>True if there are pending orders; otherwise, false.</returns>
        private async Task<bool> CheckPendingOrdersAsync(string productId)
        {
            // Implement logic to check if there are pending orders for the given product
            // This could involve checking an Orders collection or similar.
            return false; // Placeholder
        }

        /// <summary>
        /// Retrieves the current stock quantity of a specific product.
        /// </summary>
        /// <param name="productId">The ID of the product.</param>
        /// <returns>The current stock quantity of the product.</returns>
        public async Task<int> GetStockQuantityAsync(string productId)
        {
            var product = await _products.Find(p => p.Id == productId).FirstOrDefaultAsync();
            return product?.StockQuantity ?? 0;
        }

        /// <summary>
        /// Notifies the vendor if the stock quantity of a product is below a threshold.
        /// </summary>
        /// <param name="productId">The ID of the product to check and notify about.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task NotifyVendorIfLowStockAsync(string productId)
        {
            var product = await _products.Find(p => p.Id == productId).FirstOrDefaultAsync();
            if (product != null && product.StockQuantity < 10) 
            {
                //TODO: add vendor email
                await _notificationService.NotifyVendorAsync(product.VendorId, productId, product.StockQuantity);
            }
        }
    }
}
