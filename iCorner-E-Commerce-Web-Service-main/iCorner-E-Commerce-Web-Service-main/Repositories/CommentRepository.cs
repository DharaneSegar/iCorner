/****************************************************************************************
 * File: CommentRepository.cs
 * Description: This file defines the CommentRepository class, which handles data 
 *              operations related to comments in the application. It includes methods 
 *              for adding, deleting, and fetching comments by various criteria such as 
 *              Product ID, Vendor ID, and User ID. Additionally, it provides a method 
 *              to calculate the average rating for a product based on its comments.
 ****************************************************************************************/

using MongoDB.Driver;
using EAD.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EAD.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        // MongoDB collection for storing comments
        private readonly IMongoCollection<Comment> _comments;

        // Constructor to initialize the CommentRepository with the MongoDB database
        public CommentRepository(IMongoDatabase database)
        {
            _comments = database.GetCollection<Comment>("Comments"); // Fetch the Comments collection
        }

        // Fetch comments by Product ID
        public async Task<IEnumerable<Comment>> GetCommentsByProductId(string productId)
        {
            return await _comments.Find(comment => comment.ProductId == productId).ToListAsync(); // Retrieve comments for the specified product
        }

        // Add a new comment
        public async Task<Comment> AddComment(Comment comment)
        {
            await _comments.InsertOneAsync(comment); // Insert the comment into the collection
            return comment; // Return the added comment
        }

        // Delete a comment by ID
        public async Task<bool> DeleteComment(string commentId)
        {
            var result = await _comments.DeleteOneAsync(comment => comment.Id == commentId); // Delete the comment
            return result.DeletedCount > 0; // Return true if a comment was deleted
        }

        // Fetch comments by Vendor ID
        public async Task<IEnumerable<Comment>> GetCommentsByVendorId(string vendorId)
        {
            return await _comments.Find(comment => comment.VendorId == vendorId).ToListAsync(); // Retrieve comments for the specified vendor
        }

        // Retrieve comments by User ID
        public async Task<IEnumerable<Comment>> GetCommentsByUserId(string userId)
        {
            return await _comments.Find(comment => comment.UserId == userId).ToListAsync(); // Retrieve comments for the specified user
        }

        // Calculate the average rating for a product by its comments
        public async Task<double?> GetAverageRatingByProductId(string productId)
        {
            var comments = await _comments.Find(comment => comment.ProductId == productId).ToListAsync(); // Fetch comments for the product

            // Check if there are any comments for the product
            if (comments.Count == 0)
            {
                return null; // No ratings available
            }

            // Calculate the average rating
            double averageRating = comments.Average(comment => comment.Rating ?? 0); // Handle potential null ratings

            return averageRating; // Return the calculated average rating
        }
    }
}
