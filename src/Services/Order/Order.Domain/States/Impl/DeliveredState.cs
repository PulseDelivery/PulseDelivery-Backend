using Order.Domain.Entities;

namespace Order.Domain.States;

public class DeliveredState : IOrderState
{
    public void SetPreparing(OrderEntity order) =>
        throw new InvalidOperationException("A delivered order cannot be modified.");

    public void SetOutForDelivery(OrderEntity order) =>
        throw new InvalidOperationException("A delivered order cannot be modified.");

    public void SetDelivered(OrderEntity order) =>
        throw new InvalidOperationException("The order has already been delivered.");

    public void SetCanceled(OrderEntity order) =>
        throw new InvalidOperationException("A delivered order cannot be canceled.");
}