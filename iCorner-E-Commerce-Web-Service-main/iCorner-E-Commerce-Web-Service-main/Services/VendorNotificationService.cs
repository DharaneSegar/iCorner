using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;

namespace EAD.Services
{
    /// <summary>
    /// Service responsible for sending notifications to vendors.
    /// </summary>
    public class VendorNotificationService : IVendorNotificationService
    {
        // SMTP server configuration details
        private readonly string _smtpServer = "smtp.gmail.com";
        private readonly int _smtpPort = 587;
        private readonly string _smtpUser = "nonamenecessary0612@gmail.com";
        private readonly string _smtpPass = "ekbgdpcvlpdiciws";

        /// <summary>
        /// Sends a low stock notification email to a vendor.
        /// </summary>
        /// <param name="vendorId">The vendor's email address.</param>
        /// <param name="productId">The ID of the product with low stock.</param>
        /// <param name="stockQuantity">The current stock quantity.</param>
        public async Task NotifyVendorAsync(string vendorId, string productId, int stockQuantity)
        {
            // Create a new email message
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("iCorner", _smtpUser));
            emailMessage.To.Add(new MailboxAddress("", vendorId)); // Vendor's email
            emailMessage.Subject = "Low Stock Alert";

            // Build the email body content with HTML formatting
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
                <p>Dear Vendor,</p>
                <p>We are writing to inform you that the stock for product <strong>{productId}</strong> is running low.</p>
                <p>Current stock quantity: <strong>{stockQuantity}</strong></p>
                <p>Please take the necessary actions to restock.</p>
                <p>Thank you.</p>
                <p>Best regards,<br>iCorner</p>"
            };
            emailMessage.Body = bodyBuilder.ToMessageBody(); // Set the email body content

            // Connect to SMTP server and send the email
            using (var client = new SmtpClient())
            {
                try
                {
                    // Establish a connection using the configured SMTP settings
                    await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                    // Authenticate using the provided credentials
                    await client.AuthenticateAsync(_smtpUser, _smtpPass);
                    // Send the email message
                    await client.SendAsync(emailMessage);
                }
                catch (Exception ex)
                {
                    // Log or handle the exception if email sending fails
                    Console.WriteLine($"Error sending email: {ex.Message}");
                }
                finally
                {
                    // Ensure the SMTP client disconnects properly
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }
    }
}
