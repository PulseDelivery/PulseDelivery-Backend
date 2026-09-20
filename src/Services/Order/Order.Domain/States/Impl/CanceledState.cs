using Order.Domain.Entities;

namespace Order.Domain.States;

public class CanceledState : IOrderState
{
    public void SetPreparing(OrderEntity order) =>
        throw new InvalidOperationException("A canceled order cannot be restored.");

    public void SetOutForDelivery(OrderEntity order) =>
        throw new InvalidOperationException("A canceled order cannot be restored.");

    public void SetDelivered(OrderEntity order) =>
        throw new InvalidOperationException("A canceled order cannot be restored.");

    public void SetCanceled(OrderEntity order) =>
        throw new InvalidOperationException("The order has already been canceled.");
}