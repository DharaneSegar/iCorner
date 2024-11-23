package com.example.ead.fragments.LoginRegister

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Toast
import androidx.fragment.app.Fragment
import androidx.fragment.app.viewModels
import androidx.navigation.fragment.findNavController
import com.example.ead.R
import com.example.ead.databinding.FragmentRegisterBinding
import com.example.ead.models.Customer
import com.example.ead.viewmodel.RegisterViewModel

/**
 * Fragment for handling user registration.
 * Allows users to input their details to create a new account.
 */
class RegisterFragment : Fragment() {

    // View binding for the fragment's layout
    private lateinit var binding: FragmentRegisterBinding

    // ViewModel for handling registration logic
    private val viewModel: RegisterViewModel by viewModels()

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
        binding = FragmentRegisterBinding.inflate(inflater)
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

        // Set up click listener for "Do you have an account?" TextView
        binding.tvDoYouHaveAccount.setOnClickListener {
            // Navigate to the login fragment when clicked
            findNavController().navigate(R.id.action_registerFragment_to_loginFragment)
        }

        // Set up click listener for the register button
        binding.buttonRegisterRegister.setOnClickListener {
            // Retrieve input data from the registration form
            val fullName = binding.edFullNameRegister.text.toString().trim()
            val email = binding.edEmailRegister.text.toString().trim()
            val password = binding.edPasswordRegister.text.toString().trim()
            val confirmPassword = binding.edRePasswordRegister.text.toString().trim()
            val mobileNumber = binding.edMobileNumberRegister.text.toString().trim()
            val address = binding.edAddressRegister.text.toString().trim()

            // Validate inputs before proceeding
            if (viewModel.validateInputs(
                    requireContext(),
                    email,
                    password,
                    confirmPassword,
                    mobileNumber
                )
            ) {
                // Create a Customer object from the input data
                val customer = Customer(
                    fullName = fullName,
                    email = email,
                    password = password,
                    mobileNumber = mobileNumber.toInt(),
                    address = address
                )

                // Call ViewModel method to register the customer
                viewModel.createAccountWithEmailAndPassword(requireContext(), customer) {
                    // Navigate to the login page on success
                    findNavController().navigate(R.id.action_registerFragment_to_loginFragment)
                }
            }
        }
    }
}
