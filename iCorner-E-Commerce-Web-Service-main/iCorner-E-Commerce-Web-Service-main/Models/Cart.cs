// -----------------------------------------------------------------------
// <summary>
// This class represents a Cart model for the application, which stores 
// information about a user's shopping cart. It includes properties for 
// the cart ID, user ID, list of cart items, and the status of the cart.
// </summary>
// <remarks>
// The Cart class is mapped to a MongoDB document with an ObjectId as the identifier.
// </remarks>
// -----------------------------------------------------------------------

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace EAD.Models
{
    public class Cart
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string UserId { get; set; }
        public List<CartItem> Items { get; set; }
        public bool Status { get; set; }
    }
}
