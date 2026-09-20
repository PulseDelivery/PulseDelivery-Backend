using Order.Domain.Entities;
using Order.Domain.Enums;

namespace Order.Domain.States;

public class PendingState : IOrderState
{
    public void SetPreparing(OrderEntity order)
    {
        order.Status = OrderStatus.Preparing;
        order.State = new PreparingState();
    }

    public void SetOutForDelivery(OrderEntity order)
    {
        throw new InvalidOperationException("A pending order cannot go directly to out for delivery!");
    }

    public void SetDelivered(OrderEntity order)
    {
        throw new InvalidOperationException("A pending order cannot be delivered directly!");
    }

    public void SetCanceled(OrderEntity order)
    {
        order.Status = OrderStatus.Canceled; 
        order.State = new CanceledState();
    }
}