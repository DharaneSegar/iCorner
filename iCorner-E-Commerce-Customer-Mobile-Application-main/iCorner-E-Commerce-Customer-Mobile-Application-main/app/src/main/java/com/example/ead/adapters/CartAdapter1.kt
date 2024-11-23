package com.example.ead.adapters


import android.content.Context
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Button
import android.widget.ImageView
import android.widget.LinearLayout
import android.widget.TextView
import android.widget.Toast
import androidx.recyclerview.widget.RecyclerView
import com.example.ead.GlobalVariable
import com.example.ead.R
import com.example.ead.activities.CartActivity
import com.example.ead.activities.CartActivity1
import com.example.ead.models.CartItem
import com.squareup.picasso.Picasso
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import okhttp3.MediaType.Companion.toMediaTypeOrNull
import okhttp3.OkHttpClient
import okhttp3.Request


// Adapter class for managing the cart items in the RecyclerView
class CartAdapter1(
    private var cartItems: MutableList<CartItem>,  // List of items in the cart
    private val context: Context,                   // Context for accessing resources
    private val activity: CartActivity1              // Reference to the activity managing the cart
) : RecyclerView.Adapter<CartAdapter1.CartViewHolder>() {

    // Base URL for making network requests
    val baseUrl = GlobalVariable.BASE_URL

    // Called when RecyclerView needs a new ViewHolder of the given type to represent an item
    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): CartViewHolder {
        // Inflate the cart_item layout and create a ViewHolder
        val view = LayoutInflater.from(parent.context).inflate(R.layout.cart_item, parent, false)
        return CartViewHolder(view)
    }

    // Called by RecyclerView to display the data at the specified position
    override fun onBindViewHolder(holder: CartViewHolder, position: Int) {
        // Get the item at the current position
        val item = cartItems[position]

        // Set item details to the respective TextViews
        holder.itemNameTextView.text = item.productName
        holder.itemPriceTextView.text = String.format("Price: $%.2f", item.price)
        holder.itemCountTextView.text = item.quantity.toString()
        holder.itemCountChangeTextView.text = item.quantity.toString()

        // Load item image using Picasso library
        Picasso.get()
            .load(item.imagePath)
            .placeholder(R.drawable.logo_dark) // Placeholder image while loading
            .error(R.drawable.logo_dark) // Image to show if loading fails
            .into(holder.itemImageView)

        // Set click listener for the '+' button to increase item quantity
        holder.plusButton.setOnClickListener {
            item.quantity += 1  // Increment the quantity
            holder.itemCountChangeTextView.text = item.quantity.toString()  // Update display
        }

        // Set click listener for the '-' button to decrease item quantity
        holder.minusButton.setOnClickListener {
            if (item.quantity > 0) { // Only allow decrement if quantity is greater than 0
                item.quantity -= 1  // Decrement the quantity
                holder.itemCountChangeTextView.text = item.quantity.toString() // Update display
            }
        }

        // Set click listener for the edit button to update item quantity
        holder.editButton.setOnClickListener {
            val updatedQuantity = holder.itemCountChangeTextView.text.toString().toInt() // Get new quantity
            // Get user ID from SharedPreferences
            val sharedPreferences = context.getSharedPreferences("user_prefs", Context.MODE_PRIVATE)
            val userId = sharedPreferences.getString("customer_id", null)

            // Update item quantity if user ID is found
            if (userId != null) {
                updateCartItemQuantity(userId, item.id, updatedQuantity)
            } else {
                Toast.makeText(context, "User ID not found", Toast.LENGTH_SHORT).show()
            }
        }

        // Set up delete button to remove item from cart
        // Set up delete button click listener
        holder.deleteButton.setOnClickListener {
            // Create an AlertDialog to confirm deletion
            val builder = android.app.AlertDialog.Builder(context)
            builder.setTitle("Delete Item")
            builder.setMessage("Are you sure you want to delete this item from the cart?")

            // Set up the buttons
            builder.setPositiveButton("Yes") { dialog, which ->
                // Retrieve user ID from SharedPreferences
                val sharedPreferences = context.getSharedPreferences("user_prefs", Context.MODE_PRIVATE)
                val userId = sharedPreferences.getString("customer_id", null)

                // Only proceed if user ID is available
                if (userId != null) {
                    deleteCartItem(userId, item.id, position) // Delete the cart item via API
                } else {
                    Toast.makeText(context, "User ID not found", Toast.LENGTH_SHORT).show() // Show error if user ID is missing
                }
            }

            builder.setNegativeButton("No") { dialog, which ->
                // Do nothing, just dismiss the dialog
                dialog.dismiss()
            }

            // Display the dialog
            builder.show()
        }

    }

    // Function to update the item quantity in the cart
    private fun updateCartItemQuantity(userId: String, itemId: String?, quantity: Int) {
        // Validate itemId and quantity
        if (itemId == null || quantity <= 0) {
            Toast.makeText(context, "Invalid item ID or quantity", Toast.LENGTH_SHORT).show()
            return
        }

        CoroutineScope(Dispatchers.IO).launch {
            // Get cart ID from activity
            val cartId = activity.cartId
            val url = "$baseUrl/cart/$cartId/item/$itemId" // Build URL for updating the item
            val client = OkHttpClient() // Create HTTP client
            val requestBody = okhttp3.RequestBody.create(
                "application/json; charset=utf-8".toMediaTypeOrNull(),
                quantity.toString() // Sending quantity as plain text in the request body
            )
            val request = Request.Builder()
                .url(url)
                .put(requestBody) // Specify PUT request for updating
                .build()

            try {
                val response = client.newCall(request).execute() // Execute network call
                if (response.isSuccessful) {
                    // Update UI on the main thread after successful update
                    withContext(Dispatchers.Main) {
                        // Find the updated item and refresh it in the RecyclerView
                        val position = cartItems.indexOfFirst { it.id == itemId }
                        if (position != -1) {
                            cartItems[position].quantity = quantity // Update quantity in the list
                            notifyItemChanged(position) // Notify adapter of the change
                        }
                        activity.updateTotalAmount() // Update total amount in activity
                        Toast.makeText(context, "Item quantity updated successfully", Toast.LENGTH_SHORT).show()
                    }
                } else {
                    // Handle failed response
                    withContext(Dispatchers.Main) {
                        Toast.makeText(context, "Failed to update item: ${response.message}", Toast.LENGTH_SHORT).show()
                    }
                }
            } catch (e: Exception) {
                // Handle any errors during the update
                withContext(Dispatchers.Main) {
                    Log.e("CartAdapter", "Error updating item: ${e.message}")
                    Toast.makeText(context, "Error updating item", Toast.LENGTH_SHORT).show()
                }
            }
        }
    }

    // Return the total number of items in the cart
    override fun getItemCount(): Int {
        return cartItems.size
    }

    // Method to delete an item from the cart
    private fun deleteCartItem(userId: String, itemId: String?, position: Int) {
        val cartId = activity.cartId // Get cart ID from activity
        if (itemId == null) {
            Toast.makeText(context, "Invalid item ID", Toast.LENGTH_SHORT).show()
            return
        }

        // Coroutine for performing the network request on a background thread
        CoroutineScope(Dispatchers.IO).launch {
            val url = "$baseUrl/cart/$cartId/item/$itemId" // Build URL for deleting the item
            val client = OkHttpClient() // Create HTTP client
            val request = Request.Builder()
                .url(url)
                .delete() // HTTP DELETE request
                .build()

            try {
                val response = client.newCall(request).execute() // Execute network call
                if (response.isSuccessful) {
                    // Update the UI on the main thread after successful deletion
                    withContext(Dispatchers.Main) {
                        cartItems.removeAt(position) // Remove item from list
                        notifyItemRemoved(position) // Notify adapter of the item removal
                        notifyItemRangeChanged(position, cartItems.size) // Update the range in adapter

                        activity.updateTotalAmount() // Update total amount in activity
                        Toast.makeText(context, "Item deleted successfully", Toast.LENGTH_SHORT).show()
                    }
                } else {
                    // Handle failed response
                    withContext(Dispatchers.Main) {
                        Toast.makeText(context, "Failed to delete item: ${response.message}", Toast.LENGTH_SHORT).show()
                    }
                }
            } catch (e: Exception) {
                // Handle any errors during deletion
                withContext(Dispatchers.Main) {
                    Log.e("CartAdapter", "Error deleting item: ${e.message}")
                    Toast.makeText(context, "Error deleting item", Toast.LENGTH_SHORT).show()
                }
            }
        }
    }

    // ViewHolder class for holding item views
    class CartViewHolder(itemView: View) : RecyclerView.ViewHolder(itemView) {
        val itemNameTextView: TextView = itemView.findViewById(R.id.itemNameTextView) // TextView for item name
        val itemPriceTextView: TextView = itemView.findViewById(R.id.itemPriceTextView) // TextView for item price
        val itemCountTextView: TextView = itemView.findViewById(R.id.itemCountTextView) // TextView for showing item count
        val itemCountChangeTextView: TextView = itemView.findViewById(R.id.itemCountChangeTextView) // TextView for changing item count
        val itemImageView: ImageView = itemView.findViewById(R.id.itemImageView) // ImageView for displaying item image
        val plusButton: Button = itemView.findViewById(R.id.plusButton) // Button for increasing item count
        val minusButton: Button = itemView.findViewById(R.id.minusButton) // Button for decreasing item count
        val itemLayout: LinearLayout = itemView.findViewById(R.id.itemLayout) // Layout for the item
        val deleteButton: ImageView = itemView.findViewById(R.id.deleteIcon) // Icon for deleting item
        val editButton: ImageView = itemView.findViewById(R.id.editButton) // Icon for editing item
    }
}
