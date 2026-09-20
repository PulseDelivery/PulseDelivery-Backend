using MassTransit;
using Notification.API.Services;
using PulseDelivery.Shared.Events;

namespace Notification.API.Consumers;

public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedEventConsumer> _logger;
    private readonly IEmailService _emailService;

    public OrderCreatedEventConsumer(
        ILogger<OrderCreatedEventConsumer> logger,
        IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var order = context.Message;

        _logger.LogInformation(
            $"Order created message received from RabbitMQ. Sending email to {order.Email}...");

        var subject = "Your Order Has Been Received! - PulseDelivery";

        var htmlBody = $@"
            <div style='font-family: Arial, sans-serif; padding: 20px; border: 1px solid #eaeaea; border-radius: 8px;'>
                <h2 style='color: #ff6600;'>Your Order Has Been Successfully Received!</h2>
                <p>Hello,</p>
                <p>Your order with the number <strong>{order.OrderId}</strong> has been successfully received by our system and is awaiting confirmation.</p>
                
                <div style='background-color: #f9f9f9; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                    <p style='margin: 0;'><strong>Order Total:</strong> {order.TotalAmount} ₺</p>
                    <p style='margin: 5px 0 0 0;'><strong>Order Date:</strong> {order.CreatedAt:dd.MM.yyyy HH:mm}</p>
                </div>

                <p>We will notify you when the restaurant starts preparing your order.</p>
                <br/>
                <p>Thank you for choosing us!<br/><strong>PulseDelivery Team</strong></p>
            </div>";

        // Use the newly added CustomerEmail field
        await _emailService.SendEmailAsync(
            order.Email,
            subject,
            htmlBody);

        _logger.LogInformation(
            $"Order confirmation email sent successfully for OrderId: {order.OrderId}");
    }
}