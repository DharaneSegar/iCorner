/****************************************************************************************
 * File: LoginModel.cs
 * Description: This file defines the LoginModel class, which is used to capture the 
 *              user's login information such as email and password.
 ****************************************************************************************/

using System;

namespace EAD.Models
{
    // This class is used to represent the login credentials submitted by a user.
    public class LoginModel
    {
        // The email address entered by the user for authentication.
        public string Email { get; set; }

        // The password entered by the user for authentication.
        public string Password { get; set; }
    }
}
