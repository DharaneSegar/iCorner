package com.example.ead.fragments.LoginRegister

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.fragment.app.Fragment
import androidx.navigation.fragment.findNavController
import com.example.ead.R
import com.example.ead.databinding.FragmentAccountOptionsBinding

/**
 * Fragment that provides options for the user to either log in or register an account.
 */
class AccountOptionsFragment : Fragment(R.layout.fragment_account_options) {
    // View binding for the fragment's layout
    private lateinit var binding: FragmentAccountOptionsBinding

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
    ): View? {
        // Inflate the layout using the generated binding class
        binding = FragmentAccountOptionsBinding.inflate(inflater)
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

        // Set up click listeners for the login and register buttons
        binding.buttonLoginAccountOptions.setOnClickListener {
            // Navigate to the login fragment when the login button is clicked
            findNavController().navigate(R.id.action_accountOptionsFragment_to_loginFragment)
        }

        binding.buttonRegisterAccountOptions.setOnClickListener {
            // Navigate to the register fragment when the register button is clicked
            findNavController().navigate(R.id.action_accountOptionsFragment_to_registerFragment)
        }
    }
}
