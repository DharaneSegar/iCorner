package com.example.ead.adapters

import android.content.Context
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.ImageView
import android.widget.RatingBar
import android.widget.TextView
import androidx.recyclerview.widget.RecyclerView
import com.example.ead.R
import com.example.ead.models.Comment
import com.squareup.picasso.Picasso

// Adapter class for displaying comments in a RecyclerView
class CommentAdapter(
    private val context: Context, // Context for inflating views and accessing resources
    private val comments: List<Comment> // List of Comment objects to display
) : RecyclerView.Adapter<CommentAdapter.CommentViewHolder>() {

    // Called when RecyclerView needs a new ViewHolder of the given type to represent an item
    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): CommentViewHolder {
        // Inflate the comment_item layout and create a ViewHolder
        val view = LayoutInflater.from(context).inflate(R.layout.comment_item, parent, false)
        return CommentViewHolder(view) // Return the newly created ViewHolder
    }

    // Called by RecyclerView to display the data at the specified position
    override fun onBindViewHolder(holder: CommentViewHolder, position: Int) {
        // Get the comment at the current position
        val comment = comments[position]

        // Set product name; use "Unknown Product" if productName is null
        holder.itemNameTextView.text = comment.productName ?: "Unknown Product"
        // Set the vendor ID for the comment
        holder.itemVendorTextView.text = comment.vendorId
        // Set the actual comment text
        holder.itemCommentTextView.text = comment.comments
        // Set the rating value in the RatingBar
        holder.itemRatingBar.rating = comment.rating.toFloat()

        // Load product image using Picasso
        Picasso.get()
            .load(comment.productImageUrl) // Load image from the URL provided in comment
            .placeholder(R.drawable.logo_dark) // Placeholder image while loading
            .error(R.drawable.logo_dark) // Image to show if loading fails
            .into(holder.itemImageView) // Set the loaded image to ImageView
    }

    // Return the total number of comments in the list
    override fun getItemCount(): Int {
        return comments.size
    }

    // ViewHolder class to hold references to each view in the comment layout
    class CommentViewHolder(itemView: View) : RecyclerView.ViewHolder(itemView) {
        // Initialize the views for each comment item
        val itemImageView: ImageView = itemView.findViewById(R.id.itemImageView) // ImageView for product image
        val itemNameTextView: TextView = itemView.findViewById(R.id.itemNameTextView) // TextView for product name
        val itemVendorTextView: TextView = itemView.findViewById(R.id.itemVendorTextView) // TextView for vendor ID
        val itemCommentTextView: TextView = itemView.findViewById(R.id.itemCommentTextView) // TextView for comment text
        val itemRatingBar: RatingBar = itemView.findViewById(R.id.ratingBar) // RatingBar for product rating
    }
}
