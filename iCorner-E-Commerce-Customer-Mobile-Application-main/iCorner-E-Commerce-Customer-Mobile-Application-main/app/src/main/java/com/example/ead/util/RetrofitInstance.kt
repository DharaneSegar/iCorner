package com.example.ead.util

import com.example.ead.GlobalVariable
import com.example.ead.api.CustomerApi
import com.example.ead.api.ProductApi
import okhttp3.OkHttpClient
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory

// Singleton object to provide Retrofit instances for API calls
object RetrofitInstance {
    // Base URL retrieved from global variables
    val baseUrl = GlobalVariable.BASE_URL

    // Append a trailing slash to the base URL for proper API endpoint formation
    private val BASE_URL = "$baseUrl/"
    // Create an instance of the logging interceptor for logging HTTP requests and responses
    private val loggingInterceptor = LoggingInterceptor()

    // Configure OkHttpClient with the logging interceptor
    private val client = OkHttpClient.Builder()
        .addInterceptor(loggingInterceptor) // Add the logging interceptor to the client
        .build()

    // Lazy initialization of CustomerApi Retrofit instance
    val Customerapi: CustomerApi by lazy {
        Retrofit.Builder()
            .baseUrl(BASE_URL) // Set the base URL for the API
            .addConverterFactory(GsonConverterFactory.create()) // Use Gson for JSON conversion
            .build() // Build the Retrofit instance
            .create(CustomerApi::class.java) // Create the CustomerApi interface instance
    }

    // Lazy initialization of ProductApi Retrofit instance
    val Productapi: ProductApi by lazy {
        Retrofit.Builder()
            .baseUrl(BASE_URL) // Set the base URL for the API
            .addConverterFactory(GsonConverterFactory.create()) // Use Gson for JSON conversion
            .build() // Build the Retrofit instance
            .create(ProductApi::class.java) // Create the ProductApi interface instance
    }


}
