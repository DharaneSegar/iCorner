// ------------------------------------------------------------------------------------
// File: CartRepository.cs
// Namespace: EAD.Repositories
// Description: This class implements the ICartRepository interface, providing methods
//              to manage carts in a MongoDB database. It includes operations such as
//              adding/removing items, updating quantities, clearing carts, and handling 
//              carts based on user ID or cart ID. The repository interacts with MongoDB
//              collections to persist and retrieve cart data.
// ------------------------------------------------------------------------------------

namespace EAD.Repositories
{
    using EAD.Models;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using System.Threading.Tasks;
    using System.Linq;
    using System.Collections.Generic;
    using System;

    public class CartRepository : ICartRepository
    {
        private readonly IMongoCollection<Cart> _carts;

        // --------------------------------------------------------------------------------
        // Constructor: CartRepository
        // Purpose: Initializes the CartRepository with the MongoDB collection for carts.
        // Parameters: 
        //      - database: IMongoDatabase (The MongoDB database instance).
        // --------------------------------------------------------------------------------
        public CartRepository(IMongoDatabase database)
        {
            _carts = database.GetCollection<Cart>("Carts");
        }

        // --------------------------------------------------------------------------------
        // Method: GetCartByUserIdAsync
        // Purpose: Retrieves the active (Status == true) cart for a given user by their ID.
        // Parameters: 
        //      - userId: string (The user's unique ID).
        // Returns: A Task<Cart> object representing the active cart for the user.
        // --------------------------------------------------------------------------------
        public async Task<Cart> GetCartByUserIdAsync(string userId)
        {
            return await _carts.Find(cart => cart.UserId == userId && cart.Status == true)
                               .FirstOrDefaultAsync();
        }

        // --------------------------------------------------------------------------------
        // Method: AddItemToCartAsync
        // Purpose: Adds an item to the user's active cart or creates a new cart if none exists.
        // Parameters: 
        //      - userId: string (The user's unique ID).
        //      - newItem: CartItem (The item to be added to the cart).
        // --------------------------------------------------------------------------------
        public async Task AddItemToCartAsync(string userId, CartItem newItem)
        {
            // Find all carts for the given user
            var carts = await _carts.Find(c => c.UserId == userId).ToListAsync();

            // Check if any cart is active (status is true)
            var activeCart = carts.FirstOrDefault(c => c.Status);

            // If no active cart exists, create a new one
            if (activeCart == null)
            {
                activeCart = new Cart { UserId = userId, Items = new List<CartItem>(), Status = true };
                await _carts.InsertOneAsync(activeCart);
            }

            // Update or add item to the active cart
            if (activeCart.Items.Any(i => i.ProductId == newItem.ProductId))
            {
                // Item already exists, update quantity
                var existingItem = activeCart.Items.FirstOrDefault(i => i.ProductId == newItem.ProductId);
                existingItem.Quantity += newItem.Quantity;
            }
            else
            {
                // New item, ensure it has an Id
                if (string.IsNullOrEmpty(newItem.Id))
                {
                    newItem.Id = ObjectId.GenerateNewId().ToString();
                }

                newItem.Status = "pending";

                // Add the item to the cart
                activeCart.Items.Add(newItem);
            }

            // Save the updated cart to the database
            await _carts.ReplaceOneAsync(c => c.UserId == userId && c.Status, activeCart);

            // Ensure the cart is not empty after adding the item
            if (activeCart.Items.Count == 0)
            {
                throw new InvalidOperationException("The cart cannot be empty after adding an item.");
            }
        }

        // --------------------------------------------------------------------------------
        // Method: RemoveItemFromCartAsync
        // Purpose: Removes an item from the user's active cart based on the item ID.
        // Parameters: 
        //      - userId: string (The user's unique ID).
        //      - itemId: string (The ID of the item to be removed).
        // --------------------------------------------------------------------------------
        public async Task RemoveItemFromCartAsync(string userId, string itemId)
        {
            var cart = await _carts.Find(c => c.UserId == userId && c.Status == true).FirstOrDefaultAsync();

            if (cart == null) return; // Exit if no active cart is found

            // Remove the item from the cart's item list
            cart.Items.RemoveAll(item => item.Id == itemId);

            // Update the cart in the database
            await _carts.ReplaceOneAsync(c => c.UserId == userId && c.Status == true, cart);
        }

        // --------------------------------------------------------------------------------
        // Method: ClearCartAsync
        // Purpose: Clears all items from the cart based on cart ID.
        // Parameters: 
        //      - cartId: string (The cart's unique ID).
        // --------------------------------------------------------------------------------
        public async Task ClearCartAsync(string cartId)
        {
            var cart = await GetCartByIdAsync(cartId);
            if (cart == null) return;

            cart.Items.Clear();
            await _carts.ReplaceOneAsync(c => c.Id == cartId, cart);
        }

        // --------------------------------------------------------------------------------
        // Method: UpdateCartItemQuantityAsync
        // Purpose: Updates the quantity of a specific item in the user's active cart.
        // Parameters: 
        //      - userId: string (The user's unique ID).
        //      - itemId: string (The ID of the item to update).
        //      - quantity: int (The new quantity of the item).
        // --------------------------------------------------------------------------------
        public async Task UpdateCartItemQuantityAsync(string userId, string itemId, int quantity)
        {
            var cart = await _carts.Find(c => c.UserId == userId && c.Status == true).FirstOrDefaultAsync();
            if (cart == null) return;

            var item = cart.Items.FirstOrDefault(i => i.Id == itemId);
            if (item != null)
            {
                item.Quantity = quantity;
                await _carts.ReplaceOneAsync(c => c.UserId == userId && c.Status == true, cart);
            }
        }

        // --------------------------------------------------------------------------------
        // Method: CreateCartAsync
        // Purpose: Creates a new cart for a user.
        // Parameters: 
        //      - userId: string (The user's unique ID).
        // Returns: A Task<Cart> object representing the created cart.
        // --------------------------------------------------------------------------------
        public async Task<Cart> CreateCartAsync(string userId)
        {
            var cart = new Cart { UserId = userId, Items = new List<CartItem>() };
            await _carts.InsertOneAsync(cart);
            return cart;
        }

        // --------------------------------------------------------------------------------
        // Method: DeleteCartAsync
        // Purpose: Deletes a user's cart based on their ID.
        // Parameters: 
        //      - userId: string (The user's unique ID).
        // --------------------------------------------------------------------------------
        public async Task DeleteCartAsync(string userId)
        {
            await _carts.DeleteOneAsync(cart => cart.UserId == userId);
        }

        // --------------------------------------------------------------------------------
        // Method: UpdateCartAsync
        // Purpose: Updates the cart status or other fields as needed.
        // Parameters: 
        //      - updatedCart: Cart (The cart object with updated details).
        // --------------------------------------------------------------------------------
        public async Task UpdateCartAsync(Cart updatedCart)
        {
            var existingCart = await _carts.Find(c => c.Id == updatedCart.Id).FirstOrDefaultAsync();
            if (existingCart != null)
            {
                existingCart.Status = updatedCart.Status;
                await _carts.ReplaceOneAsync(c => c.Id == existingCart.Id, existingCart);
            }
            else
            {
                throw new InvalidOperationException("Cart not found for update.");
            }
        }

        // --------------------------------------------------------------------------------
        // Method: GetCartByIdAsync
        // Purpose: Retrieves a cart by its unique cart ID.
        // Parameters: 
        //      - cartId: string (The cart's unique ID).
        // Returns: A Task<Cart> object representing the cart.
        // --------------------------------------------------------------------------------
        public async Task<Cart> GetCartByIdAsync(string cartId)
        {
            return await _carts.Find(cart => cart.Id == cartId).FirstOrDefaultAsync();
        }

        // --------------------------------------------------------------------------------
        // Method: UpdateCartItemQuantityByCartIdAsync
        // Purpose: Updates the quantity of an item within a cart based on cart ID.
        // Parameters: 
        //      - cartId: string (The cart's unique ID).
        //      - itemId: string (The item's unique ID).
        //      - quantity: int (The new quantity of the item).
        // --------------------------------------------------------------------------------
        public async Task UpdateCartItemQuantityByCartIdAsync(string cartId, string itemId, int quantity)
        {
            var cart = await _carts.Find(c => c.Id == cartId).FirstOrDefaultAsync();
            if (cart == null) return;

            var item = cart.Items.FirstOrDefault(i => i.Id == itemId);
            if (item != null)
            {
                item.Quantity = quantity;
                await _carts.ReplaceOneAsync(c => c.Id == cartId, cart);
            }
        }

        // --------------------------------------------------------------------------------
        // Method: RemoveItemFromCartByCartIdAsync
        // Purpose: Removes an item from the cart based on cart ID and item ID.
        // Parameters: 
        //      - cartId: string (The cart's unique ID).
        //      - itemId: string (The item's unique ID).
        // --------------------------------------------------------------------------------
        public async Task RemoveItemFromCartByCartIdAsync(string cartId, string itemId)
        {
            var cart = await GetCartByIdAsync(cartId);
            if (cart == null) return;

            cart.Items.RemoveAll(i => i.Id == itemId);
            await _carts.ReplaceOneAsync(c => c.Id == cartId, cart);
        }
    }
}
