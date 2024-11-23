package com.example.ead.models

data class CartResponse(
    val id: String,
    val userId: String,
    val items: List<CartItem>
)