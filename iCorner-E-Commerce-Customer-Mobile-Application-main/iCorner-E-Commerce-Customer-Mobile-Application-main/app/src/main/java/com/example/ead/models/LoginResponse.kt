package com.example.ead.models

data class LoginResponse(
    val message: String,
    val customer: CustomerResponse
)

data class CustomerResponse(
    val id: String,
    val email: String,
    val password : String,
    val fullName: String,
    val mobileNumber: Long,
    val address: String,
    val isActive: Boolean
)
