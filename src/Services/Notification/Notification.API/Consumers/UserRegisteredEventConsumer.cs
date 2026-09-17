using MassTransit;
using Notification.API.Services;
using PulseDelivery.Shared.Events;

namespace Notification.API.Consumers;

public class UserRegisteredEventConsumer : IConsumer<UserRegisteredEvent>
{
    private readonly ILogger<UserRegisteredEventConsumer> _logger;
    private readonly IEmailService _emailService;

    public UserRegisteredEventConsumer(ILogger<UserRegisteredEventConsumer> logger, IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
    {
        var user = context.Message;

        _logger.LogInformation($"Message received from RabbitMQ. Sending email to {user.Email}...");

        var subject = "Welcome to PulseDelivery!";
        var htmlBody = $@"
            <div style='font-family: Arial, sans-serif; padding: 20px;'>
                <h2>Hello {user.FirstName},</h2>
                <p>You have successfully registered for the PulseDelivery system.</p>
                <p>We look forward to delivering your orders as quickly as possible!</p>
            </div>";

        await _emailService.SendEmailAsync(user.Email, subject, htmlBody);

        _logger.LogInformation("Welcome email sent successfully.");
    }
}