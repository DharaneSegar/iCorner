/****************************************************************************************
 * File: AddRankingModel.cs
 * Description: This file defines the AddRankingModel class, which serves as the data 
 *              model for adding a ranking score provided by a customer for a vendor. 
 *              The model contains the customer ID and the ranking score.
 * ****************************************************************************************/

namespace EAD.Models
{
    // This class defines the model used for adding a ranking score for a vendor.
    public class AddRankingModel
    {
        // Property to hold the ID of the customer providing the ranking.
        public string CustomerId { get; set; }

        // Property to hold the ranking score (between 1 and 5) given by the customer.
        public int Score { get; set; } // Ranking between 1-5
    }
}
