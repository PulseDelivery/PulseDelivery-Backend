using Order.Domain.Entities;

namespace Order.Domain.States;

public interface IOrderState
{
    void SetPreparing(OrderEntity order);
    void SetOutForDelivery(OrderEntity order);
    void SetDelivered(OrderEntity order);
    void SetCanceled(OrderEntity order);
}