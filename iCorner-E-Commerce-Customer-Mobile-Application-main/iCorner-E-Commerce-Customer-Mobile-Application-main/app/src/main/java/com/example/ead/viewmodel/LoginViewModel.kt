package com.example.ead.viewmodel

import android.content.Context
import android.util.Log
import android.widget.Toast
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.example.ead.models.LoginRequest
import com.example.ead.util.Resource
import com.example.ead.util.RetrofitInstance
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.launch
import retrofit2.HttpException
import retrofit2.Response
import java.io.IOException

class LoginViewModel : ViewModel() {

    private val _login = MutableStateFlow<Resource<String>>(Resource.Loading())
    val login: StateFlow<Resource<String>> = _login

    fun loginWithEmailAndPassword(
        context: Context,
        email: String,
        password: String,
        onSuccess: () -> Unit
    ) {
        viewModelScope.launch {
            _login.value = Resource.Loading()

            try {
                val loginRequest = LoginRequest(email, password)
                val response = RetrofitInstance.Customerapi.loginCustomer(loginRequest)
                Log.i("LoginViewModel", response.toString())


                handleResponse(response, context, onSuccess)
            } catch (e: IOException) {
                _login.value = Resource.Error("Network error: Please check your connection.")
                Toast.makeText(context, "Network error: Please check your connection.", Toast.LENGTH_LONG).show()
            } catch (e: HttpException) {
                val errorBody = e.response()?.errorBody()?.string() ?: "Unknown error occurred"
                Log.e("LoginViewModel", errorBody)
                _login.value = Resource.Error("Server error: $errorBody")
                Toast.makeText(context, "Server error: $errorBody", Toast.LENGTH_LONG).show()
            }
        }
    }

    private fun handleResponse(
        response: Response<com.example.ead.models.LoginResponse>,
        context: Context,
        onSuccess: () -> Unit
    ) {
        if (response.isSuccessful) {
            val responseBody = response.body()

            when (responseBody?.message) {
                "Login successful." -> {
                    _login.value = Resource.Success("Login successful!")
                    Toast.makeText(context, "Login successful!", Toast.LENGTH_SHORT).show()
                    onSuccess()
                    Log.d("customer response", responseBody?.customer.toString())

                    // Create SharedPreferences instance and store responseBody
                    saveLoginDataToSharedPreferences(context, responseBody)


                }
                "Invalid email address." -> {
                    _login.value = Resource.Error("Invalid email address.")
                    Toast.makeText(context, "Invalid email address.", Toast.LENGTH_LONG).show()
                }
                "Incorrect password." -> {
                    _login.value = Resource.Error("Incorrect password.")
                    Toast.makeText(context, "Incorrect password.", Toast.LENGTH_LONG).show()
                }
                "Account is deactivated." -> {
                    _login.value = Resource.Error("Account is deactivated.")
                    Toast.makeText(context, "Account is deactivated. Wait until the account is activated", Toast.LENGTH_LONG).show()
                }
                else -> {
                    _login.value = Resource.Error("Unexpected error: ${responseBody?.message}")
                    Toast.makeText(context, "Unexpected error: ${responseBody?.message}", Toast.LENGTH_LONG).show()
                }
            }
        } else {
            when (response.code()) {
                401 -> {
                    _login.value = Resource.Error("Unauthorized: Invalid email or password.")
                    Toast.makeText(context, "Unauthorized: Invalid email or password.", Toast.LENGTH_LONG).show()
                }
                404 -> {
                    _login.value = Resource.Error("Not Found: Email address does not exist.")
                    Toast.makeText(context, "Not Found: Email address does not exist.", Toast.LENGTH_LONG).show()
                }
                else -> {
                    _login.value = Resource.Error("Login failed: ${response.message()}")
                    Toast.makeText(context, "Login failed: ${response.message()}", Toast.LENGTH_LONG).show()
                }
            }
        }
    }

    private fun saveLoginDataToSharedPreferences(context: Context, responseBody: com.example.ead.models.LoginResponse) {
        val sharedPreferences = context.getSharedPreferences("user_prefs", Context.MODE_PRIVATE)
        val editor = sharedPreferences.edit()

        // Assuming `responseBody.customer` contains relevant user details
        // Here you would extract whatever data you need from `responseBody`
        editor.putString("customer_name", responseBody.customer?.fullName) // Example, adjust as per your model
        editor.putString("customer_email", responseBody.customer?.email) // Example
        editor.putString("customer_mobileNumber", responseBody.customer?.mobileNumber.toString())
        editor.putString("customer_address", responseBody.customer?.address) // Example
        editor.putString("customer_id", responseBody.customer?.id) // Example
        editor.putString("customer_password", responseBody.customer?.password) // Example




        // Example if you have a token

        editor.apply() // Use apply() for asynchronous saving
    }

}