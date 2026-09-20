using Order.Domain.Entities;
using Order.Domain.Enums;

namespace Order.Domain.States;

public class PreparingState : IOrderState
{
    public void SetPreparing(OrderEntity order)
    {
        throw new InvalidOperationException("The order is already being prepared.");
    }

    public void SetOutForDelivery(OrderEntity order)
    {
        order.Status = OrderStatus.OutForDelivery;
        order.State = new OutForDeliveryState();
    }

    public void SetDelivered(OrderEntity order)
    {
        throw new InvalidOperationException(
            "An order that is being prepared cannot be delivered directly.");
    }

    public void SetCanceled(OrderEntity order)
    {
        throw new InvalidOperationException(
            "An order that has started being prepared cannot be canceled. Please contact customer service.");
    }
}