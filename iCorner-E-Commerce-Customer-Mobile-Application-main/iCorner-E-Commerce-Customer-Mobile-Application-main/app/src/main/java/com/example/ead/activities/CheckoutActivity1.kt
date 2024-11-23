package com.example.ead.activities

import android.annotation.SuppressLint
import android.content.Context
import android.content.Intent
import android.os.Bundle
import android.text.Editable
import android.util.Log
import android.view.MenuInflater
import android.view.MenuItem
import android.view.View
import androidx.appcompat.app.AppCompatActivity
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import android.widget.Button
import android.widget.EditText
import android.widget.ImageButton
import android.widget.PopupMenu
import android.widget.RadioButton
import android.widget.RadioGroup
import android.widget.TextView
import android.widget.Toast
import com.example.ead.GlobalVariable
import com.example.ead.R
import com.example.ead.adapters.CheckoutAdapter
import com.example.ead.models.CartItem
import com.example.ead.models.Order
import com.google.gson.Gson
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import okhttp3.MediaType.Companion.toMediaTypeOrNull
import okhttp3.OkHttpClient
import okhttp3.Request
import okhttp3.RequestBody
import org.json.JSONObject
import java.time.LocalDateTime
import java.time.format.DateTimeFormatter

class CheckoutActivity1 : AppCompatActivity() {
    private lateinit var cartRecyclerView: RecyclerView
    private lateinit var checkoutAdapter: CheckoutAdapter
    private lateinit var totalAmountTextView: TextView
    private lateinit var payNowButton: Button
    private lateinit var buttonBack: ImageButton
    private var cartItems: MutableList<CartItem> = mutableListOf()
    private lateinit var usernameTextView: TextView
    private lateinit var addressEditText: EditText
    private lateinit var subtotalAmountTextView: TextView
    private lateinit var deliveryFeeAmountTextView: TextView
    val baseUrl = GlobalVariable.BASE_URL
    private lateinit var buttonCart: ImageButton
    private lateinit var notesEditText: EditText
    private lateinit var paymentOptionsGroup: RadioGroup
    private lateinit var radioCOD: RadioButton
    private lateinit var radioVisa: RadioButton
    private lateinit var buttonUser: ImageButton


    @SuppressLint("MissingInflatedId")
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_checkout1)

        buttonBack = findViewById(R.id.buttonBack)
        buttonBack.setOnClickListener { onBackPressed() }

        cartRecyclerView = findViewById(R.id.cartRecyclerView)
        totalAmountTextView = findViewById(R.id.totalAmountTextView)
        payNowButton = findViewById(R.id.payNowButton)
        usernameTextView = findViewById(R.id.userNameTextView)
        addressEditText = findViewById(R.id.userAddressEditText)
        subtotalAmountTextView = findViewById(R.id.subtotalAmountTextView)
        deliveryFeeAmountTextView = findViewById(R.id.deliveryFeeAmountTextView)
        notesEditText = findViewById(R.id.notesEditText)

        buttonCart = findViewById(R.id.buttonCart)
        buttonUser = findViewById(R.id.buttonUser)

        buttonUser.setOnClickListener {
            showUserOptions(buttonUser)
        }

        paymentOptionsGroup = findViewById(R.id.paymentOptionsGroup)
        radioCOD = findViewById(R.id.radioCOD)
        radioVisa = findViewById(R.id.radioVisa)

        buttonCart.setOnClickListener {
            startActivity(Intent(this, CartActivity::class.java))
        }

        val sharedPreferences = getSharedPreferences("user_prefs", MODE_PRIVATE)
        val username = sharedPreferences.getString("customer_name", "User Name")
        val address = sharedPreferences.getString("customer_address", "User Address")
        val userId = sharedPreferences.getString("customer_id", null)
        var orderId = intent.getStringExtra("order_id") ?: null
        var cartId = intent.getStringExtra("cart_id") ?: null

        usernameTextView.text = username
        address?.let {
            addressEditText.text = Editable.Factory.getInstance().newEditable(it)
        }


        cartRecyclerView.layoutManager = LinearLayoutManager(this)
        checkoutAdapter = CheckoutAdapter(cartItems)
        cartRecyclerView.adapter = checkoutAdapter

        if (userId != null) {
            fetchCartItems(userId)
        }


        deliveryFeeAmountTextView.text = String.format("$5.00")

        val subtotal = intent.getDoubleExtra("total_amount", 0.0)
        subtotalAmountTextView.text = subtotal.toString()
        val total = subtotal + 5
        totalAmountTextView.text = String.format("$$total")

// Set up checkout button listener
        payNowButton.setOnClickListener {
            // Get user inputs
            val userId = sharedPreferences.getString("customer_id", null) ?: ""
            val totalPrice = calculateTotal()
            val shippingAddress = addressEditText.text.toString()
            val notes = notesEditText.text.toString()

            // Check if a payment option is selected
            val selectedRadioButtonId = paymentOptionsGroup.checkedRadioButtonId
            if (selectedRadioButtonId == -1) { // No radio button is selected
                Toast.makeText(
                    this@CheckoutActivity1,
                    "Please select a payment option",
                    Toast.LENGTH_SHORT
                ).show()
                return@setOnClickListener // Exit the click listener
            }

            // Get payment status from the selected radio button
            val paymentStatus = when (selectedRadioButtonId) {
                R.id.radioCOD -> "COD" // Assuming "Paid" for COD
                R.id.radioVisa -> "Visa" // Assuming "Paid" for Visa
                else -> "Unpaid" // Fallback case
            }

            val orderDate = LocalDateTime.now().format(DateTimeFormatter.ISO_DATE_TIME)

            // Create an Order object
            val order = Order(
                id = orderId,
                customerId = userId,
                cart = cartId ?: "",
                totalPrice = total,
                shippingAddress = shippingAddress,
                orderDate = orderDate, // Example date, replace with actual date if needed
                paymentStatus = paymentStatus,
                notes = notes
            )

            createOrder(order)
        }
    }

    // Method to show user options
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


    // Method to clear shared preferences and log out
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


    private fun createOrder(order: Order) {

        CoroutineScope(Dispatchers.IO).launch {
            try {

                val jsonOrder = Gson().toJson(order)


                val requestBody = RequestBody.create(
                    "application/json; charset=utf-8".toMediaTypeOrNull(),
                    jsonOrder
                )

                val request = Request.Builder()
                    .url("$baseUrl/order/create")
                    .post(requestBody)
                    .build()

                val client = OkHttpClient()

                val response = client.newCall(request).execute()


                if (response.isSuccessful) {
                    val responseBody = response.body?.string()
                    withContext(Dispatchers.Main) {
                        Toast.makeText(
                            this@CheckoutActivity1,
                            "Your order updated successfully!",
                            Toast.LENGTH_SHORT
                        ).show()
                        // Optionally, navigate to a different screen or update UI

                    }


                    val intent = Intent(this@CheckoutActivity1, OrdersActivity::class.java)
                    startActivity(intent)
                } else {
                    withContext(Dispatchers.Main) {
                        Toast.makeText(
                            this@CheckoutActivity1,
                            "Failed to place order",
                            Toast.LENGTH_SHORT
                        ).show()
                    }
                }
            } catch (e: Exception) {
                withContext(Dispatchers.Main) {
                    Log.e("CheckoutActivity", "Error creating order: ${e.message}")
                    Toast.makeText(
                        this@CheckoutActivity1,
                        "Error creating order",
                        Toast.LENGTH_SHORT
                    ).show()
                }
            }
        }

    }


    // Method to fetch cart items from the API
    private fun fetchCartItems(userId: String) {
        val cartId = intent.getStringExtra("cart_id")


        val url = "$baseUrl/cart/cart/$cartId"

        CoroutineScope(Dispatchers.IO).launch {
            try {
                val client = OkHttpClient()
                val request = Request.Builder()
                    .url(url)
                    .get()
                    .build()

                val response = client.newCall(request).execute()

                if (response.isSuccessful) {
                    val responseBody = response.body?.string()
                    responseBody?.let {
                        parseCartItems(it)

                    }

                } else {
                    withContext(Dispatchers.Main) {
                        Toast.makeText(
                            this@CheckoutActivity1,
                            "Failed to fetch cart items",
                            Toast.LENGTH_SHORT
                        ).show()
                    }
                }
            } catch (e: Exception) {
                withContext(Dispatchers.Main) {
                    Log.e("CheckoutActivity", "Error fetching cart items: ${e.message}")
                    Toast.makeText(
                        this@CheckoutActivity1,
                        "Error fetching cart items",
                        Toast.LENGTH_SHORT
                    ).show()
                }
            }
        }
    }

    // Method to parse cart items from the JSON response
    private suspend fun parseCartItems(responseBody: String) {
        try {
            // Parse the main JSON object
            val jsonObject = JSONObject(responseBody)
            val itemsArray = jsonObject.getJSONArray("items") // Get the items array

            cartItems.clear() // Clear the list to prevent duplication

            // Loop through each item in the array
            for (i in 0 until itemsArray.length()) {
                val itemObject = itemsArray.getJSONObject(i)

                // Extract product details from the item object
                val id = itemObject.getString("id")
                val productId = itemObject.getString("productId")
                val productName = itemObject.getString("productName")
                val quantity = itemObject.getInt("quantity")
                val price = itemObject.getDouble("price")
                val imageUrl = itemObject.getString("imagePath") // Full URL for the image

                // Create a new CartItem object
                val cartItem = CartItem(id, productId, productName, quantity, price, imageUrl,"Pending")
                cartItems.add(cartItem) // Add the item to the list
            }

            // Update the UI on the main thread
            withContext(Dispatchers.Main) {
                checkoutAdapter.notifyDataSetChanged() // Notify adapter to refresh RecyclerView

                // Calculate and display the total amount
                val subtotal = calculateTotal()
//                subtotalAmountTextView.text = String.format("$%.2f", subtotal)

                // Now that the subtotal is updated, update the total amount
            }

        } catch (e: Exception) {
            Log.e("CheckoutActivity", "Error parsing cart items: ${e.message}")
        }
    }


    // Method to calculate the total price of the cart
    private fun calculateTotal(): Double {
        var total = 0.0
        for (item in cartItems) {
            total += item.price * item.quantity
        }
        return total
    }


    override fun onBackPressed() {
        super.onBackPressed()
        if (supportFragmentManager.backStackEntryCount > 0) {
            supportFragmentManager.popBackStack()
        } else {
            finish()
        }
    }


}
