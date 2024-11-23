package com.example.ead.activities

import com.example.ead.R


import android.content.Intent
import android.os.Bundle
import android.widget.ImageView
import androidx.appcompat.app.AppCompatActivity
import com.squareup.picasso.Picasso;

class FullScreenImageActivity : AppCompatActivity() {

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_full_screen_image)

        val fullScreenImageView: ImageView = findViewById(R.id.fullScreenImageView)
        val closeButton: ImageView = findViewById(R.id.closeButton)

        // Get image URL from intent
        val intent: Intent = intent
        val productImage: String? = intent.getStringExtra("productImage")

        // Load image using Picasso
        Picasso.get()
            .load(productImage)
            .into(fullScreenImageView)

        // Set click listener for the close button
        closeButton.setOnClickListener { finish() }
    }
}
