package com.example.ead.fragments.LoginRegister

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.fragment.app.Fragment
import androidx.navigation.fragment.findNavController
import com.example.ead.R
import com.example.ead.databinding.FragmentIntroductionBinding

/**
 * Fragment that serves as the introduction screen of the application.
 * It provides an entry point for the user to start navigating the app.
 */
class IntroductionFragment : Fragment(R.layout.fragment_introduction) {
    // View binding for the fragment's layout
    private lateinit var binding: FragmentIntroductionBinding

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
        binding = FragmentIntroductionBinding.inflate(inflater)
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

        // Set up click listener for the start button
        binding.buttonStart.setOnClickListener {
            // Navigate to the account options fragment when the start button is clicked
            findNavController().navigate(R.id.action_introductionFragment_to_accountOptionsFragment)
        }
    }
}
