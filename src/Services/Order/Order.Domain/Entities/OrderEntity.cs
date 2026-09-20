using Order.Domain.Enums;
using Order.Domain.States;

namespace Order.Domain.Entities;

public class OrderEntity
{
    public Guid Id { get; private set; }
    public string CustomerId { get; private set; }
    public string RestaurantId { get; private set; }
    public decimal TotalAmount { get; private set; }
    
    // Status can only be updated from within the Domain
    public OrderStatus Status { get; internal set; }
    
    // EF Core will not persist this field to the database (ignored in Infrastructure); it is only used for domain logic
    internal IOrderState State { get; set; }

    // Parameterless constructor for EF Core
    private OrderEntity() { }

    // Creates a new order (Factory Method)
    public static OrderEntity Create(string customerId, string restaurantId, decimal totalAmount)
    {
        return new OrderEntity
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            RestaurantId = restaurantId,
            TotalAmount = totalAmount,
            Status = OrderStatus.Pending,
            State = new PendingState()
        };
    }

    // State delegation - Only these methods can be called from outside
    public void StartPreparing() => State.SetPreparing(this);
    public void SendToDelivery() => State.SetOutForDelivery(this);
    public void Deliver() => State.SetDelivered(this);
    public void Cancel() => State.SetCanceled(this);
}   