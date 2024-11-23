/****************************************************************************************
 * File: Comment.cs
 * Description: This file defines the Comment class, which represents user comments on
 *              products or vendors. It includes fields for user, product, vendor IDs,
 *              optional rating, and comment content. The class uses MongoDB attributes
 *              to map to a MongoDB collection.
 * ****************************************************************************************/
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace EAD.Models
{
    // This class represents a user's comment on a product or vendor, 
    // with optional rating and comment content.
    public class Comment
    {
        // The unique identifier for the comment, mapped to MongoDB ObjectId.
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        // The unique identifier of the user who made the comment.
        public string UserId { get; set; }

        // The unique identifier of the product related to the comment.
        public string ProductId { get; set; }

        // The unique identifier of the vendor related to the comment.
        public string VendorId { get; set; }

        // Optional rating given by the user (1 to 5 scale, nullable).
        public int? Rating { get; set; }

        // Optional text content of the user's comment.
        public string? Comments { get; set; }
    }
}
