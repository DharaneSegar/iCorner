package com.example.ead.api

import com.example.ead.models.Customer
import com.example.ead.models.LoginRequest
import com.example.ead.models.LoginResponse
import retrofit2.Call
import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.DELETE
import retrofit2.http.POST
import retrofit2.http.PUT
import retrofit2.http.Path

// Interface defining API endpoints for customer operations
interface CustomerApi {

    /**
     * Registers a new customer.
     *
     * @param customer The Customer object containing the registration details.
     * @return A Response wrapping the registered Customer object,
     *         which may include the customer's ID and other information.
     */
    @POST("customer/register")
    suspend fun registerCustomer(@Body customer: Customer): Response<Customer>

    /**
     * Authenticates a customer login.
     *
     * @param loginRequest The LoginRequest object containing email and password.
     * @return A Response wrapping the LoginResponse object, which contains
     *         authentication status and token if successful.
     */
    @POST("customer/login")
    suspend fun loginCustomer(@Body loginRequest: LoginRequest): Response<LoginResponse>

    /**
     * Updates an existing customer's details.
     *
     * @param id The unique identifier of the customer to be updated.
     * @param customer The Customer object containing updated information.
     * @return A Call wrapping the CustomerUpdateResponse,
     *         which includes updated customer information.
     */
    @PUT("customer/update/{id}")
    fun updateCustomer(
        @Path("id") id: String?,
        @Body customer: Customer
    ): Call<CustomerUpdateResponse>

    /**
     * Deactivates a customer account based on the email provided.
     *
     * @param email The email address of the customer to deactivate.
     * @return A Call wrapping the CustomerDeleteResponse, which contains
     *         status information about the deletion process.
     */
    @DELETE("customer/delete/{email}")
    fun deactivateCustomer(@Path("email") email: String): Call<CustomerDeleteResponse>
}

// Data class to represent the response from updating a customer
data class CustomerUpdateResponse(
    val id: String,
    val email: String,
    val fullName: String,
    val mobileNumber: String,
    val address: String,
    val isActive: Boolean
)

// Data class to represent the response from deleting a customer
data class CustomerDeleteResponse(
    val email: String,
    val status: String
)
