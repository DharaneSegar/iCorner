/****************************************************************************************
 * File: VendorRanking.cs
 * Description: This file defines the VendorRanking class, which represents a ranking 
 *              given to a vendor by a customer. The class includes properties for 
 *              identifying the vendor and customer, the score given by the customer, 
 *              and a timestamp indicating when the ranking was created.
 ****************************************************************************************/

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EAD.Models
{
    public class VendorRanking
    {
        // MongoDB document identifier for the VendorRanking.
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        // Identifier for the vendor receiving the ranking.
        public string VendorId { get; set; }

        // Identifier for the customer giving the ranking.
        public string CustomerId { get; set; }

        // Score given to the vendor, typically within a defined range (e.g., 1-5).
        public int Score { get; set; }

        // Timestamp indicating when the ranking was created.
        public DateTime CreatedAt { get; set; } = DateTime.Now; // Automatically set to the current local time.
    }
}
