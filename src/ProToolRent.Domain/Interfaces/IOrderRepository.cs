using ProToolRent.Domain.Entities;

namespace ProToolRent.Domain.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Order order, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddOrderItemAsync(OrderItem item, CancellationToken cancellationToken = default);
    Task<List<Order>> GetOrderByUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
