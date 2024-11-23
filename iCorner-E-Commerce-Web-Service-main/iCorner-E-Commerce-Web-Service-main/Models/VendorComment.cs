/****************************************************************************************
 * File: VendorComment.cs
 * Description: This file defines the VendorComment class, which represents a comment 
 *              made by a customer about a vendor. The class includes properties for 
 *              identifying the vendor and customer, the comment text, and timestamps 
 *              for creation and updates.
 ****************************************************************************************/

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EAD.Models
{
    public class VendorComment
    {
        // MongoDB document identifier for the VendorComment.
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        // Identifier for the vendor this comment is associated with.
        public string VendorId { get; set; }

        // Identifier for the customer who made the comment.
        public string CustomerId { get; set; }

        // The actual comment text provided by the customer.
        public string Comment { get; set; }

        // Timestamp indicating when the comment was created.
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreatedAt { get; set; } = DateTime.Now; // Automatically set to the current local time.

        // Timestamp indicating when the comment was last updated. Nullable in case it hasn't been updated.
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? UpdatedAt { get; set; }
    }
}
