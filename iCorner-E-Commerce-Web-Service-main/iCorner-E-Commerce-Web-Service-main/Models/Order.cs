using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace EAD.Models
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string CustomerId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string Cart { get; set; }

        public decimal TotalPrice { get; set; }

        public string ShippingAddress { get; set; }

        public DateTime OrderDate { get; set; }

        public string Status { get; set; }

        public string PaymentStatus { get; set; }

        public string? Notes { get; set; }
    }
}