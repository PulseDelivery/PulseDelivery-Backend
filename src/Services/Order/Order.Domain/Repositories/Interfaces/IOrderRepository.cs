using Order.Domain.Entities;

namespace Order.Domain.Repositories;

public interface IOrderRepository
{
    Task AddAsync(OrderEntity order);
    Task<OrderEntity?> GetByIdAsync(Guid id);
}