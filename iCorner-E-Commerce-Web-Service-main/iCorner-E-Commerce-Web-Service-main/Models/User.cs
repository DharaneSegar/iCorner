/****************************************************************************************
 * File: User.cs
 * Description: This file defines the User class, which represents the user entity 
 *              in the system. The class contains various properties related to 
 *              user details, such as Username, Email, Password, etc. The class also
 *              includes data validation annotations.
 ****************************************************************************************/

using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class User
{
	// MongoDB document identifier for the User.
	[BsonId]
	[BsonRepresentation(BsonType.ObjectId)]
	public string? Id { get; set; }

	// Username of the user. This field is required.
	[Required]
	public string Username { get; set; }

	// User's email address. This field is required and must be a valid email format.
	[Required]
	[EmailAddress]
	public string Email { get; set; }

	// User's password. This field is required.
	[Required]
	public string Password { get; set; }

	// User's mobile number. This field is required.
	[Required]
	public string MobileNumber { get; set; }

	// User's physical address. This field is required.
	[Required]
	public string Address { get; set; }

	// The role of the user, such as Admin, Vendor, or CSR. This field is required.
	[Required]
	public string Role { get; set; }

	// Indicates whether the user is active or not. Default is true.
	public bool IsActive { get; set; } = true;
}
