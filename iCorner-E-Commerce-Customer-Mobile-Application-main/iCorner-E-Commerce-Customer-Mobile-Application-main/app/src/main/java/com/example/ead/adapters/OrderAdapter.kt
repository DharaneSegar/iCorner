package com.example.ead.adapters

import android.annotation.SuppressLint
import android.content.Context
import android.content.Intent
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.ImageView
import android.widget.TextView
import android.widget.Toast
import androidx.core.content.ContextCompat
import androidx.recyclerview.widget.RecyclerView
import com.example.ead.GlobalVariable
import com.example.ead.R
import com.example.ead.activities.CartActivity1
import com.example.ead.activities.OrdersActivity
import com.example.ead.models.Order
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import okhttp3.OkHttpClient
import okhttp3.Request
import okhttp3.RequestBody
import java.time.ZonedDateTime
import java.time.format.DateTimeFormatter

val baseUrl = GlobalVariable.BASE_URL

// Adapter class for displaying orders in a RecyclerView
class OrderAdapter(private var orderList: List<Order>, private val context: Context) :
    RecyclerView.Adapter<OrderAdapter.OrderViewHolder>() {

    // ViewHolder class that holds references to the views for each order item
    class OrderViewHolder(view: View) : RecyclerView.ViewHolder(view) {
        // TextViews for displaying order information
        val orderNumberTextView: TextView = view.findViewById(R.id.orderNumberTextView) // Displays the order number
        val productsTextView: TextView = view.findViewById(R.id.productsTextView) // Displays the list of products
        val totalAmountTextView: TextView = view.findViewById(R.id.totalAmountTextView) // Displays the total amount of the order
        val statusTextView: TextView = view.findViewById(R.id.statusTextView) // Displays the order status
        val editButton: ImageView = view.findViewById(R.id.editButtonOrder) // Button to edit the order
        val deleteButton: ImageView = view.findViewById(R.id.deleteIconOrder) // Button to delete the order
        val orderDateTextView: TextView = view.findViewById(R.id.orderDateTextView) // Displays the order date
    }

    // Inflating the layout for each item in the RecyclerView
    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): OrderViewHolder {
        // Inflate the order_item layout to create a ViewHolder
        val view = LayoutInflater.from(parent.context)
            .inflate(R.layout.order_item, parent, false)
        return OrderViewHolder(view) // Return the ViewHolder
    }

    // Binding data to the ViewHolder for each item
    @SuppressLint("ResourceAsColor") // Suppress warnings about color resources
    override fun onBindViewHolder(holder: OrderViewHolder, position: Int) {
        // Get the order object at the current position
        val order = orderList[position]

        // Set the order number text
        holder.orderNumberTextView.text = "Order # : " + order.id
        // Set the products text
        holder.productsTextView.text = order.cart
        // Set the total amount text
        holder.totalAmountTextView.text = "Total Amount : $" + order.totalPrice.toString()
        // Set the order status text
        holder.statusTextView.text = "Status : " + order.status

        // Parse and format the order date
        val dateFormatter = DateTimeFormatter.ofPattern("yyyy-MM-dd") // Date format
        val parsedDate = ZonedDateTime.parse(order.orderDate).format(dateFormatter) // Parse and format the date
        holder.orderDateTextView.text = "Order Date : $parsedDate" // Set the formatted date text

        // Set status text color based on the order status
        when (order.status) {
            "Processing" -> holder.statusTextView.setTextColor(
                ContextCompat.getColor(context, R.color.g_orange_yellow) // Orange-yellow for Processing status
            )

            "Cancelled" -> holder.statusTextView.setTextColor(
                ContextCompat.getColor(context, R.color.red) // Red for Cancelled status
            )

            "Dispatched" -> holder.statusTextView.setTextColor(
                ContextCompat.getColor(context, R.color.dispatched) // Color for Dispatched status
            )

            "Completed" -> holder.statusTextView.setTextColor(
                ContextCompat.getColor(context, R.color.g_green) // Green for Completed status
            )

            else -> holder.statusTextView.setTextColor(
                ContextCompat.getColor(context, R.color.black) // Default color for other statuses
            )
        }

        // Hide edit and delete buttons if the order is "Dispatched", "Cancelled", or "Completed"
        if (order.status == "Dispatched" || order.status == "Cancelled" || order.status == "Completed") {
            holder.editButton.visibility = View.GONE // Hide edit button
            holder.deleteButton.visibility = View.GONE // Hide delete button
        } else {
            holder.editButton.visibility = View.VISIBLE // Show edit button
            holder.deleteButton.visibility = View.VISIBLE // Show delete button
        }

        // Handle delete button click
        // Handle delete button click with a confirmation alert
        holder.deleteButton.setOnClickListener {
            // Create an AlertDialog to confirm deletion
            val builder = android.app.AlertDialog.Builder(context)
            builder.setTitle("Cancel Order")
            builder.setMessage("Are you sure you want to cancel this order?")

            // Set up the buttons
            builder.setPositiveButton("Yes") { dialog, which ->
                // Proceed with the order cancellation if user confirms
                order.id?.let { orderId ->
                    updateOrderStatus(orderId) // Call method to cancel the order
                }
            }

            builder.setNegativeButton("No") { dialog, which ->
                // Do nothing, just dismiss the dialog
                dialog.dismiss()
            }

            // Display the dialog
            builder.show()
        }


        // Handle edit button click
        holder.editButton.setOnClickListener {
            // Create an Intent to start the CartActivity1 for editing the order
            val intent = Intent(context, CartActivity1::class.java)
            intent.putExtra("cartId", order.vendorId) // Pass vendor ID
            intent.putExtra("orderId", order.id) // Pass order ID
            context.startActivity(intent) // Start the activity
        }
    }

    // Return the total number of orders
    override fun getItemCount(): Int {
        return orderList.size
    }

    // Method to update the order status (e.g., cancel the order)
    private fun updateOrderStatus(orderId: String) {
        // Coroutine for performing the network request on a background thread
        CoroutineScope(Dispatchers.IO).launch {
            // Define the URL for the API call to cancel the order
            val url = "$baseUrl/order/cancel/$orderId"
            val client = OkHttpClient() // Create an OkHttpClient instance
            val request = Request.Builder()
                .url(url) // Set the URL for the request
                .put(RequestBody.create(null, ByteArray(0))) // Send an empty body with a PUT request
                .build()

            // Execute the request
            val response = client.newCall(request).execute()

            // Check if the response is successful
            if (response.isSuccessful) {
                withContext(Dispatchers.Main) { // Switch to the main thread for UI updates
                    Toast.makeText(context, "Order cancelled successfully", Toast.LENGTH_SHORT)
                        .show() // Show success message
                    updateOrderList(orderList) // Update the order list
                    val refresh = Intent(context, OrdersActivity::class.java) // Create intent to refresh OrdersActivity
                    context.startActivity(refresh) // Start the activity
                }
            } else {
                withContext(Dispatchers.Main) { // Switch to the main thread for UI updates
                    Toast.makeText(context, "Failed to cancel order", Toast.LENGTH_SHORT).show() // Show failure message
                }
            }
        }
    }

    // Method to update the order list and refresh the RecyclerView
    fun updateOrderList(newOrderList: List<Order>) {
        orderList = newOrderList // Update the order list
        notifyDataSetChanged() // Notify the RecyclerView to refresh
    }
}
