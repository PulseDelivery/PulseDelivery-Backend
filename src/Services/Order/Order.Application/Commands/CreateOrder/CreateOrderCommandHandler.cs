using MassTransit; 
using MediatR;

using Order.Application.Strategies;
using Order.Domain.Entities;
using Order.Domain.Repositories;

using PulseDelivery.Shared.DTOs;
using PulseDelivery.Shared.Events; 

namespace Order.Application.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, ResponseDto<Guid>>
{
    private readonly IOrderRepository _repository;
    private readonly IPublishEndpoint _publishEndpoint; // RabbitMQ publisher

    public CreateOrderCommandHandler(
        IOrderRepository repository,
        IPublishEndpoint publishEndpoint)
    {
        _repository = repository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<ResponseDto<Guid>> Handle(
        CreateOrderCommand request,
        CancellationToken cancellationToken)
    {
        // 1. STRATEGY PATTERN: Calculate the discount
        IDiscountStrategy discountStrategy = request.DiscountType switch
        {
            "Student" => new StudentDiscountStrategy(),
            "Percentage" when request.DiscountValue.HasValue =>
                new PercentageDiscountStrategy(request.DiscountValue.Value),
            _ => new NoDiscountStrategy()
        };

        decimal finalAmount = discountStrategy.ApplyDiscount(request.CartTotal);

        // 2. STATE PATTERN: Create the order (automatically starts in Pending state)
        var order = OrderEntity.Create(
            request.CustomerId,
            request.RestaurantId,
            finalAmount);

        // 3. DATABASE: Save the order to PostgreSQL
        await _repository.AddAsync(order);

        // 4. RABBITMQ: Publish an event to other services after successful creation
        var messageEvent = new OrderCreatedEvent
        {
            OrderId = order.Id,
            CustomerId = order.CustomerId,
            RestaurantId = order.RestaurantId,
            TotalAmount = order.TotalAmount,
            CreatedAt = DateTime.UtcNow
        };

        await _publishEndpoint.Publish(messageEvent, cancellationToken);

        // 5. SUCCESS RESPONSE: Return the result to the API
        return ResponseDto<Guid>.Success(order.Id, 201);
    }
}