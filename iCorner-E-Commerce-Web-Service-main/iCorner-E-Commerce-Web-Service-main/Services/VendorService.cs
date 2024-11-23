using System.Threading.Tasks;
using EAD.Models;
using EAD.Repositories;

namespace EAD.Services
{
    /// <summary>
    /// Provides services for managing vendor-related operations, including adding rankings,
    /// managing comments, and retrieving vendor scores.
    /// </summary>
    /// <remarks>
    /// This service interacts with the VendorRepository to perform CRUD operations on vendor
    /// rankings and comments. It ensures that rankings are only added once per customer-vendor
    /// relationship and allows for the addition or updating of comments. It also computes
    /// average rankings for vendors.
    /// </remarks>
    public class VendorService
    {
        private readonly VendorRepository _repository;

        public VendorService(VendorRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Adds a ranking for a vendor from a customer. 
        /// A ranking cannot be modified once created.
        /// </summary>
        /// <param name="customerId">The ID of the customer.</param>
        /// <param name="vendorId">The ID of the vendor.</param>
        /// <param name="score">The score to assign to the vendor.</param>
        /// <returns>True if the ranking was added; otherwise, false.</returns>
        public async Task<bool> AddRankingAsync(string customerId, string vendorId, int score)
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(customerId) || string.IsNullOrWhiteSpace(vendorId) || score < 0)
            {
                throw new ArgumentException("Invalid input parameters.");
            }

            var existingRanking = await _repository.GetRankingAsync(customerId, vendorId);
            if (existingRanking != null)
            {
                return false; // Ranking cannot be modified once created
            }

            var ranking = new VendorRanking
            {
                CustomerId = customerId,
                VendorId = vendorId,
                Score = score
            };

            await _repository.AddRankingAsync(ranking);
            return true;
        }

        /// <summary>
        /// Adds or updates a comment for a vendor from a customer.
        /// </summary>
        /// <param name="customerId">The ID of the customer.</param>
        /// <param name="vendorId">The ID of the vendor.</param>
        /// <param name="comment">The comment to be added or updated.</param>
        /// <returns>True if the operation was successful.</returns>
        public async Task<bool> AddOrUpdateCommentAsync(string customerId, string vendorId, string comment)
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(customerId) || string.IsNullOrWhiteSpace(vendorId) || string.IsNullOrWhiteSpace(comment))
            {
                throw new ArgumentException("Customer ID, Vendor ID, and comment cannot be empty.");
            }

            var customerComment = new VendorComment
            {
                CustomerId = customerId,
                VendorId = vendorId,
                Comment = comment
            };

            await _repository.AddOrUpdateCommentAsync(customerComment);
            return true;
        }

        /// <summary>
        /// Retrieves the average ranking for a vendor.
        /// </summary>
        /// <param name="vendorId">The ID of the vendor.</param>
        /// <returns>The average score for the vendor.</returns>
        public async Task<double> GetAverageRankingForVendorAsync(string vendorId)
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(vendorId))
            {
                throw new ArgumentException("Vendor ID cannot be empty.");
            }

            return await _repository.GetVendorScoreAsync(vendorId);
        }

        /// <summary>
        /// Retrieves a comment for a specific customer and vendor.
        /// </summary>
        /// <param name="customerId">The ID of the customer.</param>
        /// <param name="vendorId">The ID of the vendor.</param>
        /// <returns>The comment associated with the customer and vendor.</returns>
        public async Task<VendorComment?> GetCommentAsync(string customerId, string vendorId)
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(customerId) || string.IsNullOrWhiteSpace(vendorId))
            {
                throw new ArgumentException("Customer ID and Vendor ID cannot be empty.");
            }

            return await _repository.GetCommentAsync(customerId, vendorId);
        }
    }
}
