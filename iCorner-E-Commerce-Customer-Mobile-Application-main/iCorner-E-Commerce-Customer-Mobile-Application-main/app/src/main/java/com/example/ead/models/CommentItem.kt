package com.example.ead.models


data class CommentItem(
    val id: String,
    val userId: String,
    val productId: String,
    val vendorId: String,
    val rating: Int,
    val comments: String,
    var productName: String? = null, // Add product name
    var productImageUrl: String? = null // Add product image URL
)
