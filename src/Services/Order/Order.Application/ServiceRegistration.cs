using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Order.Application.Behaviors;
using System.Reflection;

namespace Order.Application;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // 1. Automatically discover and register FluentValidation validators
        services.AddValidatorsFromAssembly(assembly);

        // 2. Register MediatR and all handlers
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            
            // 3. Register the Decorator (Pipeline Behavior) - Every request passes through this first
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        return services;
    }
}