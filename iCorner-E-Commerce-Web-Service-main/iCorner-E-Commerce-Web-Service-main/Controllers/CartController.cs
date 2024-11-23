// -----------------------------------------------------------------------
// <summary>
// This class represents the CartController, which manages cart-related operations
// for users in the application. It provides endpoints to create, retrieve, 
// update, and delete carts and cart items, as well as to clear a cart.
// </summary>
// <remarks>
// This controller relies on the ICartRepository interface for data access operations.
// </remarks>
// -----------------------------------------------------------------------

using EAD.Models;
using EAD.Repositories;
using global::EAD.Models;
using global::EAD.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EAD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _cartRepository;

        public CartController(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        // Get Cart for a User
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCartByUserId(string userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);

            if (cart == null)
            {
                return NotFound(new { message = $"No cart found for the user with ID: {userId}" });
            }

            return Ok(cart);
        }

        // Add Item to Cart
        [HttpPost("{userId}/items")]
        public async Task<IActionResult> AddItemToCart(string userId, [FromBody] CartItem cartItem)
        {
            if (cartItem == null)
                return BadRequest("Invalid item data.");

            await _cartRepository.AddItemToCartAsync(userId, cartItem);
            return Ok(cartItem);
        }

        // Update Item Quantity in Cart
        [HttpPut("{userId}/items/{itemId}")]
        public async Task<IActionResult> UpdateCartItemQuantity(string userId, string itemId, [FromBody] int quantity)
        {
            if (quantity <= 0)
                return BadRequest("Quantity must be greater than 0.");

            await _cartRepository.UpdateCartItemQuantityAsync(userId, itemId, quantity);
            return Ok(new { message = "Item quantity updated successfully." });
        }

        // Remove Item from Cart
        [HttpDelete("{userId}/items/{itemId}")]
        public async Task<IActionResult> RemoveItemFromCart(string userId, string itemId)
        {
            Console.WriteLine(userId + " " + itemId);
            await _cartRepository.RemoveItemFromCartAsync(userId, itemId);
            return Ok(new { message = "Item removed from cart successfully." });
        }

        // Clear Cart for a User
        [HttpDelete("{cartId}/clear")]
        public async Task<IActionResult> ClearCart(string cartId)
        {
            await _cartRepository.ClearCartAsync(cartId);
            return Ok(new { message = "Cart cleared successfully." });
        }

        // Create Cart for a User
        [HttpPost("{userId}")]
        public async Task<IActionResult> CreateCart(string userId)
        {
            var cart = await _cartRepository.CreateCartAsync(userId);
            return Ok(cart);
        }

        // Delete Cart
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteCart(string userId)
        {
            await _cartRepository.DeleteCartAsync(userId);
            return Ok(new { message = "Cart deleted successfully." });
        }

        // Updated method to accept status directly
        [HttpPut("{userId}/status/{cartId}")]
        public async Task<IActionResult> UpdateCartStatus(string userId, string cartId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);

            // Check if the cart exists and if the provided cartId matches
            if (cart == null || cart.Id != cartId)
            {
                return NotFound(new { message = $"No cart found for the user with ID: {userId} and cart ID: {cartId}" });
            }

            // Update the cart status
            cart.Status = false;
            await _cartRepository.UpdateCartAsync(cart);
            return Ok(new { message = "Cart status updated successfully." });
        }

        // Get Cart by ID

        [HttpGet("cart/{cartId}")]
        public async Task<IActionResult> GetCartById(string cartId)
        {
            var cart = await _cartRepository.GetCartByIdAsync(cartId);

            if (cart == null)
            {
                return NotFound(new { message = $"No cart found with ID: {cartId}" });
            }
            if (cart == null)
            {
                return NotFound(new { message = $"No cart found with ID: {cartId}" });
            }

            return Ok(cart);
        }


        [HttpPut("{cartId}/item/{itemId}")]
        public async Task<IActionResult> UpdateCartItemQuantityByCartId(string cartId, string itemId, [FromBody] int quantity)
        {
            if (quantity <= 0)
                return BadRequest("Quantity must be greater than 0.");

            await _cartRepository.UpdateCartItemQuantityByCartIdAsync(cartId, itemId, quantity);
            return Ok(new { message = "Item quantity updated successfully." });
        }

        [HttpDelete("{cartId}/item/{itemId}")]
        public async Task<IActionResult> RemoveItemFromCartByCartId(string cartId, string itemId)
        {
            await _cartRepository.RemoveItemFromCartByCartIdAsync(cartId, itemId);
            return Ok(new { message = "Item removed from cart successfully." });
        }
    }
}
