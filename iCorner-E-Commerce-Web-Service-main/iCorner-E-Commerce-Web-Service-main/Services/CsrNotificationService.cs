/****************************************************************************************
 * File: CsrNotificationService.cs
 * Description: This file contains the CsrNotificationService class, which implements 
 *              the ICsrNotificationService interface. It provides functionality to send 
 *              email notifications to Customer Service Representatives (CSRs) when a 
 *              new customer registers. The service uses MailKit for SMTP communication.
 ****************************************************************************************/

using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EAD.Services
{
    public class CsrNotificationService : ICsrNotificationService
    {
        // SMTP server configuration
        private readonly string _smtpServer = "smtp.gmail.com"; // SMTP server address
        private readonly int _smtpPort = 587; // SMTP server port for TLS
        private readonly string _smtpUser = "nonamenecessary0612@gmail.com"; // SMTP user email
        private readonly string _smtpPass = "ekbgdpcvlpdiciws"; // SMTP user password

        /// <summary>
        /// Notifies CSRs about a new customer registration via email.
        /// </summary>
        /// <param name="customerEmail">The email of the new customer.</param>
        /// <param name="csrEmails">A list of CSR emails to notify.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task NotifyCsrsAboutNewCustomerAsync(string customerEmail, List<string> csrEmails)
        {
            // Iterate through the list of CSR emails
            foreach (var csrEmail in csrEmails)
            {
                // Create a new email message
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress("iCorner", _smtpUser)); // Sender's address
                emailMessage.To.Add(new MailboxAddress("CSR Team", csrEmail)); // Add each CSR email

                emailMessage.Subject = "New Customer Registered"; // Email subject

                // Build the HTML body of the email
                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = $@"
                    <p>Dear CSR,</p>
                    <p>A new customer has registered with the email: {customerEmail}.</p>
                    <p>Please take the necessary actions.</p>
                    <p>Best regards,<br>iCorner Team</p>"
                };
                emailMessage.Body = bodyBuilder.ToMessageBody(); // Set the email body

                // Send the email using the SMTP client
                using (var client = new SmtpClient())
                {
                    try
                    {
                        // Connect to the SMTP server
                        await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                        // Authenticate with the SMTP server
                        await client.AuthenticateAsync(_smtpUser, _smtpPass);
                        // Send the email message
                        await client.SendAsync(emailMessage);
                    }
                    catch (Exception ex)
                    {
                        // Log the error if sending fails
                        Console.WriteLine($"Error sending email to {csrEmail}: {ex.Message}");
                    }
                    finally
                    {
                        // Disconnect from the SMTP server
                        await client.DisconnectAsync(true);
                    }
                }
            }
        }
    }
}
