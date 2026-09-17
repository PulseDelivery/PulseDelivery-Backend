namespace Notification.API.Services;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string htmlBody);
}