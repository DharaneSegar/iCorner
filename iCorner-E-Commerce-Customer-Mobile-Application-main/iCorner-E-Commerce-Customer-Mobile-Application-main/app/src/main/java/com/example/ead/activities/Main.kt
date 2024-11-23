package com.example.ead.activities

import android.content.Context
import android.content.Intent
import android.os.Bundle
import android.util.Log
import android.view.MenuInflater
import android.view.MenuItem
import android.view.View
import android.widget.ImageButton
import android.widget.ImageView
import android.widget.PopupMenu
import android.widget.TextView
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import androidx.drawerlayout.widget.DrawerLayout
import androidx.fragment.app.FragmentTransaction
import com.example.ead.R
import com.example.ead.fragments.HomeFragment
import com.google.android.material.navigation.NavigationView

class Main : AppCompatActivity() {

    // Declare UI components such as DrawerLayout, Buttons, ImageView, and NavigationView
    private lateinit var drawerLayout: DrawerLayout
    private lateinit var buttonDrawerToggle: ImageButton
    private lateinit var buttonDrawerMenuRight: ImageButton
    private lateinit var navigationView: NavigationView
    private lateinit var clickcartLogo: ImageView
    private lateinit var buttonUser: ImageButton

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.main)

        // Initialize views and components
        drawerLayout = findViewById(R.id.navigatorDrawer)
        buttonDrawerToggle = findViewById(R.id.buttonDrawerToggle)
        buttonDrawerMenuRight = findViewById(R.id.buttonDrawerMenuRight)
        navigationView = findViewById(R.id.navigationView)
        clickcartLogo = findViewById(R.id.clickcart_logo)

        // Retrieve user information (username and email) from SharedPreferences
        val sharedPreferences = getSharedPreferences("user_prefs", MODE_PRIVATE)
        val username = sharedPreferences.getString("customer_name", "User Name") // Default value if null
        val email = sharedPreferences.getString("customer_email", "User Email") // Default value if null

        // Load HomeFragment by default when the activity is first created (only if savedInstanceState is null)
        if (savedInstanceState == null) {
            val transaction: FragmentTransaction = supportFragmentManager.beginTransaction()
            transaction.replace(R.id.mainContent, HomeFragment())  // Display HomeFragment as the default screen
            transaction.commit()
        }

        // Set click listener for the drawer toggle button (opens the drawer)
        buttonDrawerToggle.setOnClickListener {
            drawerLayout.open()  // Open the navigation drawer
        }

        // Set click listener for the user icon to show user options
        buttonUser = findViewById(R.id.buttonUser)
        buttonUser.setOnClickListener {
            Log.d("MainActivity", "User icon clicked")
            showUserOptions(buttonUser)  // Display user options popup menu
        }

        // Handle click event on the app logo to reload HomeFragment
        clickcartLogo.setOnClickListener {
            val transaction: FragmentTransaction = supportFragmentManager.beginTransaction()
            transaction.replace(R.id.mainContent, HomeFragment())  // Replace current fragment with HomeFragment
            transaction.commit()
        }

        // Access the header of the navigation drawer to display user info (name and email)
        val headerView = navigationView.getHeaderView(0)
        val userImage: ImageView = headerView.findViewById(R.id.userImage)
        val textUsername: TextView = headerView.findViewById(R.id.textUserName)
        val textUserEmail: TextView = headerView.findViewById(R.id.textUserEmail)

        // Set the retrieved username and email in the drawer header views
        textUsername.text = username
        textUserEmail.text = email

        // Set a click listener on the user image to display a Toast with the username
        userImage.setOnClickListener {
            Toast.makeText(this, textUsername.text, Toast.LENGTH_SHORT).show()
        }

        // Set click listener for the right-side drawer button to open CartActivity
        buttonDrawerMenuRight.setOnClickListener {
            // Launch CartActivity when the cart icon is clicked
            val intent = Intent(this, CartActivity::class.java)
            startActivity(intent)
        }

        // Handle navigation drawer item selection
        navigationView.setNavigationItemSelectedListener { item: MenuItem ->
            val itemId = item.itemId

            // Switch case for handling different navigation menu item selections
            when (itemId) {
                R.id.navHome -> {
                    // Load HomeFragment when "Home" is selected
                    val transaction: FragmentTransaction = supportFragmentManager.beginTransaction()
                    transaction.replace(R.id.mainContent, HomeFragment())
                    transaction.commit()
                }

                R.id.navProfile -> {
                    // Launch ProfileActivity when "Profile" is selected
                    val intent = Intent(this, ProfileActivity::class.java)
                    startActivity(intent)
                }

                R.id.navCart -> {
                    // Launch CartActivity when "Cart" is selected
                    val intent = Intent(this, CartActivity::class.java)
                    startActivity(intent)
                }

                R.id.navMyOrders -> {
                    // Launch OrdersActivity when "My Orders" is selected
                    val intent = Intent(this, OrdersActivity::class.java)
                    startActivity(intent)
                }

                R.id.navMyComments -> {
                    // Launch CommentsActivity when "My Comments" is selected
                    val intent = Intent(this, CommentsActivity::class.java)
                    startActivity(intent)
                }

                R.id.navLogout -> {
                    // Handle user logout and clear saved preferences

                    // Remove user data from SharedPreferences
                    val sharedPreferences = getSharedPreferences("user_prefs", Context.MODE_PRIVATE)
                    val editor = sharedPreferences.edit()
                    editor.clear()  // Clears all the saved user data
                    editor.apply()  // Apply the changes to SharedPreferences

                    // Launch LoginRegisterActivity and clear the back stack
                    val intent = Intent(this, LoginRegisterActivity::class.java)
                    intent.flags = Intent.FLAG_ACTIVITY_NEW_TASK or Intent.FLAG_ACTIVITY_CLEAR_TASK
                    startActivity(intent)
                    finish()  // Close the current activity
                }
            }

            drawerLayout.close()  // Close the drawer after selecting a menu item
            true
        }
    }

    /**
     * Shows a popup menu with user options (e.g., Home, Logout).
     * @param view The view to anchor the popup menu.
     */
    private fun showUserOptions(view: View) {
        val popupMenu = PopupMenu(this, view)
        val inflater: MenuInflater = popupMenu.menuInflater
        inflater.inflate(R.menu.menu_user_options, popupMenu.menu)  // Inflate the popup menu layout

        popupMenu.setOnMenuItemClickListener { menuItem: MenuItem ->
            when (menuItem.itemId) {
                R.id.menu_home -> {
                    // Navigate to the HomeFragment when "Home" is selected
                    val intent = Intent(this, Main::class.java)
                    startActivity(intent)
                    true
                }

                R.id.menu_logout -> {
                    // Logout the user when "Logout" is selected
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
        popupMenu.show()  // Display the popup menu
    }

    /**
     * Clears user data from SharedPreferences and logs the user out.
     */
    private fun logoutUser() {
        val sharedPreferences = getSharedPreferences("user_prefs", Context.MODE_PRIVATE)
        val editor = sharedPreferences.edit()
        editor.clear()  // Clears all the saved data (user session, preferences)
        editor.apply()  // Apply the changes to SharedPreferences

        // Start the LoginRegisterActivity and clear the activity stack
        val intent = Intent(this, LoginRegisterActivity::class.java)
        intent.flags = Intent.FLAG_ACTIVITY_NEW_TASK or Intent.FLAG_ACTIVITY_CLEAR_TASK
        startActivity(intent)
    }
}
