// ------------------------------------------------------------------------------------
// File: ICartRepository.cs
// Namespace: EAD.Repositories
// Description: This interface defines the contract for the CartRepository. It includes 
//              methods for managing shopping carts, adding/removing items, updating 
//              quantities, and clearing carts. It ensures all essential cart operations 
//              are covered for user-specific cart management.
// ------------------------------------------------------------------------------------

namespace EAD.Repositories
{
    using EAD.Models;
    using global::EAD.Models;
    using System.Threading.Tasks;

    public interface ICartRepository
    {
        // --------------------------------------------------------------------------------
        // Method: GetCartByUserIdAsync
        // Purpose: Retrieves a shopping cart based on the user's ID.
        // Parameters: 
        //      - userId: string (The unique ID of the user whose cart needs to be retrieved).
        // Returns: A Task<Cart> object representing the user's cart.
        // --------------------------------------------------------------------------------
        Task<Cart> GetCartByUserIdAsync(string userId);

        // --------------------------------------------------------------------------------
        // Method: AddItemToCartAsync
        // Purpose: Adds an item to the user's shopping cart.
        // Parameters: 
        //      - userId: string (The user's unique ID).
        //      - item: CartItem (The item to be added to the cart).
        // --------------------------------------------------------------------------------
        Task AddItemToCartAsync(string userId, CartItem item);

        // --------------------------------------------------------------------------------
        // Method: RemoveItemFromCartAsync
        // Purpose: Removes an item from the user's shopping cart by item ID.
        // Parameters: 
        //      - userId: string (The user's unique ID).
        //      - itemId: string (The ID of the item to be removed).
        // --------------------------------------------------------------------------------
        Task RemoveItemFromCartAsync(string userId, string itemId);

        // --------------------------------------------------------------------------------
        // Method: ClearCartAsync
        // Purpose: Clears all items from a specific cart by cart ID.
        // Parameters: 
        //      - cartId: string (The unique cart ID whose items will be removed).
        // --------------------------------------------------------------------------------
        Task ClearCartAsync(string cartId);

        // --------------------------------------------------------------------------------
        // Method: UpdateCartItemQuantityAsync
        // Purpose: Updates the quantity of a specific item in the user's shopping cart.
        // Parameters: 
        //      - userId: string (The user's unique ID).
        //      - itemId: string (The ID of the item to update).
        //      - quantity: int (The new quantity for the item).
        // --------------------------------------------------------------------------------
        Task UpdateCartItemQuantityAsync(string userId, string itemId, int quantity);

        // --------------------------------------------------------------------------------
        // Method: CreateCartAsync
        // Purpose: Creates a new shopping cart for a user.
        // Parameters: 
        //      - userId: string (The user's unique ID for whom the cart is created).
        // Returns: A Task<Cart> object representing the newly created cart.
        // --------------------------------------------------------------------------------
        Task<Cart> CreateCartAsync(string userId);

        // --------------------------------------------------------------------------------
        // Method: DeleteCartAsync
        // Purpose: Deletes a user's shopping cart.
        // Parameters: 
        //      - userId: string (The user's unique ID whose cart is to be deleted).
        // --------------------------------------------------------------------------------
        Task DeleteCartAsync(string userId);

        // --------------------------------------------------------------------------------
        // Method: UpdateCartAsync
        // Purpose: Updates an existing shopping cart with new details.
        // Parameters: 
        //      - cart: Cart (The cart object with updated details).
        // --------------------------------------------------------------------------------
        Task UpdateCartAsync(Cart cart);

        // --------------------------------------------------------------------------------
        // Method: GetCartByIdAsync
        // Purpose: Retrieves a shopping cart by its unique cart ID.
        // Parameters: 
        //      - cartId: string (The unique cart ID).
        // Returns: A Task<Cart> object representing the cart.
        // --------------------------------------------------------------------------------
        Task<Cart> GetCartByIdAsync(string cartId);

        // --------------------------------------------------------------------------------
        // Method: UpdateCartItemQuantityByCartIdAsync
        // Purpose: Updates the quantity of an item in a cart based on cart ID and item ID.
        // Parameters: 
        //      - cartId: string (The cart's unique ID).
        //      - itemId: string (The item ID to update).
        //      - quantity: int (The new quantity for the item).
        // --------------------------------------------------------------------------------
        Task UpdateCartItemQuantityByCartIdAsync(string cartId, string itemId, int quantity);

        // --------------------------------------------------------------------------------
        // Method: RemoveItemFromCartByCartIdAsync
        // Purpose: Removes an item from the cart by its cart ID and item ID.
        // Parameters: 
        //      - cartId: string (The cart's unique ID).
        //      - itemId: string (The ID of the item to remove).
        // --------------------------------------------------------------------------------
        Task RemoveItemFromCartByCartIdAsync(string cartId, string itemId);
    }
}
