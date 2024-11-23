using EAD.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EAD.Controllers
{
    // Route configuration for the Inventory API controller
    [ApiController]
    [Route("api/inventory")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryRepository _inventoryRepository;

        // Constructor that initializes the inventory repository
        public InventoryController(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        public class UpdateStockRequest
        {
            public int Stock { get; set; }
        }

        /// <summary>
        /// Updates the stock quantity for a given product ID.
        /// </summary>
        /// <param name="productId">The ID of the product to update.</param>
        /// <param name="request">The request containing the new stock quantity.</param>
        /// <returns>An action result indicating the outcome of the operation.</returns>
        [HttpPut("update-stock/{productId}")]
        public async Task<IActionResult> UpdateStock(string productId, [FromBody] UpdateStockRequest request)
        {
            try
            {
                await _inventoryRepository.UpdateStockAsync(productId, request.Stock);
                return Ok(new { Message = "Stock updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = $"Error updating stock: {ex.Message}" });
            }
        }

        /// <summary>
        /// Removes a specified quantity of stock for a given product ID.
        /// </summary>
        /// <param name="productId">The ID of the product to remove stock from.</param>
        /// <param name="request">The request containing the stock quantity to remove.</param>
        /// <returns>An action result indicating the outcome of the operation.</returns>
        [HttpPut("remove-stock/{productId}")]
        public async Task<IActionResult> RemoveStock(string productId, [FromBody] UpdateStockRequest request)
        {
            try
            {
                await _inventoryRepository.RemoveStockAsync(productId, request.Stock);
                return Ok(new { Message = "Stock removed successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = $"Error removing stock: {ex.Message}" });
            }
        }

        /// <summary>
        /// Retrieves the current stock quantity for a given product ID.
        /// </summary>
        /// <param name="productId">The ID of the product to check.</param>
        /// <returns>An action result containing the stock quantity.</returns>
        [HttpGet("get-stock/{productId}")]
        public async Task<IActionResult> GetStockQuantity(string productId)
        {
            try
            {
                var quantity = await _inventoryRepository.GetStockQuantityAsync(productId);
                return Ok(new { ProductId = productId, StockQuantity = quantity });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = $"Error fetching stock quantity: {ex.Message}" });
            }
        }
    }
}
