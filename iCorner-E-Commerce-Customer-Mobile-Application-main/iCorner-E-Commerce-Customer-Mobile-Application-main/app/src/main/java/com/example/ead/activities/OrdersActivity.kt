package com.example.ead.activities

import android.content.Context
import android.content.Intent
import android.os.Bundle
import android.util.Log
import android.view.MenuInflater
import android.view.MenuItem
import android.view.View
import android.widget.AdapterView
import android.widget.ImageButton
import android.widget.PopupMenu
import android.widget.Spinner
import androidx.appcompat.app.AppCompatActivity
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.example.ead.GlobalVariable
import com.example.ead.R
import com.example.ead.adapters.OrderAdapter
import com.example.ead.fragments.HomeFragment
import com.example.ead.models.Order
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import okhttp3.OkHttpClient
import okhttp3.Request
import org.json.JSONArray
import org.json.JSONObject
import java.io.IOException

// Use the base URL from a global variable
val baseUrl = GlobalVariable.BASE_URL

class OrdersActivity : AppCompatActivity() {

    // Define UI components and data structures
    private lateinit var buttonBack: ImageButton
    private lateinit var orderRecyclerView: RecyclerView
    private lateinit var orderAdapter: OrderAdapter
    private lateinit var orderList: MutableList<Order>  // To store all orders
    private lateinit var filteredOrderList: MutableList<Order> // To store filtered orders
    private lateinit var spinnerOrderStatus: Spinner
    private lateinit var client: OkHttpClient
    private lateinit var buttonCart: ImageButton
    private lateinit var buttonUser: ImageButton

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_orders)  // Set layout for the activity

        // Initialize views (back button, recycler view, spinner for filtering)
        buttonBack = findViewById(R.id.buttonBack)
        orderRecyclerView = findViewById(R.id.orderRecyclerView)
        spinnerOrderStatus = findViewById(R.id.spinnerOrderStatus)
        buttonCart = findViewById(R.id.buttonCart)

        // Navigate to the cart activity when cart button is clicked
        buttonCart.setOnClickListener {
            startActivity(Intent(this, CartActivity::class.java))
        }

        // Navigate back to home fragment when back button is clicked
        buttonBack.setOnClickListener {
            startActivity(Intent(this, HomeFragment::class.java))
        }

        // Initialize the user options button and its click listener
        buttonUser = findViewById(R.id.buttonUser)
        buttonUser.setOnClickListener {
            showUserOptions(buttonUser) // Show popup menu for user options
        }

        // Set up RecyclerView with a LinearLayoutManager for vertical scrolling
        orderRecyclerView.layoutManager = LinearLayoutManager(this)

        // Initialize OkHttpClient to make network requests
        client = OkHttpClient()

        // Fetch orders from the server
        fetchOrders()
        println("Navigated to order activity")

        // Set up Spinner (dropdown) listener to filter orders based on status
        spinnerOrderStatus.onItemSelectedListener = object : AdapterView.OnItemSelectedListener {
            override fun onItemSelected(
                parent: AdapterView<*>?,
                view: View?,
                position: Int,
                id: Long
            ) {
                // Ensure the order list is initialized before filtering
                if (::orderList.isInitialized) {
                    val selectedStatus = parent?.getItemAtPosition(position).toString()
                    filterOrders(selectedStatus) // Filter the orders based on the selected status
                } else {
                    Log.e("OrdersActivity", "Order list is not initialized yet.")
                }
            }

            override fun onNothingSelected(parent: AdapterView<*>?) {
                // Do nothing when no item is selected
            }
        }
    }

    // Show popup menu with user options
    private fun showUserOptions(view: View) {
        val popupMenu = PopupMenu(this, view)
        val inflater: MenuInflater = popupMenu.menuInflater
        inflater.inflate(R.menu.menu_user_options, popupMenu.menu)

        // Handle the click event for each menu item
        popupMenu.setOnMenuItemClickListener { menuItem: MenuItem ->
            when (menuItem.itemId) {
                R.id.menu_home -> {
                    // Navigate back to the main activity
                    val intent = Intent(this, Main::class.java)
                    startActivity(intent)
                    true
                }
                R.id.menu_logout -> {
                    // Log the user out by clearing shared preferences
                    logoutUser()
                    true
                }

                R.id.menu_myprofile -> {
                    // Navigate to the HomeFragment (main activity)
                    val intent = Intent(this, ProfileActivity::class.java)
                    startActivity(intent)
                    true
                }
                else -> false
            }
        }
        popupMenu.show() // Show the popup menu
    }

    // Log out the user by clearing stored preferences
    private fun logoutUser() {
        val sharedPreferences = getSharedPreferences("user_prefs", Context.MODE_PRIVATE)
        val editor = sharedPreferences.edit()
        editor.clear() // Clears all saved data
        editor.apply() // Apply changes

        // Start the LoginRegisterActivity and clear the activity stack
        val intent = Intent(this, LoginRegisterActivity::class.java)
        intent.flags = Intent.FLAG_ACTIVITY_NEW_TASK or Intent.FLAG_ACTIVITY_CLEAR_TASK
        startActivity(intent)
    }

    // Fetch orders from the server for the logged-in user
    private fun fetchOrders() {
        println("Called fetch orders")
        orderList = mutableListOf() // Initialize the order list before adding elements

        // Use a coroutine to run the network request in the background
        CoroutineScope(Dispatchers.IO).launch {
            println("Called URL")
            try {
                val userId = getCustomerId() // Get the user ID from shared preferences
                if (userId != null) {
                    // Construct the URL to fetch orders for the user
                    val ordersUrl = "$baseUrl/order/customer/$userId"
                    val ordersResponse = makeGetRequest(ordersUrl) // Fetch data using GET request
                    Log.i("oredr response", ordersResponse.toString())
                    val ordersJsonArray = JSONArray(ordersResponse)

                    Log.i("oredr array", ordersJsonArray.toString())

                    // Loop through each order in the response and create Order objects
                    for (i in 0 until ordersJsonArray.length()) {
                        val orderJson = ordersJsonArray.getJSONObject(i)
                        val cartId = orderJson.getString("cart")
                        val totalPrice = orderJson.getDouble("totalPrice")
                        val status = orderJson.getString("status")
                        val products = fetchCartItems(cartId) // Fetch product names for the cart
                        val shippingAddress = orderJson.getString("shippingAddress")
                        val customerId = orderJson.getString("customerId")
                        val orderDate = orderJson.getString("orderDate")
                        val paymentStatus = orderJson.getString("paymentStatus")
                        val notes = orderJson.getString("notes")

                        // Create an Order object and add it to the list
                        orderList.add(
                            Order(
                                orderJson.getString("id"),
                                customerId,
                                products.joinToString(", "), // Combine product names into a string
                                cartId,
                                totalPrice,
                                shippingAddress,
                                orderDate,
                                status,
                                paymentStatus,
                                notes
                            )
                        )
                    }

                    Log.i("oredr LIST", orderList.toString())

                    // Update the RecyclerView with the fetched order list on the main thread
                    withContext(Dispatchers.Main) {
                        orderAdapter = OrderAdapter(orderList, this@OrdersActivity)
                        orderRecyclerView.adapter = orderAdapter // Set the adapter for the RecyclerView
                        Log.i("OrdersActivity fetched", "User ID found")
                    }

                } else {
                    Log.e("OrdersActivity", "User ID not found.")
                }
            } catch (e: Exception) {
                Log.e("OrdersActivity", "Error fetching orders: ${e.message}")
            }
        }
    }

    // Filter the list of orders based on the selected status
    private fun filterOrders(status: String) {
        filteredOrderList = if (status == "All") {
            orderList // Show all orders if "All" is selected
        } else {
            orderList.filter { it.status == status }.toMutableList() // Filter by status
        }

        // Update the RecyclerView with the filtered list
        orderAdapter = OrderAdapter(filteredOrderList, this)
        orderRecyclerView.adapter = orderAdapter
    }

    // Fetch the items in a cart by its ID and return a list of product names
    private fun fetchCartItems(cartId: String): List<String> {
        val products = mutableListOf<String>()
        val cartUrl = "$baseUrl/cart/cart/$cartId"

        try {
            // Fetch the cart details from the server
            val cartResponse = makeGetRequest(cartUrl)
            val cartJson = JSONObject(cartResponse)
            val itemsArray = cartJson.getJSONArray("items")

            // Loop through each item in the cart and get the product name
            for (j in 0 until itemsArray.length()) {
                val itemJson = itemsArray.getJSONObject(j)
                val productName = itemJson.getString("productName")
                products.add(productName)
            }
        } catch (e: Exception) {
            Log.e("OrdersActivity", "Error fetching cart items: ${e.message}")
        }
        return products // Return the list of product names
    }

    // Helper function to make a GET request using OkHttp
    private fun makeGetRequest(url: String): String {
        val request = Request.Builder().url(url).build()
        client.newCall(request).execute().use { response ->
            if (!response.isSuccessful) throw IOException("Unexpected code $response")
            return response.body?.string() ?: ""
        }
    }

    // Override the onBackPressed method to manage fragment back navigation
    override fun onBackPressed() {
        Log.d("TAG", "Debug message")
        // Check if the fragment manager has a back stack
        if (supportFragmentManager.backStackEntryCount > 0) {
            Log.d("TAG", "If Debug message")
            supportFragmentManager.popBackStack() // Go back to previous fragment
        } else {
            Log.d("TAG", "Else Debug message")
            finish() // Close the activity if no fragments are in the stack
        }
    }

    // Retrieve the customer ID from shared preferences
    private fun getCustomerId(): String? {
        val sharedPreferences = getSharedPreferences("user_prefs", Context.MODE_PRIVATE)
        return sharedPreferences.getString("customer_id", null)
    }
}
