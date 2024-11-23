namespace EAD.Controllers
{
    using EAD.Models;
    using EAD.Services;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    // Route configuration for the Email API controller
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly EmailService _emailService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailController"/> class.
        /// </summary>
        /// <param name="emailService">The email service for sending emails.</param>
        public EmailController(EmailService emailService)
        {
            _emailService = emailService;
        }

        /// <summary>
        /// Sends an email based on the provided email request data.
        /// </summary>
        /// <param name="emailRequest">The email request containing the recipient's email, subject, and message.</param>
        /// <returns>A task representing the asynchronous operation, with the result of the action.</returns>
        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromBody] EmailRequest emailRequest)
        {
            if (string.IsNullOrEmpty(emailRequest.ToEmail) || string.IsNullOrEmpty(emailRequest.Subject) || string.IsNullOrEmpty(emailRequest.Message))
            {
                return BadRequest("Invalid email request data");
            }

            await _emailService.SendEmailAsync(emailRequest.ToEmail, emailRequest.Subject, emailRequest.Message);
            return Ok("Email sent successfully.");
        }
    }
}
