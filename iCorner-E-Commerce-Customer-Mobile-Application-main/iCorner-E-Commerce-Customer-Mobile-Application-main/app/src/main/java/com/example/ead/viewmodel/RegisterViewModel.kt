package com.example.ead.viewmodel

import android.content.Context
import android.widget.Toast
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.example.ead.models.Customer
import com.example.ead.util.Resource
import com.example.ead.util.RetrofitInstance
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.launch
import retrofit2.HttpException
import java.io.IOException
import android.util.Log
import com.example.ead.util.EmailService

class RegisterViewModel : ViewModel() {

    private val _register = MutableStateFlow<Resource<Customer>>(Resource.Loading())
    val register: StateFlow<Resource<Customer>> = _register

    // Validate the inputs and show corresponding toast messages
    fun validateInputs(
        context: Context,
        email: String,
        password: String,
        confirmPassword: String,
        mobileNumber: String
    ): Boolean {
        if (!android.util.Patterns.EMAIL_ADDRESS.matcher(email).matches()) {
            Toast.makeText(context, "Invalid email format.", Toast.LENGTH_SHORT).show()
            return false
        }

        if (!isValidPassword(password)) {
            Toast.makeText(
                context,
                "Password must be at least 8 characters, contain a lowercase letter, uppercase letter, number, and special character.",
                Toast.LENGTH_LONG
            ).show()
            return false
        }

        if (password != confirmPassword) {
            Toast.makeText(context, "Passwords do not match.", Toast.LENGTH_SHORT).show()
            return false
        }

        if (mobileNumber.length != 10) {
            Toast.makeText(context, "Mobile number must be exactly 10 digits.", Toast.LENGTH_SHORT).show()
            return false
        }

        return true
    }

    private fun isValidPassword(password: String): Boolean {
        if (password.length < 8) return false
        var hasLower = false
        var hasUpper = false
        var hasNumber = false
        var hasSpecial = false

        for (char in password) {
            when {
                Character.isLowerCase(char) -> hasLower = true
                Character.isUpperCase(char) -> hasUpper = true
                Character.isDigit(char) -> hasNumber = true
                !Character.isLetterOrDigit(char) -> hasSpecial = true
            }
        }
        return hasLower && hasUpper && hasNumber && hasSpecial
    }

    // Function to send data to the backend using Retrofit and handle navigation on success
    fun createAccountWithEmailAndPassword(
        context: Context,
        customer: Customer,
        onSuccess: () -> Unit
    ) {
        viewModelScope.launch {
            _register.value = Resource.Loading()

            try {
                val response = RetrofitInstance.Customerapi.registerCustomer(customer)

                if (response.isSuccessful) {
                    _register.value = Resource.Success(response.body()!!)
                    Toast.makeText(
                        context,
                        "Registration successful! Admin will activate your account shortly",
                        Toast.LENGTH_SHORT
                    ).show()
                    var subject = "Account Activation Pending"
                    var message = "Dear ${customer.fullName},\n\n" +
                            "Thank you for registering. Your account is pending activation. " +
                            "You will receive a confirmation email once your account has been activated.\n\n" +
                            "Best regards,\niCorner"
                    customer.email?.let {
                        EmailService(context, it, subject, message).execute()
                    }

                    onSuccess()

                } else {
                    val errorMessage = response.errorBody()?.string() ?: "Unknown error occurred"
                    Log.e("RegisterViewModel", "Error: $errorMessage")
                    _register.value = Resource.Error("Registration failed: $errorMessage")
                    Toast.makeText(context, "Registration failed: $errorMessage", Toast.LENGTH_LONG).show()
                }
            } catch (e: IOException) {
                _register.value = Resource.Error("Network error: Please check your connection.")
                Toast.makeText(context, "Network error: Please check your connection.", Toast.LENGTH_LONG).show()
            } catch (e: HttpException) {
                val errorMessage = e.response()?.errorBody()?.string() ?: "Unknown server error"
                Log.e("RegisterViewModel", "HTTP Error: $errorMessage")
                _register.value = Resource.Error("Server error: $errorMessage")
                Toast.makeText(context, "Server error: $errorMessage", Toast.LENGTH_LONG).show()
            }
        }
    }
}
