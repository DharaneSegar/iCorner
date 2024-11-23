package com.example.ead.activities

import android.content.Context
import android.content.Intent
import android.os.Bundle
import android.util.Log
import android.view.MenuInflater
import android.view.MenuItem
import android.view.View
import android.widget.Button
import android.widget.EditText
import android.widget.ImageButton
import android.widget.ImageView
import android.widget.PopupMenu
import android.widget.RatingBar
import android.widget.TextView
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import androidx.swiperefreshlayout.widget.SwipeRefreshLayout
import com.example.ead.GlobalVariable
import com.example.ead.R
import com.example.ead.fragments.HomeFragment
import com.example.ead.models.CartItem
import com.example.ead.models.Comment
import com.example.ead.util.RetrofitInstance
import com.google.gson.Gson
import com.squareup.picasso.Picasso
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import okhttp3.MediaType
import okhttp3.MediaType.Companion.toMediaTypeOrNull
import okhttp3.OkHttpClient
import okhttp3.Request
import okhttp3.RequestBody
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response

class ProductDetailActivity : AppCompatActivity() {

    private lateinit var buttonBack: ImageButton
    private lateinit var buttonCart: ImageButton
    private lateinit var clickcartHome: ImageView
    private lateinit var buttonAddToCart: Button

    private lateinit var quantityTextView: TextView
    private lateinit var buttonMinus: Button
    private lateinit var buttonPlus: Button
    private var quantity = 1
    private lateinit var ratingBarInput: RatingBar
    private lateinit var addReview: Button
    private lateinit var commentInput: EditText // Add EditText for comment input

    val baseUrl = GlobalVariable.BASE_URL

    private lateinit var buttonUser: ImageButton

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_product_detail)

        buttonBack = findViewById(R.id.buttonBack)
        clickcartHome = findViewById(R.id.clickcartHome)
        buttonCart = findViewById(R.id.buttonCart)
        buttonAddToCart = findViewById(R.id.buttonAddToCart)
        ratingBarInput = findViewById(R.id.ratingBarInput)
        addReview = findViewById(R.id.updateReview)

        ratingBarInput = findViewById(R.id.ratingBarInput)
        commentInput = findViewById(R.id.commentEditTextInput)


        val customerId = getCustomerId()
        if (customerId != null) {
            Log.d("ProductDetailActivity", "Customer ID: $customerId")
            // Use the customer ID as needed
        } else {
            Log.d("ProductDetailActivity", "Customer ID not found in SharedPreferences.")
        }


        // Handle Home click to load HomeFragment
        buttonCart.setOnClickListener {
            supportFragmentManager.beginTransaction()
                .replace(R.id.mainContent, HomeFragment())
                .commit()
        }


        // Handle Cart click to load Cart
        buttonCart.setOnClickListener {
            val intent = Intent(this, CartActivity::class.java)
            intent.flags =
                Intent.FLAG_ACTIVITY_NEW_TASK or Intent.FLAG_ACTIVITY_CLEAR_TASK // Clear previous activities
            this.startActivity(intent)
        }


        buttonAddToCart.setOnClickListener {
            val userId = getCustomerId()
            if (userId == null) {
                Toast.makeText(
                    this@ProductDetailActivity,
                    "Please login to add to cart",
                    Toast.LENGTH_SHORT
                ).show()
                return@setOnClickListener
            }

            // Get product details from the intent or UI elements
            val productId = intent.getStringExtra("productId")
            val productName = intent.getStringExtra("productName")
            val quantity = quantityTextView.text.toString()
                .toInt()  // Assuming quantityTextView displays the integer value
            val price = intent.getDoubleExtra("productPrice", 0.0)
            if (price == 0.0) {
                Toast.makeText(this, "Invalid price", Toast.LENGTH_SHORT).show()
            }
            val imagePath =
                intent.getStringExtra("productImageUrl")  // Assuming productImageUrl is a String value
            var id = null

            // Create CartItem object
            val cartItem = CartItem(id, productId, productName, quantity, price, imagePath,"Pending")

            // Call Retrofit to add item to cart
            addToCart(userId, cartItem)
        }

        buttonUser = findViewById(R.id.buttonUser)

        buttonUser.setOnClickListener {
            showUserOptions(buttonUser)
        }

        val rating = ratingBarInput.rating
        Log.d("rating bar input", rating.toString())


        // Set click listener for the back button
        buttonBack.setOnClickListener {
            val intent = Intent(this@ProductDetailActivity, HomeFragment::class.java)
            startActivity(intent)
        }

        // Initialize views
        val swipeRefreshLayout = findViewById<SwipeRefreshLayout>(R.id.swipeRefreshLayout)
        val nameTextView = findViewById<TextView>(R.id.productNameTextView)
        val priceTextView = findViewById<TextView>(R.id.productPriceTextView)
        val productImageView = findViewById<ImageView>(R.id.productImageView)
        val categoryTextView = findViewById<TextView>(R.id.productCategoryTextView)
        val vendorTextView = findViewById<TextView>(R.id.productVendorTextView)
        val descriptionTextView = findViewById<TextView>(R.id.productDescriptionTextView)
        quantityTextView = findViewById(R.id.quantityTextView)

        // Get data from Intent
        intent?.let {
            val productName = it.getStringExtra("productName") ?: ""
            val productPrice = it.getDoubleExtra("productPrice", 0.0)
            val productImage = it.getStringExtra("productImageUrl") ?: ""
            val productCategory = it.getStringExtra("productCategory") ?: ""
            val productVendor = it.getStringExtra("vendorName") ?: ""
            val description = it.getStringExtra("description") ?: ""
            quantity = it.getIntExtra("productCount", 1)


            // Bind data to views
            nameTextView.text = productName
            priceTextView.text = String.format("$%.2f", productPrice)
            categoryTextView.text = productCategory
            vendorTextView.text = productVendor
            descriptionTextView.text = description

            quantityTextView.text = quantity.toString()

            // Load product image using Picasso
            loadProductImage(productImage, productImageView)

            // Set click listener to open full screen image
            productImageView.setOnClickListener {
                val fullScreenIntent = Intent(this, FullScreenImageActivity::class.java).apply {
                    putExtra("productImage", productImage)
                }
                startActivity(fullScreenIntent)
            }
        }

        addReview.setOnClickListener {
            val userId = getCustomerId()
            val productId = intent.getStringExtra("productId")
            val vendorId = intent.getStringExtra("vendorName") ?: "defaultVendorId"
            val rating = ratingBarInput.rating
            val comment = commentInput.text.toString()

            if (userId == null || productId == null || comment.isEmpty()) {
                Toast.makeText(
                    this@ProductDetailActivity,
                    "Please fill in all fields.",
                    Toast.LENGTH_SHORT
                ).show()
                return@setOnClickListener
            }

            // Create the comment object
            val commentData = Comment("", userId, productId, vendorId, rating.toInt(), comment)

            // Send the comment data to the server
            submitComment(commentData)
        }


        // Set up SwipeRefreshLayout
        swipeRefreshLayout.setOnRefreshListener {
            refreshProductDetails(
                nameTextView.text.toString(),
                priceTextView.text.toString().toDouble(),
                productImageView.tag.toString(),
                categoryTextView.text.toString()
            )
            swipeRefreshLayout.isRefreshing = false // Stop the refreshing indicator once done
        }


        buttonMinus = findViewById(R.id.buttonMinus)
        buttonPlus = findViewById(R.id.buttonPlus)

        // Set up button listeners
        buttonMinus.setOnClickListener {
            if (quantity > 1) { // Ensure quantity does not go below 1
                quantity--
                quantityTextView.text = quantity.toString()
            }
        }

        buttonPlus.setOnClickListener {
            quantity++
            quantityTextView.text = quantity.toString()
        }
    }

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

    private fun submitComment(comment: Comment) {
        CoroutineScope(Dispatchers.IO).launch {
            try {
                // Convert comment data to JSON
                val jsonComment = Gson().toJson(comment)
                val requestBody = RequestBody.create(
                    "application/json; charset=utf-8".toMediaTypeOrNull(),
                    jsonComment
                )

                // Create the POST request
                val request = Request.Builder()
                    .url("$baseUrl/comment")
                    .post(requestBody)
                    .build()

                // Execute the request
                val client = OkHttpClient()
                val response = client.newCall(request).execute()

                // Check if the response is successful
                withContext(Dispatchers.Main) {
                    if (response.isSuccessful) {
                        Toast.makeText(
                            this@ProductDetailActivity,
                            "Review added successfully",
                            Toast.LENGTH_SHORT
                        ).show()
                        val responseData = response.body?.string()
                        val intent =
                            Intent(this@ProductDetailActivity, CommentsActivity::class.java)
                        startActivity(intent)
                        Log.d("ProductDetailActivity", "Response: $responseData")
                    } else {
                        Toast.makeText(
                            this@ProductDetailActivity,
                            "Failed to add review",
                            Toast.LENGTH_SHORT
                        ).show()
                        Log.d("ProductDetailActivity", "Error: ${response.code}")
                    }
                }
            } catch (e: Exception) {
                withContext(Dispatchers.Main) {
                    Toast.makeText(
                        this@ProductDetailActivity,
                        "Error: ${e.message}",
                        Toast.LENGTH_SHORT
                    ).show()
                    Log.e("ProductDetailActivity", "Error: ${e.message}", e)
                }
            }
        }
    }


    private fun addToCart(userId: String, cartItem: CartItem) {
        CoroutineScope(Dispatchers.IO).launch {
            try {
                val response = addItemToCart(userId, cartItem)
                Log.e("cart response", response.toString())
                withContext(Dispatchers.Main) {
                    if (response.isSuccessful) {
                        navigateToCartActivity()
                        Toast.makeText(
                            this@ProductDetailActivity,
                            "Item added to cart successfully",
                            Toast.LENGTH_SHORT
                        ).show()
                        println("item cart success")
                        // Navigate here after successful addition
                    } else {
                        Toast.makeText(
                            this@ProductDetailActivity,
                            "Failed to add item to cart",
                            Toast.LENGTH_SHORT
                        ).show()
                        println("item cart fail")

                    }
                }
            } catch (e: Exception) {
                withContext(Dispatchers.Main) {
                    Toast.makeText(
                        this@ProductDetailActivity,
                        "Error: ${e.message}",
                        Toast.LENGTH_SHORT
                    ).show()
                }
            }
        }
    }

    private fun navigateToCartActivity() {
        val intent = Intent(
            this,
            CartActivity::class.java
        ) // Replace CartActivity::class.java with the actual class name if different
        startActivity(intent)
    }


    private suspend fun addItemToCart(userId: String, cartItem: CartItem): okhttp3.Response {
        val client = OkHttpClient()

        val jsonCartItem = Gson().toJson(cartItem)
        val requestBody =
            RequestBody.create("application/json; charset=utf-8".toMediaTypeOrNull(), jsonCartItem)

        val request = Request.Builder()

            .url("$baseUrl/cart/$userId/items")
            .post(requestBody)
            .build()

        return client.newCall(request).execute()
    }


    private fun loadProductImage(productImage: String, productImageView: ImageView) {
        Log.d("ProductDetailActivity", "Loading image: $productImage")

        Picasso.get()
            .load(productImage)
            .placeholder(R.drawable.person) // Placeholder image
            .error(R.drawable.logo_dark) // Error image
            .into(productImageView, object : com.squareup.picasso.Callback {
                override fun onSuccess() {
                    Log.d("ProductDetailActivity", "Image loaded successfully: $productImage")
                }

                override fun onError(e: Exception) {
                    Log.e("ProductDetailActivity", "Error loading image: $productImage", e)
                }
            })
    }

    private fun refreshProductDetails(
        productName: String,
        productPrice: Double,
        productImage: String,
        productCategory: String
    ) {
        // Optionally, you can add logic here to fetch updated product details
        // For now, we'll just reset the views with the same data
        val nameTextView = findViewById<TextView>(R.id.productNameTextView)
        val priceTextView = findViewById<TextView>(R.id.productPriceTextView)
        val categoryTextView = findViewById<TextView>(R.id.productCategoryTextView)
        val productImageView = findViewById<ImageView>(R.id.productImageView)

        nameTextView.text = productName
        priceTextView.text = String.format("$%.2f", productPrice)
        categoryTextView.text = productCategory

        // Reload the product image (you may want to fetch the image again in a real app)
        loadProductImage(productImage, productImageView)
    }

    override fun onBackPressed() {
        Log.d("TAG", "Debug message")
        // Check if the fragment manager has a back stack
        super.onBackPressed()
        if (supportFragmentManager.backStackEntryCount > 0) {
            Log.d("TAG", "If Debug message")
            supportFragmentManager.popBackStack() // Go back to previous fragment
        } else {
            Log.d("TAG", "Else Debug message")
            finish() // Close activity if no fragments are in the stack
        }
    }

    private fun getCustomerId(): String? {
        val sharedPreferences = getSharedPreferences("user_prefs", Context.MODE_PRIVATE)
        return sharedPreferences.getString("customer_id", null) // Returns null if not found
    }


}