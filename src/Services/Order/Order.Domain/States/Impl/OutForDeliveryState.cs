using Order.Domain.Entities;
using Order.Domain.Enums;

namespace Order.Domain.States;

public class OutForDeliveryState : IOrderState
{
    public void SetPreparing(OrderEntity order)
    {
        throw new InvalidOperationException(
            "An order that is already out for delivery cannot be moved back to preparing.");
    }

    public void SetOutForDelivery(OrderEntity order)
    {
        throw new InvalidOperationException("The order is already out for delivery.");
    }

    public void SetDelivered(OrderEntity order)
    {
        order.Status = OrderStatus.Delivered;
        order.State = new DeliveredState();
    }

    public void SetCanceled(OrderEntity order)
    {
        order.Status = OrderStatus.Canceled;
        order.State = new CanceledState();
    }
}