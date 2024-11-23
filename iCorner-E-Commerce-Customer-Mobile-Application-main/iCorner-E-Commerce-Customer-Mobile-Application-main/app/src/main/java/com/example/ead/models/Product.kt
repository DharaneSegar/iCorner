package com.example.ead.models



data class Product(
    val id: String,
    val vendorId: String,
    val name: String,
    val description: String,
    val price: Double,
    val isActive: Boolean,
    val categoryId: String,
    val categoryName: String,
    val stockQuantity: Int,
    val imagePath: String,
    var rating: Double?

) {
    val imageUrl: String
        get() = "http://192.168.109.81/iCorner$imagePath"


}





