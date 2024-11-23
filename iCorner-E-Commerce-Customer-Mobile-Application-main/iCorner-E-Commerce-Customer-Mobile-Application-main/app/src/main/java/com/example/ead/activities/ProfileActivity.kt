package com.example.ead.activities

import android.annotation.SuppressLint
import android.app.Activity
import android.app.AlertDialog
import android.content.Context
import android.content.Intent
import android.graphics.drawable.Drawable
import android.net.Uri
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
import android.widget.ProgressBar
import android.widget.RelativeLayout
import android.widget.TextView
import android.widget.Toast
import androidx.activity.result.ActivityResultLauncher
import androidx.activity.result.contract.ActivityResultContracts
import androidx.appcompat.app.AppCompatActivity
import androidx.constraintlayout.widget.ConstraintLayout
import androidx.swiperefreshlayout.widget.SwipeRefreshLayout
import com.example.ead.R
import com.example.ead.api.CustomerDeleteResponse
import com.example.ead.api.CustomerUpdateResponse
import com.example.ead.fragments.HomeFragment
import com.example.ead.fragments.LoginRegister.AccountOptionsFragment
import com.example.ead.fragments.LoginRegister.RegisterFragment
import com.example.ead.models.Customer
import com.example.ead.models.LoginRequest
import com.example.ead.util.RetrofitInstance
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response

class ProfileActivity : AppCompatActivity() {

    // UI elements
    private lateinit var swipeRefreshLayout: SwipeRefreshLayout
    private lateinit var loadingProgressBar: ProgressBar
    private lateinit var toolbarLayout: RelativeLayout
    private lateinit var constraintLayout: ConstraintLayout
    private lateinit var profileEditImage: ImageView
    private lateinit var cusAccountProfileImage: ImageView
    private lateinit var editTextFullName: EditText
    private lateinit var editTextEmail: EditText
    private lateinit var editTextPhoneNo: EditText
    private lateinit var cusAccountAddress1: EditText
    private lateinit var btnCusDelete: Button
    private lateinit var cusAccountProfileImageEditFrame: View
    private lateinit var cusAccountButtons: View
    private var selectedImageUri: Uri? = null
    private lateinit var buttonBack: ImageButton
    private lateinit var buttonCart: ImageButton
    private lateinit var clickcartHome: ImageView
    private lateinit var btnCusUpdate: Button
    private lateinit var buttonUser: ImageButton

    @SuppressLint("WrongViewCast")
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_profile)

        // Initialize buttons
        buttonBack = findViewById(R.id.buttonBack)
        clickcartHome = findViewById(R.id.clickcartHome)
        buttonCart = findViewById(R.id.buttonCart)

        // Set up click listener for the cart button
        buttonCart.setOnClickListener {
            startActivity(Intent(this, CartActivity::class.java))
        }

        // Initialize delete account button
        btnCusDelete = findViewById(R.id.btnCusDelete)

        // Handle delete button click
        // Handle delete button click
        btnCusDelete.setOnClickListener {
            // Show a confirmation dialog before proceeding with account deletion
            AlertDialog.Builder(this)
                .setTitle("Confirm Deactivation")
                .setMessage("Are you sure you want to deactivate your account?")
                .setPositiveButton("OK") { dialog, _ ->
                    // Proceed with account deletion when OK is pressed
                    deleteUserAccount()
                    dialog.dismiss() // Close the dialog
                }
                .setNegativeButton("Cancel") { dialog, _ ->
                    // Do nothing if "Cancel" is pressed
                    dialog.dismiss() // Close the dialog
                }
                .show() // Display the alert dialog
        }


        // Initialize user options button
        buttonUser = findViewById(R.id.buttonUser)

        // Show user options menu on button click
        buttonUser.setOnClickListener {
            showUserOptions(buttonUser)
        }

        // Handle back button press
        buttonBack.setOnClickListener { onBackPressed() }

        // Initialize SwipeRefreshLayout and other UI elements
        swipeRefreshLayout = findViewById(R.id.swipeRefreshLayout)
        loadingProgressBar = findViewById(R.id.loadingAccManProgressBar)
        toolbarLayout = findViewById(R.id.cusAccManagementHeading)
        constraintLayout = findViewById(R.id.commonHomeConstraint1)

        // Set refresh listener for SwipeRefreshLayout
        swipeRefreshLayout.setOnRefreshListener { refreshContent() }

        // Initialize profile image view and edit texts
        cusAccountProfileImage = findViewById(R.id.cusAccountProfileImage)
        editTextFullName = findViewById(R.id.editTextFullName2)
        editTextEmail = findViewById(R.id.viewInputEmail2)
        editTextPhoneNo = findViewById(R.id.viewInputPhoneNo2)
        cusAccountAddress1 = findViewById(R.id.cusAccountAddress2)

        // Open full-screen image activity when profile image is clicked
        cusAccountProfileImage.setOnClickListener {
            val fullScreenIntent = Intent(this, FullScreenImageActivity::class.java)
            fullScreenIntent.putExtra("productImage", selectedImageUri)
            startActivity(fullScreenIntent)
        }

        // Initialize update button
        btnCusUpdate = findViewById(R.id.btnCusUpdate)

        // Update user details when the update button is clicked
        btnCusUpdate.setOnClickListener {
            updateUserDetails()
        }

        // Display full email in a dialog on click
        editTextEmail.setOnClickListener {
            val fullText = editTextEmail.text.toString()
            AlertDialog.Builder(this)
                .setTitle("Email")
                .setMessage(fullText)
                .setPositiveButton("OK") { dialog, _ -> dialog.dismiss() }
                .show()
        }

        // Retrieve user details from SharedPreferences
        val sharedPreferences = getSharedPreferences("user_prefs", Context.MODE_PRIVATE)
        val customerName = sharedPreferences.getString("customer_name", "")
        val customerEmail = sharedPreferences.getString("customer_email", "")
        val customerMobileNumber = sharedPreferences.getString("customer_mobileNumber", "")
        val customerAddress = sharedPreferences.getString("customer_address", "")

        // Populate the EditText fields with retrieved data
        editTextFullName.setText(customerName)
        editTextEmail.setText(customerEmail)
        editTextPhoneNo.setText(customerMobileNumber)
        cusAccountAddress1.setText(customerAddress)
    }

    // Method to display user options in a popup menu
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
        popupMenu.show() // Show the popup menu
    }

    // Method to clear shared preferences and log out the user
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

    // Method to delete the user account
    private fun deleteUserAccount() {
        // Get user email from shared preferences
        val sharedPreferences = getSharedPreferences("user_prefs", Context.MODE_PRIVATE)
        val customerEmail = sharedPreferences.getString("customer_email", "")

        // Check if the email is valid
        if (customerEmail.isNullOrEmpty()) {
            Toast.makeText(this, "Error: No email found", Toast.LENGTH_SHORT).show()
            return
        }

        // Call the API using Retrofit
        val call = RetrofitInstance.Customerapi.deactivateCustomer(customerEmail)

        // Handle the API response
        call.enqueue(object : Callback<CustomerDeleteResponse> {
            override fun onResponse(
                call: Call<CustomerDeleteResponse>,
                response: Response<CustomerDeleteResponse>
            ) {
                if (response.isSuccessful) {
                    val responseBody = response.body()

                    if (responseBody != null && responseBody.status == "Success") {
                        // Show success message and redirect or update UI
                        Toast.makeText(
                            this@ProfileActivity,
                            "Account deactivated successfully.",
                            Toast.LENGTH_SHORT
                        ).show()

                        // Navigate to the login screen after account deletion
                        val intent = Intent(this@ProfileActivity, LoginRegisterActivity::class.java)
                        startActivity(intent)
                        finish() // Close the ProfileActivity
                    } else {
                        Toast.makeText(
                            this@ProfileActivity,
                            "Failed to delete account.",
                            Toast.LENGTH_SHORT
                        ).show()
                    }
                } else {
                    Toast.makeText(
                        this@ProfileActivity,
                        "Error: ${response.code()}",
                        Toast.LENGTH_SHORT
                    ).show()
                }
            }

            override fun onFailure(call: Call<CustomerDeleteResponse>, t: Throwable) {
                // Handle failure
                Toast.makeText(this@ProfileActivity, "Error: ${t.message}", Toast.LENGTH_SHORT)
                    .show()
            }
        })
    }

    // Method to update user details
    private fun updateUserDetails() {
        // Get values from EditTexts
        val fullName = editTextFullName.text.toString()
        val email = editTextEmail.text.toString()
        val phoneNoString = editTextPhoneNo.text.toString()
        val phoneNo: Int? = phoneNoString.toIntOrNull() // Convert phone number string to Int
        val address = cusAccountAddress1.text.toString()
        val sharedPreferences = getSharedPreferences("user_prefs", Context.MODE_PRIVATE)
        val password = sharedPreferences.getString("customer_password", "")
        val userId = sharedPreferences.getString("customer_id", "")

        // Check for validation if required (e.g. non-empty fields)
        val updateRequest = Customer(fullName, email, password, phoneNo, address)
        Log.d("update request", updateRequest.toString()) // Log update request

        // Make API call to update customer details
        val call = RetrofitInstance.Customerapi.updateCustomer(userId, updateRequest)
        Log.d("updated call", call.toString()) // Log updated call

        // Execute API call
        if (call != null) {
            call.enqueue(object : Callback<CustomerUpdateResponse> {
                override fun onResponse(
                    call: Call<CustomerUpdateResponse>,
                    response: Response<CustomerUpdateResponse>
                ) {
                    if (response.isSuccessful) {
                        // Handle successful response
                        val updatedCustomer = response.body()

                        if (updatedCustomer != null) {
                            // Show success message and save updated details to SharedPreferences
                            val editor = sharedPreferences.edit()
                            editor.putString("customer_name", updatedCustomer.fullName)
                            editor.putString("customer_email", updatedCustomer.email)
                            editor.putString(
                                "customer_mobileNumber",
                                updatedCustomer.mobileNumber.toString()
                            )
                            editor.putString("customer_address", updatedCustomer.address)

                            // Apply changes to SharedPreferences
                            editor.apply()

                            Log.d("ProfileUpdate", "Updated data saved to local storage.")
                            Toast.makeText(
                                this@ProfileActivity,
                                "Update successful: ${updatedCustomer.fullName}",
                                Toast.LENGTH_SHORT
                            ).show()

                            // Log success
                            Log.d("ProfileUpdate", "Update Successful: $updatedCustomer")
                        }
                    } else {
                        // Handle failure
                        Toast.makeText(
                            this@ProfileActivity,
                            "Failed to update. Code: ${response.code()}",
                            Toast.LENGTH_SHORT
                        ).show()
                        Log.e(
                            "ProfileUpdate",
                            "Update failed with response code: ${response.code()}"
                        )
                    }
                }

                override fun onFailure(call: Call<CustomerUpdateResponse>, t: Throwable) {
                    // Handle error
                    Toast.makeText(
                        this@ProfileActivity,
                        "Error: ${t.message}",
                        Toast.LENGTH_SHORT
                    ).show()
                    Log.e("ProfileUpdate", "Error: ${t.message}")
                }
            })
        }
    }

    // Method to refresh the content of the activity
    private fun refreshContent() {
        loadingProgressBar.visibility = View.VISIBLE
        swipeRefreshLayout.isRefreshing = false
    }

    // Method to toggle edit mode for profile information
    private fun toggleEditMode() {
        toggleViewVisibility(profileEditImage)
        toggleViewVisibility(cusAccountButtons)
        toggleViewVisibility(cusAccountProfileImageEditFrame)

        toggleViewPair(editTextFullName, findViewById<TextView>(R.id.editTextFullName2))
        toggleViewPair(editTextEmail, findViewById<TextView>(R.id.viewInputEmail2))
        toggleViewPair(editTextPhoneNo, findViewById<TextView>(R.id.viewInputPhoneNo2))
        toggleViewPair(cusAccountAddress1, findViewById<TextView>(R.id.cusAccountAddress2))
    }

    // Method to toggle the visibility of a view
    private fun toggleViewVisibility(view: View) {
        view.visibility = if (view.visibility == View.GONE) View.VISIBLE else View.GONE
    }

    // Method to toggle visibility between edit and display views
    private fun toggleViewPair(editView: View, displayView: View) {
        if (editView.visibility == View.GONE) {
            editView.visibility = View.VISIBLE
            displayView.visibility = View.GONE
        } else {
            editView.visibility = View.GONE
            displayView.visibility = View.VISIBLE
        }
    }

    // Override back pressed action
    override fun onBackPressed() {
        Log.d("TAG", "Debug message")
        if (supportFragmentManager.backStackEntryCount > 0) {
            Log.d("TAG", "If Debug message")
            supportFragmentManager.popBackStack() // Go back to previous fragment
        } else {
            Log.d("TAG", "Else Debug message")
            super.onBackPressed() // Close activity if no fragments are in the stack
        }
    }
}
