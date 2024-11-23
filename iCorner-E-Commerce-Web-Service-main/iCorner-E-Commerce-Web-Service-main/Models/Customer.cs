// -----------------------------------------------------------------------
// <summary>
// This class represents a Customer model for the application, which 
// stores information about a customer. It includes properties for 
// the customer's ID, email, password, full name, mobile number, 
// address, and account status.
// </summary>
// <remarks>
// The Customer class is mapped to a MongoDB document with an ObjectId as the identifier.
// </remarks>
// -----------------------------------------------------------------------

namespace EAD.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Customer
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string Email { get; set; }

    public string? Password { get; set; }

    public string FullName { get; set; }

    public int MobileNumber { get; set; }

    public string Address { get; set; }

    public bool IsActive { get; set; }
}
