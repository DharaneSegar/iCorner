package com.example.ead.fragments.LoginRegister

import android.content.Intent
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Toast
import androidx.fragment.app.Fragment
import androidx.fragment.app.viewModels
import androidx.navigation.fragment.findNavController
import com.example.ead.R
import com.example.ead.activities.Main
import com.example.ead.databinding.FragmentLoginBinding
import com.example.ead.viewmodel.LoginViewModel

/**
 * Fragment for handling user login.
 * Allows users to enter their credentials and log in to the application.
 */
class LoginFragment : Fragment(R.layout.fragment_login) {

    // View binding for the fragment's layout
    private lateinit var binding: FragmentLoginBinding

    // ViewModel for handling login logic
    private val viewModel: LoginViewModel by viewModels()

    /**
     * Called to create the view hierarchy associated with the fragment.
     *
     * @param inflater The LayoutInflater used to inflate the fragment's view.
     * @param container The parent view that this fragment's UI should be attached to.
     * @param savedInstanceState A Bundle containing the activity's previously saved state, if any.
     * @return The root view of the fragment's layout.
     */
    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        // Inflate the layout using the generated binding class
        binding = FragmentLoginBinding.inflate(inflater)
        return binding.root
    }

    /**
     * Called immediately after onCreateView() to perform initialization.
     *
     * @param view The View returned by onCreateView().
     * @param savedInstanceState A Bundle containing the activity's previously saved state, if any.
     */
    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        // Set up click listener for "Don't have an account?" TextView
        binding.tvDontHaveAccount.setOnClickListener {
            // Navigate to the registration fragment when clicked
            findNavController().navigate(R.id.action_loginFragment_to_registerFragment)
        }

        // Set up click listener for the login button
        binding.buttonLoginLogin.setOnClickListener {
            // Retrieve email and password from input fields
            val email = binding.edEmailLogin.text.toString().trim()
            val password = binding.edPasswordLogin.text.toString().trim()

            // Validate inputs before proceeding
            if (email.isNotEmpty() && password.isNotEmpty()) {
                // Call ViewModel method to log in with email and password
                viewModel.loginWithEmailAndPassword(requireContext(), email, password) {
                    // Navigate to the main screen or dashboard on success
                    val intent = Intent(context, Main::class.java).apply {
                        flags = Intent.FLAG_ACTIVITY_NEW_TASK or Intent.FLAG_ACTIVITY_CLEAR_TASK // Clear previous activities
                    }
                    context?.startActivity(intent)
                }
            } else {
                // Show a toast message if any fields are empty
                Toast.makeText(requireContext(), "Please fill in all fields.", Toast.LENGTH_SHORT).show()
            }
        }
    }
}
