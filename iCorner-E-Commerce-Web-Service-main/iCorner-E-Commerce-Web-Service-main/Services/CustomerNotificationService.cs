/****************************************************************************************
 * File: CustomerNotificationService.cs
 * Description: This file contains the CustomerNotificationService class, which implements 
 *              the ICustomerNotificationService interface. It provides functionality to send 
 *              email notifications to customers regarding account activation and deactivation.
 *              The service uses MailKit for SMTP communication.
 ****************************************************************************************/

using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace EAD.Services
{
    public class CustomerNotificationService : ICustomerNotificationService
    {
        // SMTP server configuration
        private readonly string _smtpServer = "smtp.gmail.com"; // SMTP server address
        private readonly int _smtpPort = 587; // SMTP server port for TLS
        private readonly string _smtpUser = "nonamenecessary0612@gmail.com"; // SMTP user email
        private readonly string _smtpPass = "ekbgdpcvlpdiciws"; // SMTP user password

        /// <summary>
        /// Notifies the customer of successful account activation via email.
        /// </summary>
        /// <param name="customerEmail">The email of the customer.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task NotifyCustomerActivationAsync(string customerEmail)
        {
            // Create a new email message for activation notification
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("iCorner", _smtpUser)); // Sender's address
            emailMessage.To.Add(new MailboxAddress("", customerEmail)); // Customer's email
            emailMessage.Subject = "Account Activation Successful"; // Email subject

            // Build the HTML body of the email
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
                <p>Dear Customer,</p>
                <p>We are pleased to inform you that your account has been successfully activated.</p>
                <p>You can now log in and enjoy our services.</p>
                <p>Thank you for choosing iCorner.</p>
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
                    Console.WriteLine($"Error sending email: {ex.Message}");
                }
                finally
                {
                    // Disconnect from the SMTP server
                    await client.DisconnectAsync(true);
                    client.Dispose(); // Explicitly dispose the client
                }
            }
        }

        /// <summary>
        /// Notifies the customer of successful account deactivation via email.
        /// </summary>
        /// <param name="customerEmail">The email of the customer.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task NotifyCustomerDeactivationAsync(string customerEmail)
        {
            // Create a new email message for deactivation notification
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("iCorner", _smtpUser)); // Sender's address
            emailMessage.To.Add(new MailboxAddress("", customerEmail)); // Customer's email
            emailMessage.Subject = "Account Deactivation Successful"; // Email subject

            // Build the HTML body of the email
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
                <p>Dear Customer,</p>
                <p>We are pleased to inform you that your account has been successfully deactivated.</p>
                <p>Please continue using our services by requesting reactivation.</p>
                <p>Thank you for choosing iCorner.</p>
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
                    Console.WriteLine($"Error sending email: {ex.Message}");
                }
                finally
                {
                    // Disconnect from the SMTP server
                    await client.DisconnectAsync(true);
                    client.Dispose(); // Explicitly dispose the client
                }
            }
        }
    }
}
