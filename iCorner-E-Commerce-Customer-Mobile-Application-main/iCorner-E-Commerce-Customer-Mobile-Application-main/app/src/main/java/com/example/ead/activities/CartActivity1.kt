package com.example.ead.activities

import android.annotation.SuppressLint
import android.content.Context
import android.content.Intent
import android.os.Bundle
import android.util.Log
import android.view.MenuInflater
import android.view.MenuItem
import android.view.View
import android.widget.Button
import android.widget.ImageButton
import android.widget.PopupMenu
import android.widget.TextView
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.example.ead.GlobalVariable
import com.example.ead.R
import com.example.ead.adapters.CartAdapter1
import com.example.ead.models.CartItem
import com.example.ead.models.CartResponse // Create this model class
import com.google.gson.Gson
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import okhttp3.OkHttpClient
import okhttp3.Request
import okhttp3.Response

class CartActivity1 : AppCompatActivity() {
    // Declare UI components and variables
    private lateinit var cartRecyclerView: RecyclerView
    private lateinit var cartAdapter: CartAdapter1
    private lateinit var totalAmountTextView: TextView
    private lateinit var checkoutButton: Button
    private lateinit var buttonBack: ImageButton
    private lateinit var buttonCart: ImageButton
    private lateinit var cartItems: MutableList<CartItem>
    val baseUrl = GlobalVariable.BASE_URL // Base URL for API requests
    lateinit var cartId: String
    private lateinit var buttonUser: ImageButton

    @SuppressLint("MissingInflatedId")
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_cart1)

        // Retrieve cartId from the Intent passed to this activity
        cartId = intent.getStringExtra("cartId").toString()
        var orderId = intent.getStringExtra("orderId")
        buttonCart = findViewById(R.id.buttonCart)

        // If cartId is not present, show a Toast and finish the activity
        if (cartId == null) {
            Toast.makeText(this, "Cart ID is missing", Toast.LENGTH_SHORT).show()
            finish()
            return
        }

        // Initialize views
        cartRecyclerView = findViewById(R.id.cartRecyclerView)
        totalAmountTextView = findViewById(R.id.totalAmountTextView)
        checkoutButton = findViewById(R.id.checkoutButton)
        buttonBack = findViewById(R.id.buttonBack)

        // Initialize RecyclerView and set up the adapter to display cart items
        cartItems = mutableListOf()
        cartAdapter = CartAdapter1(cartItems, this, this)
        cartRecyclerView.layoutManager = LinearLayoutManager(this)
        cartRecyclerView.adapter = cartAdapter

        // Update the total amount when the activity starts
        updateTotalAmount()

        buttonUser = findViewById(R.id.buttonUser)

        // Set up a click listener to show user options in a popup menu
        buttonUser.setOnClickListener {
            showUserOptions(buttonUser)
        }

        // Handle the back button click to return to the previous activity
        buttonBack.setOnClickListener { onBackPressed() }

        // Handle the cart button click to navigate to another CartActivity
        buttonCart.setOnClickListener {
            startActivity(Intent(this, CartActivity::class.java))
        }

        // Handle the checkout button click, passing cartId and totalAmount to CheckoutActivity1
        checkoutButton.setOnClickListener {
            val intent = Intent(this, CheckoutActivity1::class.java)
            val totalAmount = calculateTotal() // Calculate the total price of cart items
            intent.putExtra("total_amount", totalAmount)
            intent.putExtra("cart_id", cartId) // Pass the cart ID to CheckoutActivity1
            intent.putExtra("order_id", orderId) // Pass the order ID if needed
            startActivity(intent)
        }

        // Fetch the cart details from the server based on the cartId
        fetchCartDetails(cartId)
    }

    // Show user options in a popup menu
    private fun showUserOptions(view: View) {
        val popupMenu = PopupMenu(this, view)
        val inflater: MenuInflater = popupMenu.menuInflater
        inflater.inflate(R.menu.menu_user_options, popupMenu.menu)

        // Set up menu item click handling
        popupMenu.setOnMenuItemClickListener { menuItem: MenuItem ->
            when (menuItem.itemId) {
                R.id.menu_home -> {
                    // Navigate to the main home activity
                    val intent = Intent(this, Main::class.java)
                    startActivity(intent)
                    true
                }
                R.id.menu_logout -> {
                    // Call the logout method to clear user data and log out
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

    // Method to clear shared preferences and log out the user
    private fun logoutUser() {
        val sharedPreferences = getSharedPreferences("user_prefs", Context.MODE_PRIVATE)
        val editor = sharedPreferences.edit()
        editor.clear()  // Clears all the saved user data
        editor.apply()  // Apply the changes

        // Start the LoginRegisterActivity and clear the activity stack
        val intent = Intent(this, LoginRegisterActivity::class.java)
        intent.flags = Intent.FLAG_ACTIVITY_NEW_TASK or Intent.FLAG_ACTIVITY_CLEAR_TASK
        startActivity(intent)
    }

    // Fetch cart details from the server for a specific cartId
    private fun fetchCartDetails(cartId: String) {
        println("called fetch cart details" + cartId)
        CoroutineScope(Dispatchers.IO).launch {
            val url = "$baseUrl/cart/cart/$cartId"
            val client = OkHttpClient()
            val request = Request.Builder().url(url).build()

            try {
                // Send the request to fetch cart details
                val response: Response = client.newCall(request).execute()
                if (response.isSuccessful) {
                    val responseData = response.body?.string()
                    responseData?.let {
                        // Parse the response JSON to CartResponse using Gson
                        val cartResponse = Gson().fromJson(it, CartResponse::class.java)
                        withContext(Dispatchers.Main) {
                            // Update cartItems and notify the adapter
                            cartItems.clear()
                            cartItems.addAll(cartResponse.items)
                            cartAdapter.notifyDataSetChanged()

                            // Update the total amount after cart details are fetched
                            updateTotalAmount()
                        }
                    }
                } else {
                    Log.e("CartActivity1", "Error fetching cart details: ${response.message}")
                }
            } catch (e: Exception) {
                Log.e("CartActivity1", "Exception: ${e.message}")
            }
        }
    }

    // Update the total amount displayed in the TextView based on cart items
    fun updateTotalAmount() {
        val total = calculateTotal() // Calculate the total amount
        totalAmountTextView.text = String.format("Total: $%.2f", total)
    }

    // Calculate the total price of all items in the cart
    private fun calculateTotal(): Double {
        var total = 0.0
        for (item in cartItems) {
            total += item.price * item.quantity // Multiply item price by quantity
        }
        return total
    }

    // Handle the back button press to either pop fragments or finish the activity
    override fun onBackPressed() {
        super.onBackPressed()
        if (supportFragmentManager.backStackEntryCount > 0) {
            supportFragmentManager.popBackStack() // Go back to the previous fragment
        } else {
            finish() // Close the activity if no fragments are in the back stack
        }
    }

    // Recalculate total when the activity is resumed (e.g., after a refresh)
    override fun onResume() {
        super.onResume()
        updateTotalAmount()
    }
}
