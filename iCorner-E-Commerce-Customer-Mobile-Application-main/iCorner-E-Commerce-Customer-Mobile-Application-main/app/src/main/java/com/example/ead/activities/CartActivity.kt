package com.example.ead.activities

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
import com.example.ead.adapters.CartAdapter
import com.example.ead.models.CartItem
import com.example.ead.models.CartResponse
import com.google.gson.Gson
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import okhttp3.OkHttpClient
import okhttp3.Request
import okhttp3.Response

class CartActivity : AppCompatActivity() {
    private lateinit var cartRecyclerView: RecyclerView
    private lateinit var cartAdapter: CartAdapter
    private lateinit var totalAmountTextView: TextView
    private lateinit var clearCart: TextView
    private lateinit var checkoutButton: Button
    private lateinit var buttonBack: ImageButton
    private lateinit var cartItems: MutableList<CartItem>
    val baseUrl = GlobalVariable.BASE_URL
    private var cartId: String? = null
    private lateinit var buttonUser: ImageButton

    /**
     * Initializes the activity and sets up UI components.
     * This method is called when the activity is created.
     */
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_cart)

        // Initialize views
        cartRecyclerView = findViewById(R.id.cartRecyclerView)
        totalAmountTextView = findViewById(R.id.totalAmountTextView)
        checkoutButton = findViewById(R.id.checkoutButton)
        clearCart = findViewById(R.id.clearCart)
        buttonBack = findViewById(R.id.buttonBack)

        // Set up RecyclerView
        cartItems = mutableListOf()
        cartAdapter = CartAdapter(cartItems, this, this)
        cartRecyclerView.layoutManager = LinearLayoutManager(this)
        cartRecyclerView.adapter = cartAdapter

        updateTotalAmount()

        // Set click listeners
        buttonBack.setOnClickListener { onBackPressed() }
        clearCart.setOnClickListener {
            // Call the method to clear the cart
            clearCartItemsFromServer()
        }

        buttonUser = findViewById(R.id.buttonUser)

        buttonUser.setOnClickListener {
            Log.d("CheckoutActivity1", "User icon clicked")
            showUserOptions(buttonUser)
        }

        checkoutButton.setOnClickListener {
            // Pass cart ID and total amount to CheckoutActivity
            if (cartId != null) {
                val intent = Intent(this, CheckoutActivity::class.java)
                val totalAmount = calculateTotal()
                intent.putExtra("total_amount", totalAmount)
                intent.putExtra("cart_id", cartId) // Pass the cart ID
                startActivity(intent)
            } else {
                Toast.makeText(this, "Cart ID not available", Toast.LENGTH_SHORT).show()
            }
        }

        // Fetch cart details
        fetchCartDetails()
    }

    /**
     * Displays a popup menu with user options.
     * @param view The view where the popup menu will anchor.
     */
    private fun showUserOptions(view: View) {
        val popupMenu = PopupMenu(this, view)
        val inflater: MenuInflater = popupMenu.menuInflater
        inflater.inflate(R.menu.menu_user_options, popupMenu.menu)

        popupMenu.setOnMenuItemClickListener { menuItem: MenuItem ->
            when (menuItem.itemId) {
                R.id.menu_home -> {
                    // Navigate to HomeFragment
                    val intent = Intent(this, Main::class.java)
                    startActivity(intent)
                    true
                }

                R.id.menu_logout -> {
                    // Clear shared preferences and logout
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
        popupMenu.show()
    }

    /**
     * Clears shared preferences and logs out the user.
     */
    private fun logoutUser() {
        val sharedPreferences = getSharedPreferences("user_prefs", Context.MODE_PRIVATE)
        val editor = sharedPreferences.edit()
        editor.clear()  // Clears all the saved data
        editor.apply()  // Apply changes

        // Start the LoginRegisterActivity
        val intent = Intent(this, LoginRegisterActivity::class.java)
        intent.flags = Intent.FLAG_ACTIVITY_NEW_TASK or Intent.FLAG_ACTIVITY_CLEAR_TASK
        startActivity(intent)
    }

    /**
     * Sends a request to the server to clear the cart items.
     */
    private fun clearCartItemsFromServer() {
        CoroutineScope(Dispatchers.IO).launch {
            val userId = getCustomerId() // Get the userId from SharedPreferences
            if (userId == null) {
                withContext(Dispatchers.Main) {
                    Toast.makeText(this@CartActivity, "User ID not found", Toast.LENGTH_SHORT)
                        .show()
                }
                return@launch
            }

            val url = "$baseUrl/cart/$cartId/clear"
            val client = OkHttpClient()
            val request = Request.Builder()
                .url(url)
                .delete() // HTTP DELETE request
                .build()

            try {
                val response: Response = client.newCall(request).execute()
                if (response.isSuccessful) {
                    withContext(Dispatchers.Main) {
                        // Clear the UI after successful deletion
                        clearCartItems()
                        Toast.makeText(
                            this@CartActivity,
                            "Cart cleared successfully",
                            Toast.LENGTH_SHORT
                        ).show()
                    }
                } else {
                    withContext(Dispatchers.Main) {
                        Toast.makeText(
                            this@CartActivity,
                            "Failed to clear cart",
                            Toast.LENGTH_SHORT
                        ).show()
                    }
                }
            } catch (e: Exception) {
                withContext(Dispatchers.Main) {
                    Log.e("CartActivity", "Error clearing cart: ${e.message}")
                    Toast.makeText(this@CartActivity, "Error clearing cart", Toast.LENGTH_SHORT)
                        .show()
                }
            }
        }
    }

    /**
     * Clears the cart items from the UI and updates the total amount.
     */
    private fun clearCartItems() {
        cartItems.clear()
        cartAdapter.notifyDataSetChanged()
        updateTotalAmount()
    }

    /**
     * Fetches the cart details from the server.
     */
    private fun fetchCartDetails() {
        CoroutineScope(Dispatchers.IO).launch {
            val userId = getCustomerId() // Replace with the actual cart ID
            val url = "$baseUrl/cart/$userId"
            val client = OkHttpClient()
            val request = Request.Builder().url(url).build()
            updateTotalAmount()

            try {
                val response: Response = client.newCall(request).execute()
                if (response.isSuccessful) {
                    val responseData = response.body?.string()
                    responseData?.let {
                        val cartResponse = Gson().fromJson(it, CartResponse::class.java)
                        withContext(Dispatchers.Main) {
                            cartId = cartResponse.id
                            // Update cartItems and notify the adapter
                            cartItems.clear()
                            cartItems.addAll(cartResponse.items)
                            cartAdapter.notifyDataSetChanged()

                            // Update the total amount after cart details are fetched
                            updateTotalAmount()
                        }
                        println("cart activity success")
                    }
                } else {
                    Log.e("CartActivity", "Error fetching cart details: ${response.message}")
                }
            } catch (e: Exception) {
                Log.e("CartActivity", "Exception: ${e.message}")
            }
        }
    }

    /**
     * Updates the total amount displayed in the UI.
     */
    fun updateTotalAmount() {
        val total = calculateTotal()
        totalAmountTextView.text = String.format("Total: $%.2f", total)
    }

    /**
     * Calculates the total amount for the items in the cart.
     * @return Total amount as a Double.
     */
    private fun calculateTotal(): Double {
        var total = 0.0
        for (item in cartItems) {
            total += item.price * item.quantity
        }
        return total
    }

    /**
     * Overrides the back button press to handle fragment stack.
     */
    override fun onBackPressed() {
        super.onBackPressed()
        if (supportFragmentManager.backStackEntryCount > 0) {
            supportFragmentManager.popBackStack() // Go back to previous fragment
        } else {
            finish() // Close activity if no fragments are in the stack
        }
    }

    /**
     * Retrieves the customer ID from shared preferences.
     * @return Customer ID as a String, or null if not found.
     */
    private fun getCustomerId(): String? {
        val sharedPreferences = getSharedPreferences("user_prefs", Context.MODE_PRIVATE)
        return sharedPreferences.getString("customer_id", null)
    }

    /**
     * Recalculates and updates the total amount when the activity is resumed.
     */
    override fun onResume() {
        super.onResume()
        // Recalculate total when the activity is resumed (e.g., after a refresh)
        updateTotalAmount()
    }
}
