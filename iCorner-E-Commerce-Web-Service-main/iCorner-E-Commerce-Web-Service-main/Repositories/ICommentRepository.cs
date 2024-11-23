/****************************************************************************************
 * File: ICommentRepository.cs
 * Description: This file defines the ICommentRepository interface, which outlines the 
 *              methods required for managing comments in the application. The repository 
 *              includes methods to add, delete, and retrieve comments based on Product ID, 
 *              Vendor ID, and User ID, as well as a method for calculating the average rating.
 ****************************************************************************************/

using EAD.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EAD.Repositories
{
    public interface ICommentRepository
    {
        /// <summary>
        /// Fetch comments by Product ID.
        /// </summary>
        /// <param name="productId">The ID of the product.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of comments for the specified product.</returns>
        Task<IEnumerable<Comment>> GetCommentsByProductId(string productId);

        /// <summary>
        /// Add a new comment.
        /// </summary>
        /// <param name="comment">The comment object to add.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the added comment.</returns>
        Task<Comment> AddComment(Comment comment);

        /// <summary>
        /// Delete a comment by its ID.
        /// </summary>
        /// <param name="commentId">The ID of the comment to delete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the comment was successfully deleted.</returns>
        Task<bool> DeleteComment(string commentId);

        /// <summary>
        /// Fetch comments by Vendor ID.
        /// </summary>
        /// <param name="vendorId">The ID of the vendor.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of comments for the specified vendor.</returns>
        Task<IEnumerable<Comment>> GetCommentsByVendorId(string vendorId);

        /// <summary>
        /// Fetch comments by User ID.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of comments for the specified user.</returns>
        Task<IEnumerable<Comment>> GetCommentsByUserId(string userId);

        /// <summary>
        /// Get the average rating for a product based on its comments.
        /// </summary>
        /// <param name="productId">The ID of the product.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the average rating for the product, or null if no comments are available.</returns>
        Task<double?> GetAverageRatingByProductId(string productId);
    }
}
