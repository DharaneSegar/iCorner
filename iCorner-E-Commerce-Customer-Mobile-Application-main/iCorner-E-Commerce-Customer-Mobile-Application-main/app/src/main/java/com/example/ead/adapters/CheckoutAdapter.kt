package com.example.ead.adapters

import android.content.Intent
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.ImageView
import android.widget.LinearLayout
import android.widget.TextView
import androidx.recyclerview.widget.RecyclerView
import com.example.ead.activities.ProductDetailActivity
import com.example.ead.R
import com.example.ead.models.CartItem
import com.squareup.picasso.Picasso

// Adapter class for managing cart items during checkout in the RecyclerView
class CheckoutAdapter(private val cartItems: List<CartItem>) :
    RecyclerView.Adapter<CheckoutAdapter.CartViewHolder>() {

    // Called when RecyclerView needs a new ViewHolder of the given type to represent an item
    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): CartViewHolder {
        // Inflate the checkout_item layout and create a ViewHolder
        val view =
            LayoutInflater.from(parent.context).inflate(R.layout.checkout_item, parent, false)
        return CartViewHolder(view)
    }

    // Called by RecyclerView to display the data at the specified position
    override fun onBindViewHolder(holder: CartViewHolder, position: Int) {
        // Get the cart item at the current position
        val item = cartItems[position]

        // Set item details to the respective TextViews
        holder.itemNameTextView.text = item.productName
        holder.itemPriceTextView.text = String.format("Price: $%.2f", item.price)
        holder.itemCountTextView.text = "Count: ${item.quantity}"

        // Load item image using Picasso library
        Picasso.get()
            .load(item.imagePath) // Load image from URL
            .placeholder(R.drawable.logo_dark) // Placeholder image while loading
            .error(R.drawable.logo_dark) // Image to show if loading fails
            .into(holder.itemImageView) // Set image to ImageView

        // Handle item click to open ProductDetailActivity
        holder.itemLayout.setOnClickListener {
            Log.d("Checkout Adapter", "Item clicked") // Log the item click event

            // Create an Intent to start ProductDetailActivity
            val intent =
                Intent(holder.itemLayout.context, ProductDetailActivity::class.java).apply {
                    putExtra("productName", item.productName) // Pass product name
                    putExtra("productPrice", item.price) // Pass product price
                    putExtra("productImageUrl", item.imagePath) // Pass product image URL
                    putExtra("productCategory", "Default") // Pass product category (default set here)
                    putExtra("productCount", item.quantity) // Pass product count
                }

            // Start the ProductDetailActivity
            holder.itemLayout.context.startActivity(intent)
        }
    }

    // Return the total number of items in the cart
    override fun getItemCount(): Int {
        return cartItems.size
    }

    // ViewHolder class for holding item views
    class CartViewHolder(itemView: View) : RecyclerView.ViewHolder(itemView) {
        // Initialize views for the cart item
        val itemNameTextView: TextView = itemView.findViewById(R.id.itemNameTextView) // TextView for item name
        val itemPriceTextView: TextView = itemView.findViewById(R.id.itemPriceTextView) // TextView for item price
        val itemCountTextView: TextView = itemView.findViewById(R.id.itemCountTextView) // TextView for item count
        val itemImageView: ImageView = itemView.findViewById(R.id.itemImageView) // ImageView for displaying item image
        val itemLayout: LinearLayout = itemView.findViewById(R.id.itemLayout) // Layout for the item
    }
}
