/****************************************************************************************
 * File: VendorRepository.cs
 * Description: This file defines the VendorRepository class, which provides methods for 
 *              managing vendor rankings and comments in the application. It includes 
 *              functionalities to add rankings, retrieve rankings, calculate vendor scores, 
 *              and add or update comments for specific vendors. 
 ****************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EAD.Models;
using MongoDB.Driver;

namespace EAD.Repositories
{
    public class VendorRepository
    {
        private readonly IMongoCollection<VendorRanking> _ranking; // Collection for vendor rankings
        private readonly IMongoCollection<VendorComment> _comment; // Collection for vendor comments

        // Constructor that initializes the vendor rankings and comments collections from the MongoDB database
        public VendorRepository(IMongoDatabase database)
        {
            _ranking = database.GetCollection<VendorRanking>("Rankings");
            _comment = database.GetCollection<VendorComment>("Comments");
        }

        /// <summary>
        /// Adds a new ranking for a vendor.
        /// </summary>
        /// <param name="ranking">The ranking object to add.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task AddRankingAsync(VendorRanking ranking)
        {
            await _ranking.InsertOneAsync(ranking);
        }

        /// <summary>
        /// Retrieves a vendor ranking for a specific customer and vendor.
        /// </summary>
        /// <param name="customerId">The ID of the customer.</param>
        /// <param name="vendorId">The ID of the vendor.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the ranking if found; otherwise, null.</returns>
        public async Task<VendorRanking?> GetRankingAsync(string customerId, string vendorId)
        {
            var ranking = await _ranking.Find(r => r.CustomerId == customerId && r.VendorId == vendorId).FirstOrDefaultAsync();
            return ranking; // Return the found ranking or null if not found
        }

        /// <summary>
        /// Calculates the average score for a vendor based on their rankings.
        /// </summary>
        /// <param name="vendorId">The ID of the vendor.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the average score or 0 if no rankings exist.</returns>
        public async Task<double> GetVendorScoreAsync(string vendorId)
        {
            var ranking = await _ranking.Find(r => r.VendorId == vendorId).ToListAsync();

            if (ranking.Count == 0) return 0; // Return 0 if no rankings are found

            return ranking.Average(r => r.Score); // Return the average score
        }

        /// <summary>
        /// Adds a new comment for a vendor or updates an existing comment.
        /// </summary>
        /// <param name="comment">The comment object to add or update.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task AddOrUpdateCommentAsync(VendorComment comment)
        {
            var existingComment = await _comment.Find(c => c.CustomerId == comment.CustomerId && c.VendorId == comment.VendorId).FirstOrDefaultAsync();

            if (existingComment == null)
            {
                await _comment.InsertOneAsync(comment); // Insert new comment if it does not exist
            }
            else
            {
                // Update existing comment
                existingComment.Comment = comment.Comment;
                existingComment.UpdatedAt = DateTime.Now; // Update the timestamp
                await _comment.ReplaceOneAsync(c => c.Id == existingComment.Id, existingComment);
            }
        }

        /// <summary>
        /// Retrieves a comment for a specific customer and vendor.
        /// </summary>
        /// <param name="customerId">The ID of the customer.</param>
        /// <param name="vendorId">The ID of the vendor.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the comment if found; otherwise, null.</returns>
        public async Task<VendorComment?> GetCommentAsync(string customerId, string vendorId)
        {
            var comment = await _comment.Find(r => r.CustomerId == customerId && r.VendorId == vendorId).FirstOrDefaultAsync();
            return comment; // Return the found comment or null if not found
        }
    }
}
