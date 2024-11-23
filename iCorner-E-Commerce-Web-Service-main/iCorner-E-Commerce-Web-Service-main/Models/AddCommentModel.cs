/****************************************************************************************
 * File: AddCommentModel.cs
 * Description: This file defines the AddCommentModel class, which serves as the data 
 *              model for adding or updating comments related to vendors. 
 *              The model contains the customer ID and the comment text.
 ****************************************************************************************/

using System;

namespace EAD.Models
{
    // This class defines the model used for adding or updating a comment.
    public class AddCommentModel
    {
        // Property to hold the ID of the customer leaving the comment.
        public string CustomerId { get; set; }

        // Property to hold the comment text provided by the customer.
        public string Comment { get; set; }
    }
}
