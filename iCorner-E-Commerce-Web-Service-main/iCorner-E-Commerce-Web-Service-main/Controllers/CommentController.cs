/****************************************************************************************
 * File: CommentController.cs
 * Description: This file contains the implementation of the CommentController, which 
 *              manages operations related to comments and ratings for products and vendors.
 *              It allows CRUD operations on comments and retrieves the 
 *              average rating for a product.
 ****************************************************************************************/

using EAD.Models;
using EAD.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EAD.Controllers
{
    // CommentController is responsible for handling operations related to comments and 
    // ratings, including retrieving comments for products, vendors, and users, as well 
    // as adding and deleting comments. The controller also calculates the average rating 
    // for a product.
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepository;

        // Constructor that injects the comment repository to manage comment-related operations.
        public CommentController(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        // GET: api/comment/product/{productId}/rating
        // Method to retrieve the average rating for a specific product based on its productId.
        // If no ratings are available, returns a NotFound response.
        [HttpGet("product/{productId}/rating")]
        public async Task<ActionResult<double?>> GetAverageRatingByProductId(string productId)
        {
            var averageRating = await _commentRepository.GetAverageRatingByProductId(productId); // Fetches the average rating for the product

            if (averageRating == null)
            {
                return NotFound("No ratings available for this product."); // If no ratings found, return NotFound response
            }

            return Ok(averageRating); // Return the average rating in the response
        }

        // GET: api/comment/product/{productId}
        // Method to retrieve all comments for a specific product by its productId.
        [HttpGet("product/{productId}")]
        public async Task<ActionResult<IEnumerable<Comment>>> GetCommentsByProductId(string productId)
        {
            var comments = await _commentRepository.GetCommentsByProductId(productId); // Fetches all comments for the product
            return Ok(comments); // Return the list of comments in the response
        }

        // GET: api/comment/vendor/{vendorId}
        // Method to retrieve all comments for a specific vendor by vendorId.
        [HttpGet("vendor/{vendorId}")]
        public async Task<ActionResult<IEnumerable<Comment>>> GetCommentsByVendorId(string vendorId)
        {
            var comments = await _commentRepository.GetCommentsByVendorId(vendorId); // Fetches all comments for the vendor
            return Ok(comments); // Return the list of vendor comments in the response
        }

        // POST: api/comment
        // Method to add a new comment to the database. Accepts a Comment object in the request body.
        // After the comment is created, returns a CreatedAtAction response pointing to the product's comments.
        [HttpPost]
        public async Task<ActionResult<Comment>> AddComment([FromBody] Comment comment)
        {
            var newComment = await _commentRepository.AddComment(comment); // Adds the new comment to the repository
            return CreatedAtAction(nameof(GetCommentsByProductId), new { productId = newComment.ProductId }, newComment); // Return the created comment with a 201 status
        }

        // DELETE: api/comment/{id}
        // Method to delete a comment based on its ID.
        // Returns a NoContent response if deletion is successful, or NotFound if the comment doesn't exist.
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(string id)
        {
            var deleted = await _commentRepository.DeleteComment(id); // Attempts to delete the comment by its ID
            if (deleted)
            {
                return NoContent(); // Return NoContent status if successful
            }
            return NotFound(); // Return NotFound if the comment doesn't exist
        }

        // GET: api/comment/user/{userId}
        // Method to retrieve all comments made by a specific user based on the userId.
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Comment>>> GetCommentsByUserId(string userId)
        {
            var comments = await _commentRepository.GetCommentsByUserId(userId); // Fetches all comments by the user
            return Ok(comments); // Return the list of user comments in the response
        }
    }
}
