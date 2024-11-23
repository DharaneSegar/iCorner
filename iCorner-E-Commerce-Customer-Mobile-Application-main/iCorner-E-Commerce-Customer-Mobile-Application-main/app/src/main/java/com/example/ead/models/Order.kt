package com.example.ead.models


data class Order(
    val id: String? = null,
    val customerId: String,
    val cart: String,
    val vendorId: String? = null,
    val totalPrice: Double,
    val shippingAddress: String,
    val orderDate: String,
    var status: String = "Processing", // Default status
    val paymentStatus: String,
    val notes: String
)

