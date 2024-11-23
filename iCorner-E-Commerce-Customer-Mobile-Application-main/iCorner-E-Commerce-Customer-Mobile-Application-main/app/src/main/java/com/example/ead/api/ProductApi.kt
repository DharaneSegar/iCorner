package com.example.ead.api

import com.example.ead.models.Product
import retrofit2.Call
import retrofit2.http.GET

// Interface defining API endpoints related to products
interface ProductApi {

    /**
     * Fetches the list of available products.
     *
     * This method makes a GET request to the "products/available" endpoint
     * and retrieves a list of products. The response is wrapped in a Call object.
     *
     * @return A Call object that will return a List of Product objects when executed.
     */
    @GET("products/available")
    fun getProducts(): Call<List<Product>>
}
