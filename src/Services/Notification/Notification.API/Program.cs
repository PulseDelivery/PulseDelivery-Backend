using MassTransit;
using Notification.API.Consumers;
using Notification.API.Services;

var builder = WebApplication.CreateBuilder(args);

// MassTransit & RabbitMQ Configuration
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<UserRegisteredEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("user-registered-queue", e =>
        {
            e.ConfigureConsumer<UserRegisteredEventConsumer>(context);
        });
    });
});

// Dependency Injection
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddTransient<IEmailService, EmailService>();

var app = builder.Build();

app.Run();