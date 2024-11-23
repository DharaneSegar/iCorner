package com.example.ead.adapters

import android.content.Context
import android.content.Intent
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.ImageView
import android.widget.TextView
import androidx.annotation.NonNull
import androidx.recyclerview.widget.RecyclerView
import com.example.ead.GlobalVariable
import com.example.ead.R
import com.example.ead.activities.ProductDetailActivity
import com.example.ead.models.Product
import com.squareup.picasso.Picasso
import okhttp3.OkHttpClient
import okhttp3.Request
import okhttp3.Response
import java.io.IOException
import kotlin.concurrent.thread

// Adapter class for displaying products in a RecyclerView
class ProductAdapter(private val context: Context, private val productList: List<Product>) :
    RecyclerView.Adapter<ProductAdapter.ProductViewHolder>() {

    private val client = OkHttpClient() // OkHttpClient for making network requests
    val baseUrl = GlobalVariable.BASE_URL // Base URL for API calls

    // Inflates the layout for each product item and creates a ViewHolder
    @NonNull
    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): ProductViewHolder {
        // Inflate the layout for each item
        val view =
            LayoutInflater.from(context).inflate(R.layout.adapter_product_item, parent, false)
        return ProductViewHolder(view) // Return a new instance of the ViewHolder
    }

    // Binds data to the ViewHolder for each product item
    override fun onBindViewHolder(holder: ProductViewHolder, position: Int) {
        // Get the product object at the current position
        val product = productList[position]
        Log.d("productList", product.toString()) // Log the product details for debugging

        // Set the product name to the corresponding TextView
        holder.textViewName.text = product.name
        // Format and set the product price to the corresponding TextView
        holder.textViewPrice.text = String.format("$%.2f", product.price)

        // Extract the vendor name from the vendorId email (before the "@")
        val vendorName = product.vendorId.substringBefore("@")
        // Set the extracted vendor name to the corresponding TextView
        holder.textViewVendor.text = vendorName

        // Load the product image using Picasso
        Log.d("ProductAdapter", "Loading image: ${product.imageUrl}")
        Picasso.get().load(product.imageUrl).into(holder.imageViewProduct)

        // Fetch and set the product rating from the server
        fetchProductRating(holder, product.id)

        // Handle item click to open ProductDetailActivity
        holder.itemView.setOnClickListener {
            // Create an Intent to start the ProductDetailActivity
            val intent = Intent(context, ProductDetailActivity::class.java).apply {
                putExtra("productName", product.name) // Pass product name
                putExtra("productPrice", product.price.toDouble()) // Pass product price
                putExtra("productImageUrl", product.imageUrl) // Pass product image URL
                putExtra("productCategory", product.categoryName) // Pass product category name
                putExtra("vendorName", vendorName) // Pass vendor name
                putExtra("description", product.description) // Pass product description
                putExtra("productId", product.id) // Pass product ID
            }

            // Start the ProductDetailActivity
            context.startActivity(intent)
        }
    }

    // Returns the total number of products in the list
    override fun getItemCount(): Int {
        return productList.size
    }

    // ViewHolder class to hold references to each view in the product layout
    class ProductViewHolder(itemView: View) : RecyclerView.ViewHolder(itemView) {
        val textViewName: TextView = itemView.findViewById(R.id.textViewProductName) // TextView for product name
        val textViewPrice: TextView = itemView.findViewById(R.id.textViewProductPrice) // TextView for product price
        val imageViewProduct: ImageView = itemView.findViewById(R.id.imageViewProduct) // ImageView for product image
        val textViewVendor: TextView = itemView.findViewById(R.id.textViewProductVendor) // TextView for vendor name
        val textViewRating: TextView = itemView.findViewById(R.id.textViewProductRating) // TextView for product rating
    }

    // Method to fetch the product rating from the server
    private fun fetchProductRating(holder: ProductViewHolder, productId: String) {
        val url = "$baseUrl/comment/product/$productId/rating" // URL to fetch the product rating
        val request = Request.Builder().url(url).build() // Build the HTTP request

        // Start a new thread to perform the network operation
        thread {
            try {
                // Execute the request
                val response: Response = client.newCall(request).execute()
                if (response.isSuccessful) {
                    // Parse the rating from the response body
                    val rating = response.body?.string()?.toDouble()

                    // Update the product's rating in the productList
                    productList[holder.adapterPosition].rating = rating

                    // Update UI on the main thread
                    (context as? androidx.appcompat.app.AppCompatActivity)?.runOnUiThread {
                        // Set the rating to the corresponding TextView
                        holder.textViewRating.text = ("Rating: " + (rating?.toString() ?: "N/A"))
                    }
                } else {
                    Log.e("ProductAdapter", "Failed to fetch rating: ${response.message}")
                    // Update UI on the main thread
                    (context as? androidx.appcompat.app.AppCompatActivity)?.runOnUiThread {
                        holder.textViewRating.text = "Rating: N/A" // Set to "N/A" if fetching failed
                    }
                }
            } catch (e: IOException) {
                Log.e("ProductAdapter", "Error fetching rating: ${e.message}")
                // Update UI on the main thread
                (context as? androidx.appcompat.app.AppCompatActivity)?.runOnUiThread {
                    holder.textViewRating.text = "Rating: N/A" // Set to "N/A" on error
                }
            }
        }
    }
}
