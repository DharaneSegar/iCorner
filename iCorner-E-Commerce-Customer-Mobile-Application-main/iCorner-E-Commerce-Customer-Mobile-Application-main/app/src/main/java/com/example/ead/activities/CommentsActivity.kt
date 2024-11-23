package com.example.ead.activities

import android.content.Context
import android.content.Intent
import android.os.Bundle
import android.os.Handler
import android.os.Looper
import android.util.Log
import android.view.MenuInflater
import android.view.MenuItem
import android.view.View
import android.widget.ImageButton
import android.widget.ImageView
import android.widget.PopupMenu
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.example.ead.GlobalVariable
import com.example.ead.R
import com.example.ead.adapters.CommentAdapter
import com.example.ead.models.Comment
import com.example.ead.models.Product
import com.google.gson.Gson
import com.google.gson.reflect.TypeToken
import okhttp3.Call
import okhttp3.Callback
import okhttp3.OkHttpClient
import okhttp3.Request
import okhttp3.Response
import java.io.IOException

class CommentsActivity : AppCompatActivity() {

    // Declare the UI components and data variables
    private lateinit var commentRecyclerView: RecyclerView
    private lateinit var commentAdapter: CommentAdapter
    private lateinit var commentsList: MutableList<Comment>
    val baseUrl = GlobalVariable.BASE_URL // Base URL from a global variable
    private lateinit var buttonBack: ImageView
    private lateinit var buttonUser: ImageButton

    // Create an instance of OkHttpClient to handle HTTP requests
    private val client = OkHttpClient()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_comments)

        // Initialize RecyclerView for displaying comments and set its layout manager
        commentRecyclerView = findViewById(R.id.commentRecyclerView)
        commentRecyclerView.layoutManager = LinearLayoutManager(this)

        // Set up back button functionality
        buttonBack = findViewById(R.id.buttonBack)
        buttonBack.setOnClickListener { onBackPressed() }

        // Set up user options button
        buttonUser = findViewById(R.id.buttonUser)
        buttonUser.setOnClickListener {
            showUserOptions(buttonUser)
        }

        // Fetch the customer ID from shared preferences and load comments if available
        getCustomerId()?.let { fetchCommentsFromApi(it) }
    }

    /**
     * Displays a popup menu for user options such as Home and Logout.
     * @param view The view to anchor the popup menu.
     */
    private fun showUserOptions(view: View) {
        val popupMenu = PopupMenu(this, view)
        val inflater: MenuInflater = popupMenu.menuInflater
        inflater.inflate(R.menu.menu_user_options, popupMenu.menu) // Inflate the user options menu

        popupMenu.setOnMenuItemClickListener { menuItem: MenuItem ->
            when (menuItem.itemId) {
                R.id.menu_home -> {
                    // Navigate to the HomeFragment (main activity)
                    val intent = Intent(this, Main::class.java)
                    startActivity(intent)
                    true
                }

                R.id.menu_logout -> {
                    // Log out the user by clearing shared preferences
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
        popupMenu.show() // Display the popup menu
    }

    /**
     * Clears shared preferences and logs the user out by navigating to the Login/Register activity.
     */
    private fun logoutUser() {
        val sharedPreferences = getSharedPreferences("user_prefs", Context.MODE_PRIVATE)
        val editor = sharedPreferences.edit()
        editor.clear()  // Clears all saved user preferences (e.g., login status, user data)
        editor.apply()  // Apply changes to shared preferences

        // Start the LoginRegisterActivity and clear the activity stack
        val intent = Intent(this, LoginRegisterActivity::class.java)
        intent.flags = Intent.FLAG_ACTIVITY_NEW_TASK or Intent.FLAG_ACTIVITY_CLEAR_TASK
        startActivity(intent)
    }

    /**
     * Fetches comments for the user from the API using the customer ID.
     * @param userId The ID of the customer whose comments will be fetched.
     */
    private fun fetchCommentsFromApi(userId: String) {
        val url = "$baseUrl/comment/user/$userId" // Construct the API URL for fetching comments

        val request = Request.Builder()
            .url(url)
            .build()

        // Asynchronous API call to fetch comments
        client.newCall(request).enqueue(object : Callback {
            override fun onFailure(call: Call, e: IOException) {
                // Log error and display a Toast on failure to fetch comments
                Log.e("CommentsActivity", "Error fetching comments: ${e.message}")
                runOnUiThread {
                    Toast.makeText(
                        this@CommentsActivity,
                        "Failed to load comments",
                        Toast.LENGTH_LONG
                    ).show()
                }
            }

            override fun onResponse(call: Call, response: Response) {
                if (response.isSuccessful) {
                    // Parse the response if successful and update the RecyclerView
                    response.body?.string()?.let { responseBody ->
                        val commentListType = object : TypeToken<List<Comment>>() {}.type
                        val comments = Gson().fromJson<List<Comment>>(responseBody, commentListType)

                        runOnUiThread {
                            commentsList = comments.toMutableList()
                            commentAdapter = CommentAdapter(this@CommentsActivity, commentsList)
                            commentRecyclerView.adapter = commentAdapter

                            // Fetch product details for each comment
                            commentsList.forEach { comment ->
                                fetchProductDetails(comment)
                            }
                        }
                    }
                } else {
                    // Display an error Toast if the API call fails
                    runOnUiThread {
                        Toast.makeText(
                            this@CommentsActivity,
                            "Failed to load comments",
                            Toast.LENGTH_SHORT
                        ).show()
                    }
                }
            }
        })
    }

    /**
     * Fetches product details for each comment based on the product ID.
     * @param comment The comment object containing the product ID.
     */
    private fun fetchProductDetails(comment: Comment) {
        // Construct the API URL for fetching product details
        val productUrl = "$baseUrl/products/${comment.productId}"

        val request = Request.Builder()
            .url(productUrl)
            .build()

        // Asynchronous API call to fetch product details
        client.newCall(request).enqueue(object : Callback {
            override fun onFailure(call: Call, e: IOException) {
                // Log error if the product details cannot be fetched
                Log.e("CommentsActivity", "Error fetching product details: ${e.message}")
            }

            override fun onResponse(call: Call, response: Response) {
                if (response.isSuccessful) {
                    // Parse the product details if the API call is successful
                    response.body?.string()?.let { responseBody ->
                        val product = Gson().fromJson(responseBody, Product::class.java)

                        // Log product details for debugging purposes
                        Log.d("CommentsActivity success", product.toString())

                        // Update the comment object with the product name and image URL
                        comment.productName = product.name
                        comment.productImageUrl = product.imageUrl

                        // Notify the adapter that the data has changed, updating the RecyclerView
                        runOnUiThread {
                            commentAdapter.notifyDataSetChanged()
                        }
                    }
                } else {
                    // Log an error message if the product details cannot be loaded
                    Log.e(
                        "CommentsActivity",
                        "Failed to load product details for productId: ${comment.productId}"
                    )
                }
            }
        })
    }

    /**
     * Retrieves the customer ID from shared preferences.
     * @return The customer ID if present, or null if not.
     */
    private fun getCustomerId(): String? {
        val sharedPreferences = getSharedPreferences("user_prefs", Context.MODE_PRIVATE)
        return sharedPreferences.getString("customer_id", null) // Return customer ID from shared preferences
    }
}
