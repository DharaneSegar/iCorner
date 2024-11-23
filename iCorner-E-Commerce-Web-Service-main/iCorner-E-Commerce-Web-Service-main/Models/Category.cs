using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EAD.Models
{
    // Represents a category in the inventory system
    public class Category
    {
        // Unique identifier for the category, stored as an ObjectId in MongoDB
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        // Name of the category
        public string? Name { get; set; }

        // Indicates whether the category is active (true) or inactive (false)
        public bool? IsActive { get; set; }
    }
}
