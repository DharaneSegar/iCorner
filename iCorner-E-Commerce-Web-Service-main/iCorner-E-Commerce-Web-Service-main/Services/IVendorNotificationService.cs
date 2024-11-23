namespace EAD.Services
{
    /// <summary>
    /// Interface for a service that handles notifications to vendors.
    /// </summary>
    public interface IVendorNotificationService
    {
        /// <summary>
        /// Sends a notification to the vendor about the stock status of a product.
        /// </summary>
        /// <param name="vendorId">The ID of the vendor to notify.</param>
        /// <param name="productId">The ID of the product with updated stock.</param>
        /// <param name="stockQuantity">The current stock quantity of the product.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task NotifyVendorAsync(string vendorId, string productId, int stockQuantity);
    }
}
