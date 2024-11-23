package com.example.ead.models

data class Comment(
    val id: String?= null,
    val userId: String,
    val productId: String,
    val vendorId: String,
    val rating: Int,
    val comments: String,
    var productName: String? = null, // To store the product name
    var productImageUrl: String? = null // To store the product image URL
)

