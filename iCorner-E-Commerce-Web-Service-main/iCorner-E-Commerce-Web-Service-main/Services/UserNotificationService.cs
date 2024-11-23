using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;
using EAD.Repositories;
public class UserNotificationService : IUserNotificationService
{
    private readonly string _smtpServer = "smtp.gmail.com";
    private readonly int _smtpPort = 587;
    private readonly string _smtpUser = "nonamenecessary0612@gmail.com"; // SMTP email
    private readonly string _smtpPass = "ekbgdpcvlpdiciws"; // SMTP password
    private readonly UserRepository _userRepository;
    public UserNotificationService(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task SendRegistrationEmailAsync(User user)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress("iCorner", _smtpUser));
        emailMessage.To.Add(new MailboxAddress(user.Username, user.Email));
        emailMessage.Subject = "Welcome to iCorner - Registration Successful";

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = $@"
                <p>Dear {user.Username},</p>
                <p>Thank you for registering with iCorner.</p>
                <p>Your login details are as follows:</p>
                <ul>
                    <li><strong>Username:</strong> {user.Email}</li>
                    <li><strong>Password:</strong> {user.Password}</li>
                    <li><strong>Role:</strong> {user.Role}</li>
                </ul>
                <p>You can log in and change your password and role in the user settings.</p>
                <p>Best regards,<br>iCorner Team</p>"
        };
        emailMessage.Body = bodyBuilder.ToMessageBody();

        using (var client = new SmtpClient())
        {
            try
            {
                await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_smtpUser, _smtpPass);
                await client.SendAsync(emailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }
    }
    public async Task SendAccountActivationEmailAsync(string userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user == null) throw new Exception("User not found");

        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress("iCorner", _smtpUser));
        emailMessage.To.Add(new MailboxAddress(user.Username, user.Email));
        emailMessage.Subject = "Account Activated";

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = $@"
            <p>Dear {user.Username},</p>
            <p>Your account has been successfully activated. You can now log in using your credentials.</p>
            <p>If you wish to change your password, you can do so after logging in.</p>
            <p>Best regards,<br>iCorner Team</p>"
        };

        emailMessage.Body = bodyBuilder.ToMessageBody();

        using (var client = new SmtpClient())
        {
            await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_smtpUser, _smtpPass);
            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);
        }
    }

    public async Task SendAccountDeactivationEmailAsync(string userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user == null) throw new Exception("User not found");

        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress("iCorner", _smtpUser));
        emailMessage.To.Add(new MailboxAddress(user.Username, user.Email));
        emailMessage.Subject = "Account Deactivated";

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = $@"
            <p>Dear {user.Username},</p>
            <p>Your account has been deactivated. You will no longer be able to log in.</p>
            <p>If you think this was a mistake, please contact support.</p>
            <p>Best regards,<br>iCorner Team</p>"
        };

        emailMessage.Body = bodyBuilder.ToMessageBody();

        using (var client = new SmtpClient())
        {
            await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_smtpUser, _smtpPass);
            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);
        }
    }
}
