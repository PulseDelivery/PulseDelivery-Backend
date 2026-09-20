using MassTransit;
using Notification.API.Consumers;
using Notification.API.Services;

var builder = WebApplication.CreateBuilder(args);

// MassTransit & RabbitMQ Configuration
builder.Services.AddMassTransit(x =>
{
	// Register Consumer
    x.AddConsumer<UserRegisteredEventConsumer>();
	// Order Created Consumer
	x.AddConsumer<OrderCreatedEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ConfigureEndpoints(context);    
    });
});

// Dependency Injection
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddTransient<IEmailService, EmailService>();

var app = builder.Build();

app.Run();