public interface IUserNotificationService
{
    Task SendRegistrationEmailAsync(User user);
    Task SendAccountActivationEmailAsync(string userId);
    Task SendAccountDeactivationEmailAsync(string userId);
}
