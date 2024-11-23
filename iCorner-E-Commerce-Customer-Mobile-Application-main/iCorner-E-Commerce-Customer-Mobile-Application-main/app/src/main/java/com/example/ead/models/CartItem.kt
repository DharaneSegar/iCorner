package com.example.ead.models

data class CartItem(
    val id: String? = null,
    val productId: String?,
    val productName: String?,
    var quantity: Int,
    val price: Double,
    val imagePath: String?,
    val status : String


)






