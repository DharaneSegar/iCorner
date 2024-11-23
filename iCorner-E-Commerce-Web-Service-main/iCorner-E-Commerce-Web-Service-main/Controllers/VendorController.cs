/****************************************************************************************
 * File: VendorController.cs
 * Description: This file contains the implementation of the VendorController, which 
 *              handles vendor-related operations such as adding rankings, comments, 
 *              retrieving average rankings, and fetching comments by customer. 
 *              It interacts with VendorService to perform the required actions.
 ****************************************************************************************/

using System.Threading.Tasks;
using EAD.Models;
using EAD.Services;
using Microsoft.AspNetCore.Mvc;

namespace EAD.Controllers
{
    [ApiController]
    [Route("api/vendors")]
    public class VendorController : ControllerBase
    {
        private readonly VendorService _vendorService;

        // Constructor to inject the VendorService for vendor-related operations.
        public VendorController(VendorService vendorService)
        {
            _vendorService = vendorService;
        }

        // POST: api/vendors/{vendorId}/rank
        // This method allows a customer to add a ranking for a specific vendor. 
        // Once submitted, the ranking cannot be modified.
        [HttpPost("{vendorId}/rank")]
        public async Task<IActionResult> AddRanking(string vendorId, [FromBody] AddRankingModel model)
        {
            // Attempt to add the ranking via the VendorService
            var result = await _vendorService.AddRankingAsync(model.CustomerId, vendorId, model.Score);
            if (!result)
            {
                // If the ranking cannot be modified, return a BadRequest response
                return BadRequest(new { message = "Ranking cannot be modified once submitted." });
            }
            return Ok(new { message = "Ranking added successfully." });
        }

        // POST: api/vendors/{vendorId}/comment
        // This method allows a customer to add or update a comment for a specific vendor.
        [HttpPost("{vendorId}/comment")]
        public async Task<IActionResult> AddOrUpdateComment(string vendorId, [FromBody] AddCommentModel model)
        {
            // Add or update the comment via the VendorService
            await _vendorService.AddOrUpdateCommentAsync(model.CustomerId, vendorId, model.Comment);
            return Ok(new { message = "Comment added or updated successfully." });
        }

        // GET: api/vendors/{vendorId}/average-ranking
        // This method retrieves the average ranking for a given vendor.
        [HttpGet("{vendorId}/average-ranking")]
        public async Task<IActionResult> GetAverageRanking(string vendorId)
        {
            // Get the average ranking from the VendorService
            var averageRanking = await _vendorService.GetAverageRankingForVendorAsync(vendorId);
            return Ok(new { averageRanking });
        }

        // GET: api/vendors/{vendorId}/comments/{customerId}
        // This method retrieves the comments left by a specific customer for a given vendor.
        [HttpGet("{vendorId}/comments/{customerId}")]
        public async Task<IActionResult> GetCommentsByCustomer(string vendorId, string customerId)
        {
            // Retrieve the comment via the VendorService
            var comment = await _vendorService.GetCommentAsync(customerId, vendorId);
            if (comment == null)
            {
                // If no comment is found, return a NotFound response
                return NotFound(new { message = "Comment not found." });
            }
            return Ok(comment);
        }
    }
}
