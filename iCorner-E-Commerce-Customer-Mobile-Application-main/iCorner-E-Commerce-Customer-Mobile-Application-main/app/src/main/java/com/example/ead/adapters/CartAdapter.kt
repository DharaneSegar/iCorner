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
import com.example.ead.models.CartItem
import com.squareup.picasso.Picasso
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import okhttp3.MediaType.Companion.toMediaTypeOrNull
import okhttp3.OkHttpClient
import okhttp3.Request


// CartAdapter class to manage the cart items in a RecyclerView
class CartAdapter(
    private var cartItems: MutableList<CartItem>, // List of cart items
    private val context: Context, // Context of the activity or fragment
    private val activity: CartActivity // Reference to the CartActivity
) : RecyclerView.Adapter<CartAdapter.CartViewHolder>() {

    // Base URL for API calls
    val baseUrl = GlobalVariable.BASE_URL

    // Create new ViewHolder for each cart item
    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): CartViewHolder {
        val view = LayoutInflater.from(parent.context).inflate(R.layout.cart_item, parent, false)
        return CartViewHolder(view)
    }

    // Bind data to the ViewHolder
    override fun onBindViewHolder(holder: CartViewHolder, position: Int) {
        val item = cartItems[position] // Get the current cart item
        holder.itemNameTextView.text = item.productName // Set the product name
        holder.itemPriceTextView.text = String.format("Price: $%.2f", item.price) // Set the price
        holder.itemCountTextView.text = item.quantity.toString() // Display the current quantity
        holder.itemCountChangeTextView.text = item.quantity.toString() // For quantity change display

        // Load product image using Picasso
        Picasso.get()
            .load(item.imagePath) // Load image from the provided URL
            .placeholder(R.drawable.logo_dark) // Placeholder image while loading
            .error(R.drawable.logo_dark) // Image to show if loading fails
            .into(holder.itemImageView)

        // Set click listener for the + button to increase item quantity
        holder.plusButton.setOnClickListener {
            item.quantity += 1 // Increment quantity
            holder.itemCountChangeTextView.text = item.quantity.toString() // Update the displayed quantity
        }

        // Set click listener for the - button to decrease item quantity
        holder.minusButton.setOnClickListener {
            if (item.quantity > 0) { // Only decrease if quantity is more than zero
                item.quantity -= 1 // Decrement quantity
                holder.itemCountChangeTextView.text = item.quantity.toString() // Update the displayed quantity
            }
        }

        // Set click listener for the edit button
        holder.editButton.setOnClickListener {
            val updatedQuantity = holder.itemCountChangeTextView.text.toString().toInt() // Get updated quantity
            val sharedPreferences = context.getSharedPreferences("user_prefs", Context.MODE_PRIVATE)
            val userId = sharedPreferences.getString("customer_id", null) // Retrieve user ID

            if (userId != null) {
                updateCartItemQuantity(userId, item.id, updatedQuantity) // Update quantity via API
            } else {
                Toast.makeText(context, "User ID not found", Toast.LENGTH_SHORT).show() // Show error if user ID is missing
            }
        }

        // Set up delete button click listener
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

    // Method to update the quantity of a cart item
    private fun updateCartItemQuantity(userId: String, itemId: String?, quantity: Int) {
        // Validate item ID and quantity
        if (itemId == null || quantity <= 0) {
            Toast.makeText(context, "Invalid item ID or quantity", Toast.LENGTH_SHORT).show()
            return // Exit if validation fails
        }

        // Launch coroutine for network call to update item quantity
        CoroutineScope(Dispatchers.IO).launch {
            val url = "$baseUrl/cart/$userId/items/$itemId" // Build API URL
            val client = OkHttpClient() // Create OkHttp client
            val requestBody = okhttp3.RequestBody.create(
                "application/json; charset=utf-8".toMediaTypeOrNull(),
                quantity.toString() // Sending quantity as plain text in the request body
            )
            val request = Request.Builder()
                .url(url)
                .put(requestBody) // Use PUT method to update the quantity
                .build()

            try {
                val response = client.newCall(request).execute() // Execute network call
                if (response.isSuccessful) {
                    withContext(Dispatchers.Main) { // Switch to the main thread for UI updates
                        // Find the updated item and refresh it in the RecyclerView
                        val position = cartItems.indexOfFirst { it.id == itemId }
                        if (position != -1) {
                            cartItems[position].quantity = quantity // Update quantity in the list
                            notifyItemChanged(position) // Notify the adapter about the change
                        }
                        activity.updateTotalAmount() // Update total amount displayed
                        Toast.makeText(
                            context,
                            "Item quantity updated successfully",
                            Toast.LENGTH_SHORT
                        ).show()
                    }
                } else {
                    withContext(Dispatchers.Main) { // Handle failure on main thread
                        Toast.makeText(
                            context,
                            "Failed to update item: ${response.message}",
                            Toast.LENGTH_SHORT
                        ).show()
                    }
                }
            } catch (e: Exception) {
                withContext(Dispatchers.Main) { // Handle exception on main thread
                    Log.e("CartAdapter", "Error updating item: ${e.message}") // Log the error
                    Toast.makeText(context, "Error updating item", Toast.LENGTH_SHORT).show()
                }
            }
        }
    }

    // Get the total number of items in the cart
    override fun getItemCount(): Int {
        return cartItems.size // Return size of the cart items list
    }

    // Method to delete an item from the cart
    private fun deleteCartItem(userId: String, itemId: String?, position: Int) {
        if (itemId == null) { // Validate item ID
            Toast.makeText(context, "Invalid item ID", Toast.LENGTH_SHORT).show()
            return // Exit if validation fails
        }

        // Coroutine for performing the network request on a background thread
        CoroutineScope(Dispatchers.IO).launch {
            val url = "$baseUrl/cart/$userId/items/$itemId" // Build API URL for deletion
            val client = OkHttpClient() // Create OkHttp client
            val request = Request.Builder()
                .url(url)
                .delete() // HTTP DELETE request
                .build()

            try {
                val response = client.newCall(request).execute() // Execute network call
                if (response.isSuccessful) {
                    // Update the UI on the main thread after successful deletion
                    withContext(Dispatchers.Main) {
                        // Remove the item from the cart list and notify the adapter
                        cartItems.removeAt(position) // Remove item from the list
                        notifyItemRemoved(position) // Notify adapter about the removal
                        notifyItemRangeChanged(position, cartItems.size) // Refresh remaining items

                        activity.updateTotalAmount() // Update total amount displayed

                        Toast.makeText(context, "Item deleted successfully", Toast.LENGTH_SHORT)
                            .show()
                    }
                } else {
                    withContext(Dispatchers.Main) { // Handle failure on main thread
                        Toast.makeText(
                            context,
                            "Failed to delete item: ${response.message}",
                            Toast.LENGTH_SHORT
                        ).show()
                    }
                }
            } catch (e: Exception) {
                withContext(Dispatchers.Main) { // Handle exception on main thread
                    Log.e("CartAdapter", "Error deleting item: ${e.message}") // Log the error
                    Toast.makeText(context, "Error deleting item", Toast.LENGTH_SHORT).show()
                }
            }
        }
    }

    // ViewHolder class for holding views of each cart item
    class CartViewHolder(itemView: View) : RecyclerView.ViewHolder(itemView) {
        val itemNameTextView: TextView = itemView.findViewById(R.id.itemNameTextView) // Product name TextView
        val itemPriceTextView: TextView = itemView.findViewById(R.id.itemPriceTextView) // Product price TextView
        val itemCountTextView: TextView = itemView.findViewById(R.id.itemCountTextView) // TextView for displaying quantity
        val itemCountChangeTextView: TextView = itemView.findViewById(R.id.itemCountChangeTextView) // TextView for updating quantity
        val itemImageView: ImageView = itemView.findViewById(R.id.itemImageView) // ImageView for product image
        val plusButton: Button = itemView.findViewById(R.id.plusButton) // Button to increase quantity
        val minusButton: Button = itemView.findViewById(R.id.minusButton) // Button to decrease quantity
        val itemLayout: LinearLayout = itemView.findViewById(R.id.itemLayout) // Layout for the item
        val deleteButton: ImageView = itemView.findViewById(R.id.deleteIcon) // Delete button
        val editButton: ImageView = itemView.findViewById(R.id.editButton)
}
}