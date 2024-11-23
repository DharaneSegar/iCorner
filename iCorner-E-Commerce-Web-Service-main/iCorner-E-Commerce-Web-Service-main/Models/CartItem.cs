// -----------------------------------------------------------------------
// <summary>
// This class represents a CartItem model for the application, which 
// stores information about an item in a shopping cart. It includes 
// properties for the item's ID, product ID, product name, quantity, 
// price, and an optional image path.
// </summary>
// <remarks>
// The CartItem class is mapped to a MongoDB document with an ObjectId as the identifier.
// </remarks>
// -----------------------------------------------------------------------

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace EAD.Models
{
    public class CartItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string? ImagePath { get; set; }
        public string Status { get; set; }
    }
}
