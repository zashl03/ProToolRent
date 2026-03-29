using Microsoft.EntityFrameworkCore;
using ProToolRent.Domain.Entities;
using ProToolRent.Domain.Interfaces;
using ProToolRent.Infrastructure.Persistence;

namespace ProToolRent.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _context.Orders.FirstOrDefaultAsync(o => o.Id == id, ct);
    }

    public async Task AddAsync(Order order, CancellationToken ct)
    {
        await _context.Orders.AddAsync(order, ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var order = await GetByIdAsync(id, ct);

        if (order != null)
        {
            _context.Orders.Remove(order);
        }
    }

    public async Task<OrderItem?> GetOrderItemByIdAsync(Guid id, CancellationToken ct)
    {
        return await _context.OrderItems.FirstOrDefaultAsync(o => o.Id == id, ct);
    }

    public async Task<List<Order>> GetOrderByUserAsync(Guid id, CancellationToken ct)
    {
        return await _context.Orders.Where(o => o.UserId == id).ToListAsync();
    }
}
