using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EAD.Models
{
    // Represents a product in the inventory system
    public class Product
    {
        // Unique identifier for the product, stored as an ObjectId in MongoDB
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        // Identifier for the vendor associated with the product
        public string? VendorId { get; set; }

        // Name of the product
        public string Name { get; set; }

        // Description of the product
        public string Description { get; set; }

        // Price of the product
        public decimal Price { get; set; }

        // Indicates whether the product is active (true) or inactive (false)
        public bool IsActive { get; set; }

        // Identifier for the category the product belongs to
        public string CategoryId { get; set; }

        // Name of the category associated with the product
        public string? CategoryName { get; set; }

        // Quantity of stock available for the product
        public int StockQuantity { get; set; }

        // Path to the product image
        public string? ImagePath { get; set; }
    }
}
